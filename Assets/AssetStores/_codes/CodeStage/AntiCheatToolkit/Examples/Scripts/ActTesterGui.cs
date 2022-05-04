using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;
using CodeStage.AntiCheat.Common;

// If you're going to use any obscured types / prefs from code you'll need to use this name space:
using CodeStage.AntiCheat.ObscuredTypes;

// If you're going to use detectors from code you'll need to use this name space:
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.Examples
{
	[AddComponentMenu("")]
	public class ActTesterGui : MonoBehaviour
	{
		private const string RED_COLOR = "#FF4040";
		private const string GREEN_COLOR = "#02C85F";

		#region ObscuredPrefs example constants
		private const string PREFS_STRING = "name";
		private const string PREFS_INT = "money";
		private const string PREFS_FLOAT = "lifeBar";
		private const string PREFS_BOOL = "gameComplete";
		private const string PREFS_UINT = "demoUint";
		private const string PREFS_LONG = "demoLong";
		private const string PREFS_DOUBLE = "demoDouble";
		private const string PREFS_VECTOR2 = "demoVector2";
		private const string PREFS_VECTOR3 = "demoVector3";
		private const string PREFS_QUATERNION = "demoQuaternion";
		private const string PREFS_RECT = "demoRect";
		private const string PREFS_COLOR = "demoColor";
		private const string PREFS_BYTE_ARRAY = "demoByteArray";
		#endregion

		[Header("Regular variables")]
		public string regularString = "I'm regular string";

		public int regularInt = 1987;
		public float regularFloat = 2013.0524f;
		public Vector3 regularVector3 = new Vector3(10.5f, 11.5f, 12.5f);

		[Header("Obscured (secure) variables")]
		public ObscuredString obscuredString = "I'm obscured string";
		public ObscuredInt obscuredInt = 1987;
		public ObscuredFloat obscuredFloat = 2013.0524f;
		public ObscuredVector3 obscuredVector3 = new Vector3(10.5f, 11.5f, 12.5f);
		public ObscuredBool obscuredBool = true;
		public ObscuredLong obscuredLong = 945678987654123345L;
		public ObscuredDouble obscuredDouble = 9.45678987654d;
		public ObscuredVector2 obscuredVector2 = new Vector2(8.5f, 9.5f);
		public ObscuredDecimal obscuredDecimal = 503.4521m;

#if UNITY_2017_2_OR_NEWER
		public ObscuredVector2Int obscuredVector2Int = new Vector2Int(8, 9);
		public ObscuredVector3Int obscuredVector3Int = new Vector3Int(15, 16, 17);
#endif

		[Header("Other")]
		// This is a small trick - it allows to hide your encryption key 
		// in the serialized MonoBehaviour in the Editor inspector, 
		// outside of the IL byte code, so hacker will need to 
		// know how to reach it in the Unity serialized files to be able to read it ;)
		public string prefsEncryptionKey = "change me!";

		private readonly string[] tabs = {"Variables protection", "Saves protection", "Cheating detectors"};
		private int currentTab;

		private string allSimpleObscuredTypes;
		private string regularPrefs;
		private string obscuredPrefs;
		private int savesLock;
		private bool savesAlterationDetected;
		private bool foreignSavesDetected;

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_ANDROID
		private bool injectionDetected;
#endif
		private bool speedHackDetected;
#if !UNITY_WINRT && !ACTK_PREVENT_INTERNET_PERMISSION
		private bool timeCheatingDetected;
#endif
		private bool obscuredTypeCheatDetected;
		private bool wallHackCheatDetected;

		private readonly StringBuilder logBuilder = new StringBuilder();

#region detectors callbacks
		// These methods are get called by the Detection Events of detectors placed at the
		// Anti-Cheat Toolkit Detectors game object.
		public void OnSpeedHackDetected()
		{
			speedHackDetected = true;
			Debug.Log("Speed hack Detected!");
		}

		public void OnTimeCheatingDetected()
		{
#if !UNITY_WINRT && !ACTK_PREVENT_INTERNET_PERMISSION
			timeCheatingDetected = true;
			Debug.Log("Time cheating Detected!");
#endif
		}

		private void OnTimeCheatingError()
		{
			Debug.LogWarning("Could not get online time at Time Cheating Detector!");
		}

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_ANDROID
		public void OnInjectionDetected()
		{
			injectionDetected = true;
			Debug.Log("Injection Detected!");
		}

		// cause will have detection cause or full assembly name
		public void OnInjectionDetectedWithCause(string cause)
		{
			injectionDetected = true;
			Debug.Log("Injection Detected! Cause: " + cause);
		}
#endif

		public void OnObscuredTypeCheatingDetected()
		{
			obscuredTypeCheatDetected = true;
			Debug.Log("Obscured Vars Cheating Detected!");
		}

		public void OnWallHackDetected()
		{
			wallHackCheatDetected = true;
			Debug.Log("Wall hack Detected!");
		}
		#endregion

		// this is needed to avoid ObscuredPrefs crypto key reset on scripts reload
		// useful for debugging purposes
		private void OnValidate()
		{
			if (Application.isPlaying) ObscuredPrefs.CryptoKey = prefsEncryptionKey;
		} 

		private void Awake()
		{
			// setting new encryption key you've set in the inspector
			ObscuredPrefs.CryptoKey = prefsEncryptionKey;

			// we may react on saves alteration
			ObscuredPrefs.onAlterationDetected = SavesAlterationDetected;

			// and even may react on foreign saves (from another device)
			ObscuredPrefs.onPossibleForeignSavesDetected = ForeignSavesDetected;
		}

		private void Start() 
		{
			ObscuredStringExample();
			ObscuredIntExample();
			ObscuredFloatExample();
			ObscuredVector3Example();

			logBuilder.Length = 0;
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "ObscuredDecimal value from inspector: " + obscuredDecimal);
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "ObscuredBool value from inspector: " + obscuredBool);
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "ObscuredLong value from inspector: " + obscuredLong);
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "ObscuredDouble value from inspector: " + obscuredDouble);
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "ObscuredVector2 value from inspector: " + obscuredVector2);

