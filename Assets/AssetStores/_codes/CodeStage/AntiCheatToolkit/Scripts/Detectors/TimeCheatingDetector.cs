#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
#define ACTK_DEBUG_ENABLED
#endif

#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
#endif

using System;
using CodeStage.AntiCheat.Common;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

#if !ACTK_PREVENT_INTERNET_PERMISSION
using System.Net;
using System.Net.Sockets;
#endif

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Allows to detect time cheating using time servers. Needs internet connection.
	/// </summary>
	/// Doesn't detects cheating if there is no internet connection or if it's too weak to gather time from time servers.<br/>
	/// Just add it to any GameObject as usual or through the "GameObject > Create Other > Code Stage > Anti-Cheat Toolkit"
	/// menu to get started.<br/>
	/// You can use detector completely from inspector without writing any code except the actual reaction on cheating.
	/// 
	/// Avoid using detectors from code at the Awake phase.<br/>
	/// 
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Windows Store platform is not supported!</strong>
	[AddComponentMenu(MENU_PATH + COMPONENT_NAME)]
	[HelpURL(ACTkConstants.DOCS_ROOT_URL + "class_code_stage_1_1_anti_cheat_1_1_detectors_1_1_time_cheating_detector.html")]
	public class TimeCheatingDetector : ActDetectorBase
	{
		internal const string COMPONENT_NAME = "Time Cheating Detector";

		private const string LOG_PREFIX = ACTkConstants.LOG_PREFIX + COMPONENT_NAME + ": ";

#if !UNITY_WINRT && !ACTK_PREVENT_INTERNET_PERMISSION
		private const string TIME_SERVER = "pool.ntp.org";
		private const int NTP_DATA_BUFFER_LENGTH = 48;

		private static int instancesInScene;

		private readonly DateTime date1900 = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		protected UnityAction errorAction;
#endif

		#region public fields

		/// <summary> 
		/// Time (in minutes) between detector checks.
		/// </summary>
		[Tooltip("Time (in minutes) between detector checks.")]
		[Range(1, 60)]
		public int interval = 1;

		/// <summary>
		/// Maximum allowed difference between online and offline time, in minutes.
		/// </summary>
		[Tooltip("Maximum allowed difference between online and offline time, in minutes.")]
		public int threshold = 65;

		#endregion

#if !UNITY_WINRT && !ACTK_PREVENT_INTERNET_PERMISSION

		#region private fields

		private Socket asyncSocket;
		private Action<double> getOnlineTimeCallback;
		private string targetHost;
		private byte[] targetIP;
		private IPEndPoint targetEndpoint;
		private byte[] ntpData = new byte[NTP_DATA_BUFFER_LENGTH];
		private SocketAsyncEventArgs connectArgs;
		private SocketAsyncEventArgs sendArgs;
		private SocketAsyncEventArgs receiveArgs;

		#endregion

		#region public static methods

		/// <summary>
		/// Starts detection.
		/// </summary>
		/// Make sure you have properly configured detector in scene with #autoStart disabled before using this method.
		public static void StartDetection()
		{
			if (Instance != null)
			{
				Instance.StartDetectionInternal(null, null, Instance.interval);
			}
			else
			{
				Debug.LogError(LOG_PREFIX + "can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		/// <summary>
		/// Starts detection with specified callback.
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="detectionCallback">Method to call after detection.</param>
		/// <param name="errorCallback">Method to call if detector will be not able to retrieve online time.</param>
		public static void StartDetection(UnityAction detectionCallback, UnityAction errorCallback = null)
		{
			StartDetection(detectionCallback, errorCallback, GetOrCreateInstance.interval);
		}

		/// <summary>
		/// Starts detection with specified callback using passed interval.<br/>
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="detectionCallback">Method to call after detection.</param>
		/// <param name="interval">Time in minutes between checks. Overrides #interval property.</param>
		public static void StartDetection(UnityAction detectionCallback, int interval)
		{
			StartDetection(detectionCallback, null, interval);
		}

		/// <summary>
		/// Starts detection with specified callback using passed interval.<br/>
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="detectionCallback">Method to call after detection.</param>
		/// <param name="errorCallback">Method to call if detector will be not able to retrieve online time.</param>
		/// <param name="interval">Time in minutes between checks. Overrides #interval property.</param>
		public static void StartDetection(UnityAction detectionCallback, UnityAction errorCallback, int interval)
		{
			GetOrCreateInstance.StartDetectionInternal(detectionCallback, errorCallback, interval);
		}

		/// <summary>
		/// Stops detector. Detector's component remains in the scene. Use Dispose() to completely remove detector.
		/// </summary>
		public static void StopDetection()
		{
			if (Instance != null) Instance.StopDetectionInternal();
		}

		/// <summary>
		/// Sets callback for errors during online time gathering process.
		/// </summary>
		/// <param name="errorCallback">Method to call if detector will be not able to retrieve online time.</param>
		public static void SetErrorCallback(UnityAction errorCallback)
		{
			if (Instance != null)
			{
				Instance.errorAction = errorCallback;
			}
			else
			{
				Debug.LogError(LOG_PREFIX + "Can't set error callback since detector is not created or initialized yet.");
			}
		}

		/// <summary>
		/// Stops and completely disposes detector component.
		/// </summary>
		/// On dispose Detector follows 2 rules:
		/// - if Game Object's name is "Anti-Cheat Toolkit Detectors": it will be automatically 
		/// destroyed if no other Detectors left attached regardless of any other components or children;<br/>
		/// - if Game Object's name is NOT "Anti-Cheat Toolkit Detectors": it will be automatically destroyed only
		/// if it has neither other components nor children attached;
		public static void Dispose()
		{
			if (Instance != null) Instance.DisposeInternal();
		}

		#endregion

		#region static instance

		/// <summary>
		/// Allows reaching public properties from code. Can be null.
		/// </summary>
		public static TimeCheatingDetector Instance { get; private set; }

		private static TimeCheatingDetector GetOrCreateInstance
		{
			get
			{
				if (Instance != null)
					return Instance;

				if (detectorsContainer == null)
				{
					detectorsContainer = new GameObject(CONTAINER_NAME);
				}
				Instance = detectorsContainer.AddComponent<TimeCheatingDetector>();
				return Instance;
			}
		}

		#endregion

		private TimeCheatingDetector()
		{
		} // prevents direct instantiation

		#region Unity messages
#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void Awake()
		{
			instancesInScene++;
			if (Init(Instance, COMPONENT_NAME))
			{
				Instance = this;
			}

#if UNITY_5_4_OR_NEWER
			SceneManager.sceneLoaded += OnLevelWasLoadedNew;
#endif
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		protected override void OnDestroy()
		{
			base.OnDestroy();
			instancesInScene--;
		}

#if UNITY_5_4_OR_NEWER
		private void OnLevelWasLoadedNew(Scene scene, LoadSceneMode mode)
		{
			OnLevelLoadedCallback();
		}
#else
#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void OnLevelWasLoaded(int level)
		{
			OnLevelLoadedCallback();
		}
#endif

		private void OnLevelLoadedCallback()
		{
			if (instancesInScene < 2)
			{
				if (!keepAlive)
				{
					DisposeInternal();
				}
			}
			else
			{
				if (!keepAlive && Instance != this)
				{
					DisposeInternal();
				}
			}
		}

		#endregion

		private void StartDetectionInternal(UnityAction detectionCallback, UnityAction errorCallback, int checkInterval)
		{
			if (isRunning)
			{
				Debug.LogWarning(LOG_PREFIX + "already running!", this);
				return;
			}

			if (!enabled)
			{
				Debug.LogWarning(
					LOG_PREFIX + "disabled but StartDetection still called from somewhere (see stack trace for this message)!",
					this);
				return;
			}

			if (detectionCallback != null && detectionEventHasListener)
			{
				Debug.LogWarning(
					LOG_PREFIX +
					"has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?",
					this);
			}

			if (detectionCallback == null && !detectionEventHasListener)
			{
				Debug.LogWarning(
					LOG_PREFIX +
					"was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.",
					this);
				enabled = false;
				return;
			}

			detectionAction = detectionCallback;
			errorAction = errorCallback;
			interval = checkInterval;

			InvokeRepeating("CheckForCheat", 0, interval * 60);

			started = true;
			isRunning = true;
		}

		protected override void StartDetectionAutomatically()
		{
			StartDetectionInternal(null, null, interval);
		}

		protected override void PauseDetector()
		{
			isRunning = false;
		}

		protected override void ResumeDetector()
		{
			if (detectionAction == null && !detectionEventHasListener) return;
			isRunning = true;
		}

		protected override void StopDetectionInternal()
		{
			if (!started)
				return;

			CancelInvoke("CheckForCheat");

			detectionAction = null;
			started = false;
			isRunning = false;
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (Instance == this) Instance = null;

			if (asyncSocket.Connected)
			{
				asyncSocket.Close();
			}
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void CheckForCheat()
		{
			if (!isRunning) return;
			GetOnlineTimeAsync(TIME_SERVER, OnTimeGot);
		}

		private void OnTimeGot(double onlineTime)
		{
			if (onlineTime <= 0)
			{
				if (errorAction != null) errorAction();
				Debug.LogWarning(LOG_PREFIX + "Can't retrieve time from time server!");
				return;
			}

			var offlineTime = GetLocalTime();

			var onlineTimeSpan = new TimeSpan((long) onlineTime * TimeSpan.TicksPerMillisecond);
			var offlineTimeSpan = new TimeSpan((long) offlineTime * TimeSpan.TicksPerMillisecond);

			/*Debug.Log("Server time: " + onlineTimeSpan.Hours + ':' + onlineTimeSpan.Minutes + ':' + onlineTimeSpan.Seconds + '.' + onlineTimeSpan.Milliseconds);
			Debug.Log("Local time: " + offlineTimeSpan.Hours + ':' + offlineTimeSpan.Minutes + ':' + offlineTimeSpan.Seconds + '.' + offlineTimeSpan.Milliseconds);

			Debug.Log("Server min: " + onlineTimeSpan.TotalMinutes);
			Debug.Log("Local min: " + offlineTimeSpan.TotalMinutes);*/

			var minutesDifference = onlineTimeSpan.TotalMinutes - offlineTimeSpan.TotalMinutes;
			if (Mathf.Abs((float)minutesDifference) > threshold)
			{
				OnCheatingDetected();
			}
		}

		/// <summary>
		/// Retrieves NTP time from the specified time server, e.g. "pool.ntp.org".
		/// </summary>
		/// May block main thread on poor connection until gets data from time server, or until 3 sec timeout is met.
		/// <param name="server">NTP time server address, e.g. "pool.ntp.org"</param>
		/// <returns>NTP time in milliseconds or -1 if there was an error getting time.</returns>
		public static double GetOnlineTime(string server)
		{
			try
			{
				var ntpData = new byte[NTP_DATA_BUFFER_LENGTH];

				ntpData[0] = 0x1B;

				var addresses = Dns.GetHostEntry(server).AddressList;
				var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				socket.Connect(new IPEndPoint(addresses[0], 123));
				socket.ReceiveTimeout = 3000;

				socket.Send(ntpData);
				socket.Receive(ntpData);
				socket.Close();

				var intc = (ulong) ntpData[40] << 24 | (ulong) ntpData[41] << 16 | (ulong) ntpData[42] << 8 | ntpData[43];
				var frac = (ulong) ntpData[44] << 24 | (ulong) ntpData[45] << 16 | (ulong) ntpData[46] << 8 | ntpData[47];

				return intc * 1000d + frac * 1000d / 0x100000000L;
			}
			catch (Exception exception)
			{
				Debug.Log(LOG_PREFIX + "Could not get NTP time from " + server + " =/\n" + exception);
				return -1;
			}
		}

		/// <summary>
		/// Asynchronously retrieves NTP time from the specified time server, e.g. "pool.ntp.org".
		/// </summary>
		/// Does not block main thread, has 3 sec timeout.
		/// <param name="server">NTP time server address, e.g. "pool.ntp.org"</param>
		/// <param name="callback">Method to call when time is taken from server (or when error occurs). 
		/// Argument - NTP time in milliseconds or -1 if there was an error getting time.</param>
		public void GetOnlineTimeAsync(string server, Action<double> callback)
		{
			try
			{
				var addresses = Dns.GetHostEntry(server).AddressList;
				if (addresses.Length == 0)
				{
					Debug.Log(LOG_PREFIX + "Could not resolve IP from the host " + server + " =/");
					callback(-1);
					return;
				}

				if (asyncSocket == null) asyncSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

				targetHost = server;

				var ip = addresses[0];
				var ipBytes = ip.GetAddressBytes();

				if (ipBytes != targetIP)
				{
					targetEndpoint = new IPEndPoint(ip, 123);
					targetIP = ipBytes;
				}

				if (connectArgs == null)
				{
					connectArgs = new SocketAsyncEventArgs();
					connectArgs.Completed += OnSockedConnected;
				}

				connectArgs.RemoteEndPoint = targetEndpoint;
				asyncSocket.ReceiveTimeout = 3000;

				getOnlineTimeCallback = callback;
				
				asyncSocket.ConnectAsync(connectArgs);
			}
			catch (Exception exception)
			{
				Debug.Log(LOG_PREFIX + "Could not get NTP time from " + server + " =/\n" + exception);
				callback(-1);
			}
		}

		private void OnSockedConnected(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.Success)
			{
				Debug.Log(LOG_PREFIX + "Could not get NTP time from " + targetHost + " =/\n" + e);
				if (getOnlineTimeCallback != null) getOnlineTimeCallback(-1);
				return;
			}

			if (!isRunning) return;

			ntpData[0] = 0x1B;

			if (sendArgs == null)
			{
				sendArgs = new SocketAsyncEventArgs();
				sendArgs.Completed += OnSocketSend;
				sendArgs.UserToken = asyncSocket;
				sendArgs.SetBuffer(ntpData, 0, NTP_DATA_BUFFER_LENGTH);
			}
			
			sendArgs.RemoteEndPoint = targetEndpoint;

			asyncSocket.SendAsync(sendArgs);
		}

		private void OnSocketSend(object sender, SocketAsyncEventArgs e)
		{
			if (!isRunning) return;

			if (e.SocketError == SocketError.Success)
			{
				if (e.LastOperation == SocketAsyncOperation.Send)
				{
					if (receiveArgs == null)
					{
						receiveArgs = new SocketAsyncEventArgs();
						receiveArgs.Completed += OnSocketReceive;
						receiveArgs.UserToken = asyncSocket;
						receiveArgs.SetBuffer(ntpData, 0, NTP_DATA_BUFFER_LENGTH);
					}
					
					receiveArgs.RemoteEndPoint = targetEndpoint;

					asyncSocket.ReceiveAsync(receiveArgs);
				}
			}
			else
			{
				Debug.Log(LOG_PREFIX + "Could not get NTP time from " + targetHost + " =/\n" + e);
				if (getOnlineTimeCallback != null) getOnlineTimeCallback(-1);
			}
		}

		private void OnSocketReceive(object sender, SocketAsyncEventArgs e)
		{
			if (!isRunning) return;

			ntpData = e.Buffer;

			var intc = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | ntpData[43];
			var frac = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | ntpData[47];

			var time = intc * 1000d + frac * 1000d / 0x100000000L;

			if (getOnlineTimeCallback != null) getOnlineTimeCallback(time);
		}

		private double GetLocalTime()
		{
			return DateTime.UtcNow.Subtract(date1900).TotalMilliseconds;
		}
#else // for WIN_RT

		public static void StartDetection()
		{
			Debug.LogError(LOG_PREFIX + "is not supported on selected platform or disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional!");
		}

		public static void StartDetection(UnityAction detectionCallback, UnityAction errorCallback = null)
		{
			Debug.LogError(LOG_PREFIX + "is not supported on selected platform or disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional!");
		}

		public static void StartDetection(UnityAction detectionCallback, int interval)
		{
			Debug.LogError(LOG_PREFIX + "is not supported on selected platform or disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional!");
		}

		public static void StartDetection(UnityAction detectionCallback, UnityAction errorCallback, int interval)
		{
			Debug.LogError(LOG_PREFIX + "is not supported on selected platform or disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional!");
		}

		public static void StopDetection()
		{
			Debug.LogError(LOG_PREFIX + "is not supported on selected platform or disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional!");
		}
		
		public static void Dispose()
		{
			Debug.LogError(LOG_PREFIX + "is not supported on selected platform or disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional!");
		}

		public static void SetErrorCallback(UnityAction errorCallback)
		{
			Debug.LogError(LOG_PREFIX + "is not supported on selected platform or disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional!");
		}

		public void GetOnlineTimeAsync(string server, Action<double> callback)
		{
			Debug.LogError(LOG_PREFIX + "is not supported on selected platform or disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional!");

		}

		public static double GetOnlineTime(string server)
		{
			Debug.LogError(LOG_PREFIX + "is not supported on selected platform or disabled with ACTK_PREVENT_INTERNET_PERMISSION conditional!");
			return -1;
		}

		protected override void StartDetectionAutomatically()
		{
		}

		protected override void StopDetectionInternal()
		{
		}

		protected override void PauseDetector()
		{
		}

		protected override void ResumeDetector()
		{
		}
#endif
	}
}