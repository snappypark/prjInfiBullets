using CodeStage.AntiCheat.Common;
using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Base class for all detectors.
	/// </summary>
	[AddComponentMenu("")]
	public abstract class ActDetectorBase : MonoBehaviour
	{
		protected const string CONTAINER_NAME = "Anti-Cheat Toolkit Detectors";
		protected const string MENU_PATH = "Code Stage/Anti-Cheat Toolkit/";
		protected const string GAME_OBJECT_MENU_PATH = "GameObject/Create Other/" + MENU_PATH;

		protected static GameObject detectorsContainer;

		/// <summary>
		/// Allows to start detector automatically.
		/// Otherwise, you'll need to call StartDetection() method to start it.
		/// </summary>
		/// Useful in conjunction with proper Detection Event configuration in the inspector.
		/// Allows to use detector without writing any code except the actual reaction on cheating.
		[Tooltip("Automatically start detector. Detection Event will be called on detection.")]
		public bool autoStart = true;

		/// <summary>
		/// Detector will survive new level (scene) load if checked. Otherwise it will be destroyed.
		/// </summary>
		/// On dispose Detector follows 2 rules:
		/// - if Game Object's name is "Anti-Cheat Toolkit Detectors": it will be automatically 
		/// destroyed if no other Detectors left attached regardless of any other components or children;<br/>
		/// - if Game Object's name is NOT "Anti-Cheat Toolkit Detectors": it will be automatically destroyed only
		/// if it has neither other components nor children attached;
		[Tooltip("Detector will survive new level (scene) load if checked.")]
		public bool keepAlive = true;

		/// <summary>
		/// Detector component will be automatically disposed after firing callback if enabled.
		/// Otherwise, it will just stop internal processes.
		/// </summary>
		/// On dispose Detector follows 2 rules:
		/// - if Game Object's name is "Anti-Cheat Toolkit Detectors": it will be automatically 
		/// destroyed if no other Detectors left attached regardless of any other components or children;<br/>
		/// - if Game Object's name is NOT "Anti-Cheat Toolkit Detectors": it will be automatically destroyed only
		/// if it has neither other components nor children attached;
		[Tooltip("Automatically dispose Detector after firing callback.")]
		public bool autoDispose = true;

		[SerializeField]
		protected UnityEvent detectionEvent;
		protected UnityAction detectionAction = null;

		[SerializeField]
		protected bool detectionEventHasListener;

		protected bool started;
		protected bool isRunning;

		/// <summary>
		/// Allows to check if detection is currently running.
		/// </summary>
		public bool IsRunning
		{
			get { return isRunning; }
		}

		#region detectors placement
#if UNITY_EDITOR
		[UnityEditor.MenuItem(GAME_OBJECT_MENU_PATH + "All detectors", false, 0)]
		private static void AddAllDetectorsToScene()
		{
			AddInjectionDetectorToScene();
			AddObscuredCheatingDetectorToScene();
			AddSpeedHackDetectorToScene();
			AddWallHackDetectorToScene();
			AddTimeCheatingDetectorToScene();
		}

		[UnityEditor.MenuItem(GAME_OBJECT_MENU_PATH + InjectionDetector.COMPONENT_NAME, false, 1)]
		private static void AddInjectionDetectorToScene()
		{
			SetupDetectorInScene<InjectionDetector>();
		}

		[UnityEditor.MenuItem(GAME_OBJECT_MENU_PATH + ObscuredCheatingDetector.COMPONENT_NAME, false, 1)]
		private static void AddObscuredCheatingDetectorToScene()
		{
			SetupDetectorInScene<ObscuredCheatingDetector>();
		}

		[UnityEditor.MenuItem(GAME_OBJECT_MENU_PATH + SpeedHackDetector.COMPONENT_NAME, false, 1)]
		private static void AddSpeedHackDetectorToScene()
		{
			SetupDetectorInScene<SpeedHackDetector>();
		}

		[UnityEditor.MenuItem(GAME_OBJECT_MENU_PATH + WallHackDetector.COMPONENT_NAME, false, 1)]
		private static void AddWallHackDetectorToScene()
		{
			SetupDetectorInScene<WallHackDetector>();
		}

		[UnityEditor.MenuItem(GAME_OBJECT_MENU_PATH + TimeCheatingDetector.COMPONENT_NAME, false, 1)]
		private static void AddTimeCheatingDetectorToScene()
		{
			SetupDetectorInScene<TimeCheatingDetector>();
		}

		private static void SetupDetectorInScene<T>() where T : ActDetectorBase
		{
			var component = FindObjectOfType<T>();
			var detectorName = typeof(T).Name;

			if (component != null)
			{
				if (component.gameObject.name == CONTAINER_NAME)
				{
					UnityEditor.EditorUtility.DisplayDialog(detectorName + " already exists!", detectorName + " already exists in scene and correctly placed on object \"" + CONTAINER_NAME + "\"", "OK");
				}
				else
				{
					var dialogResult = UnityEditor.EditorUtility.DisplayDialogComplex(detectorName + " already exists!", detectorName + " already exists in scene and placed on object \"" + component.gameObject.name + "\". Do you wish to move it to the Game Object \"" + CONTAINER_NAME + "\" or delete it from scene at all?", "Move", "Delete", "Cancel");
					switch (dialogResult)
					{
						case 0:
							var container = GameObject.Find(CONTAINER_NAME);
							if (container == null)
							{
								container = new GameObject(CONTAINER_NAME);
							}
							var newComponent = container.AddComponent<T>();
							UnityEditor.EditorUtility.CopySerialized(component, newComponent);
							DestroyDetectorImmediate(component);
							break;
						case 1:
							DestroyDetectorImmediate(component);
							break;
					}
				}
			}
			else
			{
				var container = GameObject.Find(CONTAINER_NAME);
				if (container == null)
				{
					container = new GameObject(CONTAINER_NAME);

					UnityEditor.Undo.RegisterCreatedObjectUndo(container, "Create " + CONTAINER_NAME);
				}
				UnityEditor.Undo.AddComponent<T>(container);

				UnityEditor.EditorUtility.DisplayDialog(detectorName + " added!", detectorName + " successfully added to the object \"" + CONTAINER_NAME + "\"", "OK");
			}
		}

		private static void DestroyDetectorImmediate(ActDetectorBase component)
		{
			if (component.transform.childCount == 0 && component.GetComponentsInChildren<Component>(true).Length <= 2)
			{
				DestroyImmediate(component.gameObject);
			}
			else
			{
				DestroyImmediate(component);
			}
		}
#endif
		#endregion

		#region unity messages
#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void Start()
		{
			if (detectorsContainer == null && gameObject.name == CONTAINER_NAME)
			{
				detectorsContainer = gameObject;
			}

			if (autoStart && !started)
			{
				StartDetectionAutomatically();
			}
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void OnEnable()
		{
			if (!started || (!detectionEventHasListener && detectionAction == null && !DetectorHasAdditionalCallbacks()))
				return;
			ResumeDetector();
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void OnDisable()
		{
			if (!started) return;
			PauseDetector();
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void OnApplicationQuit()
		{
			DisposeInternal();
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		protected virtual void OnDestroy()
		{
			StopDetectionInternal();

			if (transform.childCount == 0 && GetComponentsInChildren<Component>().Length <= 2)
			{
				Destroy(gameObject);
			}
			else if (name == CONTAINER_NAME && GetComponentsInChildren<ActDetectorBase>().Length <= 1)
			{
				Destroy(gameObject);
			}
		}
		#endregion

		protected virtual bool Init(ActDetectorBase instance, string detectorName)
		{
			if (instance != null && instance != this && instance.keepAlive)
			{
				Debug.LogWarning(ACTkConstants.LOG_PREFIX + name + 
					": self-destroying, other instance already exists & only one instance allowed!", gameObject);
				Destroy(this);
				return false;
			}

			DontDestroyOnLoad(gameObject);
			return true;
		}

		protected virtual void DisposeInternal()
		{
			Destroy(this);
		}

		protected virtual bool DetectorHasAdditionalCallbacks()
		{
			return false;
		}

		internal virtual void OnCheatingDetected()
		{
			if (detectionAction != null) detectionAction();
			if (detectionEventHasListener) detectionEvent.Invoke();

			if (autoDispose)
			{
				DisposeInternal();
			}
			else
			{
				StopDetectionInternal();
			}
		}

		protected abstract void StartDetectionAutomatically();
		protected abstract void StopDetectionInternal();
		protected abstract void PauseDetector();
		protected abstract void ResumeDetector();
	}
}