#if UNITY_2017_2_OR_NEWER
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "obscuredVector2Int value from inspector: " + obscuredVector2Int);
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "obscuredVector3Int value from inspector: " + obscuredVector3Int);
#endif
			Debug.Log(logBuilder);

			// read RandomizeCryptoKey() API docs for details
			Invoke("RandomizeObscuredVars", Random.Range(1f, 10f));

			#region detectors examples
			// Since ACTk v1.5 all detectors have auto start option enabled by default, so you don't need to
			// write any code to start them anymore.
			// However, if you still wish to control detectors from code - it's possible to do.
			// You have 3 options for using detectors from code in general now:
			//
			// - configure detector in inspector, disable Auto Start, 
			//   fill Detection Event and start it via StartDetection();
			//
			// - configure detector in inspector, disable Auto Start, 
			//   do not fill Detection Event and start it via StartDetection(UnityAction);
			//
			// - do not add detector to your scene at all and create it completely from code using StartDetection(UnityAction);
			//

			// SpeedHackDetector pure code usage example.
			// In this case we subscribe to the speed hack detection event,
			// set detector update interval to 1 second, allowing 5 false positives and
			// allowing Cool Down after 60 seconds (read more about Cool Down in the readme.pdf).
			// Thus OnSpeedHackDetected normally will execute after 5 seconds since 
			// speed hack was applied to the application.
			// Please, note, if we have detector added to scene, all settings
			// we made there in inspector will be overridden by settings we pass
			// to the StartDetection(); e.g.:
			// SpeedHackDetector.StartDetection(OnSpeedHackDetected, 1f, 5, 60);

			// InjectionDetector pure code usage example.
			// InjectionDetector supports only these platforms
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_ANDROID
			// and it may be started as usual, with parameterless callback...
			// InjectionDetector.StartDetection(OnInjectionDetected);
			// ... or with 1 string parameter callback to get the detected assembly
			// InjectionDetector.StartDetection(OnInjectionDetectedWithCause);
#endif

			// We can change all options of any detectors from code like this:
			// ObscuredCheatingDetector.StartDetection(OnObscuredTypeCheatingDetected);
			// ObscuredCheatingDetector.Instance.autoDispose = true;
			// ObscuredCheatingDetector.Instance.keepAlive = true;

#if !UNITY_WINRT && !ACTK_PREVENT_INTERNET_PERMISSION
			// You also can set delegate for error event of the TimeCheatingDetector like this:
			TimeCheatingDetector.SetErrorCallback(OnTimeCheatingError);
#endif
#endregion
		}

		private void RandomizeObscuredVars()
		{
			obscuredInt.RandomizeCryptoKey();
			obscuredFloat.RandomizeCryptoKey();
			obscuredString.RandomizeCryptoKey();
			obscuredVector3.RandomizeCryptoKey();
			Invoke("RandomizeObscuredVars", Random.Range(1f, 10f));
		}

		private void ObscuredStringExample()
		{
			logBuilder.Length = 0;
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "<b>[ ObscuredString test ]</b>");

			// example of custom crypto key using
			ObscuredString.SetNewCryptoKey("I LOVE MY GIRLz");

			// just a small self-test here (hey, Daniele! :D)
			var regular = "the Goscurry is not a lie ;)";
			logBuilder.AppendLine("Original string:\n" + regular);

			ObscuredString obscured = regular;
			logBuilder.AppendLine("How your string is stored in memory when obscured:\n" + obscured.GetEncrypted());

			Debug.Log(logBuilder);
		}

		private void ObscuredIntExample()
		{
			logBuilder.Length = 0;
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "<b>[ ObscuredInt test ]</b>");

			// example of custom crypto key using
			ObscuredInt.SetNewCryptoKey(434523);

			// just a small self-test here
			var regular = 5;
			logBuilder.AppendLine("Original lives count: " + regular);

			ObscuredInt obscured = regular;
			logBuilder.AppendLine("How your lives count is stored in memory when obscured: " + obscured.GetEncrypted());

			ObscuredInt.SetNewCryptoKey(666); // you can change crypto key at any time!

			// all usual operations are supported
			regular = obscured;
			obscured -= 2;
			obscured = obscured + regular + 10;
			obscured = obscured/2;
			obscured++;
			ObscuredInt.SetNewCryptoKey(999); // you can change crypto key at any time!
			obscured++;
			obscured--;

			logBuilder.AppendLine("Lives count after few usual operations: " + obscured + " (" + obscured.ToString("X") + "h)");

			Debug.Log(logBuilder);
		}

		private void ObscuredFloatExample()
		{
			logBuilder.Length = 0;
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "<b>[ ObscuredFloat test ]</b>");

			// example of custom crypto key using
			ObscuredFloat.SetNewCryptoKey(404);

			// just a small self-test here
			var regular = 99.9f;
			logBuilder.AppendLine("Original health bar: " + regular);

			ObscuredFloat obscured = regular;
			logBuilder.AppendLine("How your health bar is stored in memory when obscured: " + obscured.GetEncrypted());

			ObscuredFloat.SetNewCryptoKey(666); // you can change crypto key at any time!

			// all usual operations are supported, dummy code, just for demo purposes
			obscured += 6f;
			obscured -= 1.5f;
			obscured++;
			obscured--;
			obscured--;
			obscured = regular - obscured + 10.5f;

			logBuilder.AppendLine("Health bar after few usual operations: " + obscured);

			Debug.Log(logBuilder);
		}

		private void ObscuredVector3Example()
		{
			logBuilder.Length = 0;
			logBuilder.AppendLine(ACTkConstants.LOG_PREFIX + "<b>[ ObscuredVector3 test ]</b>");

			// example of custom crypto key using
			ObscuredVector3.SetNewCryptoKey(404);

			// just a small self-test here
			var regular = new Vector3(54.1f, 64.3f, 63.2f);
			logBuilder.AppendLine("Original position: " + regular);

			ObscuredVector3 obscured = regular;

			var encrypted = obscured.GetEncrypted();
			logBuilder.AppendLine("How your position is stored in memory when obscured: (" + encrypted.x + ", " + encrypted.y + ", " + encrypted.z + ")");

			Debug.Log(logBuilder);
		}

		private void SavesAlterationDetected()
		{
			savesAlterationDetected = true;
		}

		private void ForeignSavesDetected()
		{
			foreignSavesDetected = true;
		}

		private void OnGUI()
		{
			var centeredStyle = new GUIStyle(GUI.skin.label);
			centeredStyle.alignment = TextAnchor.UpperCenter;

			GUILayout.BeginArea(new Rect(10, 5, Screen.width - 20, Screen.height - 10));

			GUILayout.Label("<color=\"#0287C8\"><b>Anti-Cheat Toolkit Sandbox</b></color>", centeredStyle);
			GUILayout.Label("Here you can overview common ACTk features and try to cheat something yourself.", centeredStyle);
			GUILayout.Space(5);

			currentTab = GUILayout.Toolbar(currentTab, tabs);

			if (currentTab == 0)
			{
#region obscured types tab
				GUILayout.Label("ACTk offers own collection of the secure types to let you protect your variables from <b>ANY</b> memory hacking tools (Cheat Engine, ArtMoney, GameCIH, Game Guardian, etc.).");
				GUILayout.Space(5);
				using (new HorizontalLayout())
				{
					GUILayout.Label("<b>Obscured types:</b>\n<color=\"#75C4EB\">" + GetAllSimpleObscuredTypes() + "</color>", GUILayout.MinWidth(130));
					GUILayout.Space(10);
					using (new VerticalLayout(GUI.skin.box))
					{
						GUILayout.Label("Below you can try to cheat few variables of the regular types and their obscured (secure) analogues (you may change initial values from Tester object inspector):");

#region string
						GUILayout.Space(10);
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>string:</b> " + regularString, GUILayout.Width(250));
							if (GUILayout.Button("Add random value"))
							{
								regularString += (char)Random.Range(97, 122);
							}
							if (GUILayout.Button("Reset"))
							{
								regularString = "";
							}
						}

						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>ObscuredString:</b> " + obscuredString, GUILayout.Width(250));
							if (GUILayout.Button("Add random value"))
							{
								obscuredString += (char)Random.Range(97, 122);
							}
							if (GUILayout.Button("Reset"))
							{
								obscuredString = "";
							}
						}
#endregion

#region int
						GUILayout.Space(10);
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>int:</b> " + regularInt, GUILayout.Width(250));
							if (GUILayout.Button("Add random value"))
							{
								regularInt += Random.Range(1, 100);
							}
							if (GUILayout.Button("Reset"))
							{
								regularInt = 0;
							}
						}

						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>ObscuredInt:</b> " + obscuredInt, GUILayout.Width(250));
							if (GUILayout.Button("Add random value"))
							{
								obscuredInt += Random.Range(1, 100);
							}
							if (GUILayout.Button("Reset"))
							{
								obscuredInt = 0;
							}
						}
#endregion

#region float
						GUILayout.Space(10);
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>float:</b> " + regularFloat, GUILayout.Width(250));
							if (GUILayout.Button("Add random value"))
							{
								regularFloat += Random.Range(1f, 100f);
							}
							if (GUILayout.Button("Reset"))
							{
								regularFloat = 0;
							}
						}

						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>ObscuredFloat:</b> " + obscuredFloat, GUILayout.Width(250));
							if (GUILayout.Button("Add random value"))
							{
								obscuredFloat += Random.Range(1f, 100f);
							}
							if (GUILayout.Button("Reset"))
							{
								obscuredFloat = 0;
							}
						}
#endregion

#region Vector3
						GUILayout.Space(10);
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>Vector3:</b> " + regularVector3, GUILayout.Width(250));
							if (GUILayout.Button("Add random value"))
							{
								regularVector3 += Random.insideUnitSphere;
							}
							if (GUILayout.Button("Reset"))
							{
								regularVector3 = Vector3.zero;
							}
						}

						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>ObscuredVector3:</b> " + obscuredVector3, GUILayout.Width(250));
							if (GUILayout.Button("Add random value"))
							{
								obscuredVector3 += Random.insideUnitSphere;
							}
							if (GUILayout.Button("Reset"))
							{
								obscuredVector3 = Vector3.zero;
							}
						}
#endregion
					}
				}
#endregion
			}
			else if (currentTab == 1)
			{
#region obscured prefs tab
				GUILayout.Label("ACTk has secure layer for the PlayerPrefs: <color=\"#75C4EB\">ObscuredPrefs</color>. It protects data from view, detects any cheating attempts, optionally locks data to the current device and supports additional data types.");
				GUILayout.Space(5);
				using (new HorizontalLayout())
				{
					GUILayout.Label("<b>Supported types:</b>\n" + GetAllObscuredPrefsDataTypes(), GUILayout.MinWidth(130));
					using (new VerticalLayout(GUI.skin.box))
					{
						GUILayout.Label("Below you can try to cheat both regular PlayerPrefs and secure ObscuredPrefs:");
						using (new VerticalLayout())
						{
							GUILayout.Label("<color=\"" + RED_COLOR + "\"><b>PlayerPrefs:</b></color>\neasy to cheat, only 3 supported types", centeredStyle);
							GUILayout.Space(5);
							if (string.IsNullOrEmpty(regularPrefs))
							{
								LoadRegularPrefs();
							}
							using (new HorizontalLayout())
							{
								GUILayout.Label(regularPrefs, GUILayout.Width(270));
								using (new VerticalLayout())
								{
									using (new HorizontalLayout())
									{
										if (GUILayout.Button("Save"))
										{
											SaveRegularPrefs();
										}
										if (GUILayout.Button("Load"))
										{
											LoadRegularPrefs();
										}
									}
									if (GUILayout.Button("Delete"))
									{
										DeleteRegularPrefs();
									}
								}
							}
						}
						GUILayout.Space(5);
						using (new VerticalLayout())
						{
							GUILayout.Label("<color=\"" + GREEN_COLOR + "\"><b>ObscuredPrefs:</b></color>\nsecure, lot of additional types and extra options", centeredStyle);
							GUILayout.Space(5);
							if (string.IsNullOrEmpty(obscuredPrefs))
							{
								LoadObscuredPrefs();
							}

							using (new HorizontalLayout())
							{
								GUILayout.Label(obscuredPrefs, GUILayout.Width(270));
								using (new VerticalLayout())
								{
									using (new HorizontalLayout())
									{
										if (GUILayout.Button("Save"))
										{
											SaveObscuredPrefs();
										}
										if (GUILayout.Button("Load"))
										{
											LoadObscuredPrefs();
										}
									}
									if (GUILayout.Button("Delete"))
									{
										DeleteObscuredPrefs();
									}

									using (new HorizontalLayout())
									{
										GUILayout.Label("LockToDevice level");
									}
									savesLock = GUILayout.SelectionGrid(savesLock, new[] {ObscuredPrefs.DeviceLockLevel.None.ToString(), ObscuredPrefs.DeviceLockLevel.Soft.ToString(), ObscuredPrefs.DeviceLockLevel.Strict.ToString()}, 3);
									ObscuredPrefs.lockToDevice = (ObscuredPrefs.DeviceLockLevel)savesLock;
									GUILayout.Space(5);
									using (new HorizontalLayout())
									{
										ObscuredPrefs.preservePlayerPrefs = GUILayout.Toggle(ObscuredPrefs.preservePlayerPrefs, "preservePlayerPrefs");
									}
									using (new HorizontalLayout())
									{
										ObscuredPrefs.emergencyMode = GUILayout.Toggle(ObscuredPrefs.emergencyMode, "emergencyMode");
									}
									using (new HorizontalLayout())
									{
										ObscuredPrefs.readForeignSaves = GUILayout.Toggle(ObscuredPrefs.readForeignSaves, "readForeignSaves");
									}
#if UNITY_EDITOR
									using (new HorizontalLayout())
									{
										ObscuredPrefs.unobscuredMode = GUILayout.Toggle(ObscuredPrefs.unobscuredMode, "unobscuredMode");
									}
#endif
									GUILayout.Space(5);
									GUILayout.Label("<color=\"" + (savesAlterationDetected ? RED_COLOR : GREEN_COLOR) + "\">Saves modification detected: " + savesAlterationDetected + "</color>");
									GUILayout.Label("<color=\"" + (foreignSavesDetected ? RED_COLOR : GREEN_COLOR) + "\">Foreign saves detected: " + foreignSavesDetected + "</color>");
								}
							}
						}
						GUILayout.Space(5);
					}
				}
#endregion
			}
			else
			{
				GUILayout.Label("ACTk is able to detect some types of cheating to let you take action on the cheating players. This example scene has all possible detectors and all of them are automatically start on scene start.");
				GUILayout.Space(5);
				using (new VerticalLayout(GUI.skin.box))
				{
					GUILayout.Label("<b>" + SpeedHackDetector.COMPONENT_NAME + "</b>");
					GUILayout.Label("Allows to detect Cheat Engine's speed hack (and maybe some other speed hack tools) usage.");
                    GUILayout.Label("Running: " + SpeedHackDetector.Instance.IsRunning + "\n<color=\"" + (speedHackDetected ? RED_COLOR : GREEN_COLOR) + "\">Detected: " + speedHackDetected.ToString().ToLower() + "</color>");
					GUILayout.Space(10);
					GUILayout.Label("<b>" + TimeCheatingDetector.COMPONENT_NAME + "</b> (updates once per 1 min by default)");
					GUILayout.Label("Allows to detect system time change to cheat some long-term processes (building progress, etc.).");
#if !UNITY_WINRT && !ACTK_PREVENT_INTERNET_PERMISSION
					GUILayout.Label("Running: " + TimeCheatingDetector.Instance.IsRunning + "\n<color=\"" + (timeCheatingDetected ? RED_COLOR : GREEN_COLOR) + "\">Detected: " + timeCheatingDetected.ToString().ToLower() + "</color>");
#else
					GUILayout.Label("Not supported on this platform or was disabled with ACTK_PREVENT_INTERNET_PERMISSION compilation symbol.");
#endif
					GUILayout.Space(10);
                    GUILayout.Label("<b>" + ObscuredCheatingDetector.COMPONENT_NAME + "</b>");
					GUILayout.Label("Detects cheating of any Obscured type (except ObscuredPrefs, it has own detection features) used in project.");
					GUILayout.Label("Running: " + ObscuredCheatingDetector.Instance.IsRunning + "\n<color=\"" + (obscuredTypeCheatDetected ? RED_COLOR : GREEN_COLOR) + "\">Detected: " + obscuredTypeCheatDetected.ToString().ToLower() + "</color>");
					GUILayout.Space(10);
					GUILayout.Label("<b>" + WallHackDetector.COMPONENT_NAME + "</b>");
					GUILayout.Label("Detects common types of wall hack cheating: walking through the walls (Rigidbody and CharacterController modules), shooting through the walls (Raycast module), looking through the walls (Wireframe module).");
					GUILayout.Label("Running: " + WallHackDetector.Instance.IsRunning + "\n<color=\"" + (wallHackCheatDetected ? RED_COLOR : GREEN_COLOR) + "\">Detected: " + wallHackCheatDetected.ToString().ToLower() + "</color>");
					GUILayout.Space(10);
					GUILayout.Label("<b>" + InjectionDetector.COMPONENT_NAME + "</b>");
					GUILayout.Label("Allows to detect foreign managed assemblies in your application.");
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_ANDROID
					GUILayout.Label("Running: " + (InjectionDetector.Instance != null && InjectionDetector.Instance.IsRunning) + "\n<color=\"" + (injectionDetected ? RED_COLOR : GREEN_COLOR) + "\">Detected: " + injectionDetected.ToString().ToLower() + "</color>");
#else
					GUILayout.Label("Injection detection is not available on current platform");
#endif
				}
			}
			GUILayout.EndArea();
		}

		private string GetAllSimpleObscuredTypes()
		{
			var result = "Can't use reflection here, sorry :(";
#if !UNITY_WINRT
			var types = "";

			if (string.IsNullOrEmpty(allSimpleObscuredTypes))
			{	
				var csharpAssembly = AppDomain.CurrentDomain.GetAssemblies().
															SingleOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp-firstpass");
				if (csharpAssembly != null)
				{
					var q = from t in csharpAssembly.GetTypes()
							where t.IsPublic && t.Namespace == "CodeStage.AntiCheat.ObscuredTypes" && t.Name != "ObscuredPrefs"
							select t;
					q.ToList().ForEach(t =>
					{
						if (types.Length > 0)
						{
							types += "\n" + (t.Name);
						}
						else
						{
							types += (t.Name);
						}
					});

					if (!string.IsNullOrEmpty(types))
					{
						result = types;
						allSimpleObscuredTypes = types;
					}
					else
					{
						allSimpleObscuredTypes = result;
					}
				}
			}
			else
			{
				result = allSimpleObscuredTypes;
			}
#endif
			return result;
		}

#region prefs stuff
		private string GetAllObscuredPrefsDataTypes()
		{
            return "int\n" +
				   "float\n" +
				   "string\n" +
				   "<color=\"#75C4EB\">" +
                   "uint\n" +
			       "double\n" +
			       "decimal\n" +
			       "long\n" +
			       "ulong\n" +
			       "bool\n" +
			       "byte[]\n" +
			       "Vector2\n" +
			       "Vector3\n" +
			       "Quaternion\n" +
			       "Color\n" +
			       "Rect" + 
				   "</color>";
		}

		private void LoadRegularPrefs()
		{
			regularPrefs = "int: " + PlayerPrefs.GetInt(PREFS_INT, -1) + "\n";
			regularPrefs += "float: " + PlayerPrefs.GetFloat(PREFS_FLOAT, -1) + "\n";
			regularPrefs += "string: " + PlayerPrefs.GetString(PREFS_STRING, "No saved PlayerPrefs!");
		}

		private void SaveRegularPrefs()
		{
			PlayerPrefs.SetInt(PREFS_INT, 456);
			PlayerPrefs.SetFloat(PREFS_FLOAT, 456.789f);
			PlayerPrefs.SetString(PREFS_STRING, "Hey, there!");
			PlayerPrefs.Save();
		}

		private void DeleteRegularPrefs()
		{
			PlayerPrefs.DeleteKey(PREFS_INT);
			PlayerPrefs.DeleteKey(PREFS_FLOAT);
			PlayerPrefs.DeleteKey(PREFS_STRING);
			PlayerPrefs.Save();
		}

		private void LoadObscuredPrefs()
		{
			var ba = ObscuredPrefs.GetByteArray(PREFS_BYTE_ARRAY, 0, 4);

			obscuredPrefs = "int: " + ObscuredPrefs.GetInt(PREFS_INT, -1) + "\n";
			obscuredPrefs += "float: " + ObscuredPrefs.GetFloat(PREFS_FLOAT, -1) + "\n";
			obscuredPrefs += "string: " + ObscuredPrefs.GetString(PREFS_STRING, "No saved ObscuredPrefs!") + "\n";
			obscuredPrefs += "bool: " + ObscuredPrefs.GetBool(PREFS_BOOL, false) + "\n";
			obscuredPrefs += "uint: " + ObscuredPrefs.GetUInt(PREFS_UINT, 0) + "\n";
			obscuredPrefs += "long: " + ObscuredPrefs.GetLong(PREFS_LONG, -1) + "\n";
			obscuredPrefs += "double: " + ObscuredPrefs.GetDouble(PREFS_DOUBLE, -1) + "\n";
			obscuredPrefs += "Vector2: " + ObscuredPrefs.GetVector2(PREFS_VECTOR2, Vector2.zero) + "\n";
			obscuredPrefs += "Vector3: " + ObscuredPrefs.GetVector3(PREFS_VECTOR3, Vector3.zero) + "\n";
			obscuredPrefs += "Quaternion: " + ObscuredPrefs.GetQuaternion(PREFS_QUATERNION, Quaternion.identity) + "\n";
			obscuredPrefs += "Rect: " + ObscuredPrefs.GetRect(PREFS_RECT, new Rect(0,0,0,0)) + "\n";
			obscuredPrefs += "Color: " + ObscuredPrefs.GetColor(PREFS_COLOR, Color.black) + "\n";
			obscuredPrefs += "byte[]: {" + ba[0] + "," + ba[1] + "," + ba[2] + "," + ba[3] + "}";
		}

		private void SaveObscuredPrefs()
		{
			// same types as at the regular PlayerPrefs
			ObscuredPrefs.SetInt(PREFS_INT, 123);
			ObscuredPrefs.SetFloat(PREFS_FLOAT, 123.456f);
			ObscuredPrefs.SetString(PREFS_STRING, "Goscurry is not a lie ;)");

			// additional types
			ObscuredPrefs.SetBool(PREFS_BOOL, true);
			ObscuredPrefs.SetUInt(PREFS_UINT, 1234567891u);
			ObscuredPrefs.SetLong(PREFS_LONG, 1234567891234567890L);
			ObscuredPrefs.SetDouble(PREFS_DOUBLE, 1.234567890123456d);
			ObscuredPrefs.SetVector2(PREFS_VECTOR2, Vector2.one);
			ObscuredPrefs.SetVector3(PREFS_VECTOR3, Vector3.one);
			ObscuredPrefs.SetQuaternion(PREFS_QUATERNION, Quaternion.Euler(new Vector3(10,20,30)));
			ObscuredPrefs.SetRect(PREFS_RECT, new Rect(1.5f,2.6f,3.7f,4.8f));
			ObscuredPrefs.SetColor(PREFS_COLOR, Color.red);
			ObscuredPrefs.SetByteArray(PREFS_BYTE_ARRAY, new byte[] { 44, 104, 43, 32 });
			ObscuredPrefs.Save();
		}

		private void DeleteObscuredPrefs()
		{
			ObscuredPrefs.DeleteKey(PREFS_INT);
			ObscuredPrefs.DeleteKey(PREFS_FLOAT);
			ObscuredPrefs.DeleteKey(PREFS_STRING);
			ObscuredPrefs.DeleteKey(PREFS_BOOL);
			ObscuredPrefs.DeleteKey(PREFS_UINT);
			ObscuredPrefs.DeleteKey(PREFS_LONG);
			ObscuredPrefs.DeleteKey(PREFS_DOUBLE);
			ObscuredPrefs.DeleteKey(PREFS_VECTOR2);
			ObscuredPrefs.DeleteKey(PREFS_VECTOR3);
			ObscuredPrefs.DeleteKey(PREFS_QUATERNION);
			ObscuredPrefs.DeleteKey(PREFS_RECT);
			ObscuredPrefs.DeleteKey(PREFS_COLOR);
			ObscuredPrefs.DeleteKey(PREFS_BYTE_ARRAY);
			ObscuredPrefs.Save();
		}
#endregion

		private void OnApplicationQuit()
		{
			DeleteRegularPrefs();
			DeleteObscuredPrefs();
		}
    }

	internal class HorizontalLayout : IDisposable
	{
		public HorizontalLayout(params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal(options);
		}

		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}

	internal class VerticalLayout : IDisposable
	{
		public VerticalLayout(params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical(options);
		}

		public VerticalLayout(GUIStyle style)
		{
			GUILayout.BeginVertical(style);
		}

		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}
}