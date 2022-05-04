#if UNITY_EDITOR
using System;
using System.IO;
using CodeStage.AntiCheat.Common;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.Windows
{
	internal class ActSettings: EditorWindow
	{
		private class SymbolsData
		{
			public const string ACTK_INJECTION_DEBUG = "ACTK_INJECTION_DEBUG";
			public const string ACTK_INJECTION_DEBUG_VERBOSE = "ACTK_INJECTION_DEBUG_VERBOSE";
			public const string ACTK_INJECTION_DEBUG_PARANOID = "ACTK_INJECTION_DEBUG_PARANOID";
			public const string ACTK_WALLHACK_DEBUG = "ACTK_WALLHACK_DEBUG";
			public const string ACTK_EXCLUDE_OBFUSCATION = "ACTK_EXCLUDE_OBFUSCATION";
			public const string ACTK_PREVENT_READ_PHONE_STATE = "ACTK_PREVENT_READ_PHONE_STATE";
			public const string ACTK_PREVENT_INTERNET_PERMISSION = "ACTK_PREVENT_INTERNET_PERMISSION";

			public bool injectionDebug;
			public bool injectionDebugVerbose;
			public bool injectionDebugParanoid;
			public bool wallhackDebug;
			public bool excludeObfuscation;
			public bool preventReadPhoneState;
			public bool preventInternetPermission;
		}

		private const string WIREFRAME_SHADER_NAME = "Hidden/ACTk/WallHackTexture";

		private static SerializedObject graphicsSettingsAsset;
		private static SerializedProperty includedShaders;

		private SymbolsData symbolsData;

		[UnityEditor.MenuItem(ActEditorGlobalStuff.WINDOWS_MENU_PATH + "Settings...", false, 100)]
		internal static void ShowWindow()
		{
			var myself = GetWindow<ActSettings>(false, "ACTk Settings", true);
			myself.minSize = new Vector2(500, 450);
		}

		private void OnGUI()
		{
			GUILayout.Label("You're using Anti-Cheat Toolkit v." + ACTkConstants.VERSION, ActEditorGUI.LargeBoldLabel);
			EditorGUILayout.Space();
			GUILayout.Label("Injection Detector settings (global)", ActEditorGUI.LargeBoldLabel);

			var enableInjectionDetector = EditorPrefs.GetBool(ActEditorGlobalStuff.PREFS_INJECTION_ENABLED);

			EditorGUI.BeginChangeCheck();
			enableInjectionDetector = GUILayout.Toggle(enableInjectionDetector, "Enable Injection Detector");
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool(ActEditorGlobalStuff.PREFS_INJECTION_ENABLED, enableInjectionDetector);
				if (enableInjectionDetector && !ActPostprocessor.IsInjectionDetectorTargetCompatible())
				{
					Debug.LogWarning(ActEditorGlobalStuff.LOG_PREFIX + "Injection Detector is not available on selected platform (" +
					                 EditorUserBuildSettings.activeBuildTarget + ")");
				}

				if (!enableInjectionDetector)
				{
					ActEditorGlobalStuff.CleanInjectionDetectorData();
				}
				else if (!File.Exists(ActEditorGlobalStuff.injectionDataPath))
				{
					ActPostprocessor.InjectionAssembliesScan();
				}
			}

			if (GUILayout.Button("Edit Whitelist"))
			{
				ActAssembliesWhitelist.ShowWindow();
			}

			EditorGUILayout.Space();
			GUILayout.Label("WallHack Detector settings (per-project)", ActEditorGUI.LargeBoldLabel);
			GUILayout.Label(
				"Wireframe module uses specific shader under the hood. Thus such shader should be included into the build to exist at runtime. To make sure it's get included, you may add it to the Always Included Shaders list using buttons below. You don't need to include it if you're not going to use Wireframe module.",
				EditorStyles.wordWrappedLabel);

			ReadGraphicsAsset();

			if (graphicsSettingsAsset != null && includedShaders != null)
			{
				// outputs whole included shaders list, use for debug
				//EditorGUILayout.PropertyField(includedShaders, true);

				var shaderIndex = GetWallhackDetectorShaderIndex();

				EditorGUI.BeginChangeCheck();

				if (shaderIndex != -1)
				{
					GUILayout.Label("Shader already exists in the Always Included Shaders list, you're good to go!",
						EditorStyles.wordWrappedLabel);
					if (GUILayout.Button("Remove shader"))
					{
						includedShaders.DeleteArrayElementAtIndex(shaderIndex);
						includedShaders.DeleteArrayElementAtIndex(shaderIndex);
					}
				}
				else
				{
					GUILayout.Label("Shader doesn't exists in the Always Included Shaders list.", EditorStyles.wordWrappedLabel);
					if (GUILayout.Button("Include shader"))
					{
						var shader = Shader.Find(WIREFRAME_SHADER_NAME);
						if (shader != null)
						{
							includedShaders.InsertArrayElementAtIndex(includedShaders.arraySize);
							var newItem = includedShaders.GetArrayElementAtIndex(includedShaders.arraySize - 1);
							newItem.objectReferenceValue = shader;
						}
						else
						{
							Debug.LogError("Can't find " + WIREFRAME_SHADER_NAME + " shader! Please report this to the  " +
							               ActEditorGlobalStuff.REPORT_EMAIL + " including your Unity version number.");
						}
					}
					if (GUILayout.Button("Open Graphics Settings to manage it manually (see readme.pdf for details)"))
					{
						EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
					}
				}

				if (EditorGUI.EndChangeCheck())
				{
					graphicsSettingsAsset.ApplyModifiedProperties();
				}
			}
			else
			{
				GUILayout.Label("Can't automatically control " + WIREFRAME_SHADER_NAME +
				                " shader existence at the Always Included Shaders list. Please, manage this manually in Graphics Settings.");
				if (GUILayout.Button("Open Graphics Settings"))
				{
					EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
				}
			}

			EditorGUILayout.Space();
			GUILayout.Label("Compilation symbols (per-project)", ActEditorGUI.LargeBoldLabel);
			GUILayout.Label("Here you may switch conditional compilation symbols used in ACTk.\n" +
			                "Check Readme for more details on each symbol.", EditorStyles.wordWrappedLabel);
			EditorGUILayout.Space();
			if (symbolsData == null)
			{
				symbolsData = GetSymbolsData();
			}

			/*if (GUILayout.Button("Reset"))
			{
				var groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
				foreach (BuildTargetGroup buildTargetGroup in groups)
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Empty);
				}
			}*/

			EditorGUI.BeginChangeCheck();
			symbolsData.injectionDebug = GUILayout.Toggle(symbolsData.injectionDebug, new GUIContent(SymbolsData.ACTK_INJECTION_DEBUG, "Switches the Injection Detector debug."));
			if (EditorGUI.EndChangeCheck())
			{
				if (symbolsData.injectionDebug)
				{
					SetSymbol(SymbolsData.ACTK_INJECTION_DEBUG);
				}
				else
				{
					RemoveSymbol(SymbolsData.ACTK_INJECTION_DEBUG);
				}

				symbolsData = GetSymbolsData();
			}

			EditorGUI.BeginChangeCheck();
			symbolsData.injectionDebugVerbose = GUILayout.Toggle(symbolsData.injectionDebugVerbose, new GUIContent(SymbolsData.ACTK_INJECTION_DEBUG_VERBOSE, "Switches the Injection Detector verbose debug level."));
			if (EditorGUI.EndChangeCheck())
			{
				if (symbolsData.injectionDebugVerbose)
				{
					SetSymbol(SymbolsData.ACTK_INJECTION_DEBUG_VERBOSE);
				}
				else
				{
					RemoveSymbol(SymbolsData.ACTK_INJECTION_DEBUG_VERBOSE);
				}

				symbolsData = GetSymbolsData();
			}

			EditorGUI.BeginChangeCheck();
			symbolsData.injectionDebugParanoid = GUILayout.Toggle(symbolsData.injectionDebugParanoid, new GUIContent(SymbolsData.ACTK_INJECTION_DEBUG_PARANOID, "Switches the Injection Detector paraniod debug level."));
			if (EditorGUI.EndChangeCheck())
			{
				if (symbolsData.injectionDebugParanoid)
				{
					SetSymbol(SymbolsData.ACTK_INJECTION_DEBUG_PARANOID);
				}
				else
				{
					RemoveSymbol(SymbolsData.ACTK_INJECTION_DEBUG_PARANOID);
				}

				symbolsData = GetSymbolsData();
			}

			EditorGUI.BeginChangeCheck();
			symbolsData.wallhackDebug = GUILayout.Toggle(symbolsData.wallhackDebug, new GUIContent(SymbolsData.ACTK_WALLHACK_DEBUG, "Switches the WallHack Detector debug - you'll see the WallHack objects in scene and get extra information in console."));
			if (EditorGUI.EndChangeCheck())
			{
				if (symbolsData.wallhackDebug)
				{
					SetSymbol(SymbolsData.ACTK_WALLHACK_DEBUG);
				}
				else
				{
					RemoveSymbol(SymbolsData.ACTK_WALLHACK_DEBUG);
				}

				symbolsData = GetSymbolsData();
			}

			EditorGUI.BeginChangeCheck();
			symbolsData.excludeObfuscation = GUILayout.Toggle(symbolsData.excludeObfuscation, new GUIContent(SymbolsData.ACTK_EXCLUDE_OBFUSCATION, "Enable if you use Unity-unaware obfuscators which support ObfuscationAttribute to help avoid names corruption."));
			if (EditorGUI.EndChangeCheck())
			{
				if (symbolsData.excludeObfuscation)
				{
					SetSymbol(SymbolsData.ACTK_EXCLUDE_OBFUSCATION);
				}
				else
				{
					RemoveSymbol(SymbolsData.ACTK_EXCLUDE_OBFUSCATION);
				}

				symbolsData = GetSymbolsData();
			}

			EditorGUI.BeginChangeCheck();
			symbolsData.preventReadPhoneState = GUILayout.Toggle(symbolsData.preventReadPhoneState, new GUIContent(SymbolsData.ACTK_PREVENT_READ_PHONE_STATE, "Disables ObscuredPrefs Lock To Device functionality."));
			if (EditorGUI.EndChangeCheck())
			{
				if (symbolsData.preventReadPhoneState)
				{
					SetSymbol(SymbolsData.ACTK_PREVENT_READ_PHONE_STATE);
				}
				else
				{
					RemoveSymbol(SymbolsData.ACTK_PREVENT_READ_PHONE_STATE);
				}

				symbolsData = GetSymbolsData();
			}

			EditorGUI.BeginChangeCheck();
			symbolsData.preventInternetPermission = GUILayout.Toggle(symbolsData.preventInternetPermission, new GUIContent(SymbolsData.ACTK_PREVENT_INTERNET_PERMISSION, "Disables TimeCheatingDetector functionality."));
			if (EditorGUI.EndChangeCheck())
			{
				if (symbolsData.preventInternetPermission)
				{
					SetSymbol(SymbolsData.ACTK_PREVENT_INTERNET_PERMISSION);
				}
				else
				{
					RemoveSymbol(SymbolsData.ACTK_PREVENT_INTERNET_PERMISSION);
				}

				symbolsData = GetSymbolsData();
			}
		}

		internal static void ReadGraphicsAsset()
		{
			if (graphicsSettingsAsset != null) return;

			var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset");
			if (assets.Length > 0)
			{
				graphicsSettingsAsset = new SerializedObject(assets[0]);
			}

			if (graphicsSettingsAsset != null)
			{
				includedShaders = graphicsSettingsAsset.FindProperty("m_AlwaysIncludedShaders");
			}
		}

		internal static int GetWallhackDetectorShaderIndex()
		{
			if (graphicsSettingsAsset == null || includedShaders == null) return -1;

			var result = -1;
			graphicsSettingsAsset.Update();

			var itemsCount = includedShaders.arraySize;
			for (var i = 0; i < itemsCount; i++)
			{
				var arrayItem = includedShaders.GetArrayElementAtIndex(i);
				if (arrayItem.objectReferenceValue != null)
				{
					var shader = (Shader)(arrayItem.objectReferenceValue);

					if (shader.name == WIREFRAME_SHADER_NAME)
					{
						result = i;
						break;
					}
				}
			}

			return result;
		}

		internal static bool IsWallhackDetectorShaderIncluded()
		{
			var result = false;

			ReadGraphicsAsset();
			if (GetWallhackDetectorShaderIndex() != -1)
				result = true;

			return result;
		}

		private SymbolsData GetSymbolsData()
		{
			var result = new SymbolsData();

			var groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
			foreach (var buildTargetGroup in groups)
			{
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;

				var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

				result.injectionDebug |= GetSymbol(symbols, SymbolsData.ACTK_INJECTION_DEBUG);
				result.injectionDebugVerbose |= GetSymbol(symbols, SymbolsData.ACTK_INJECTION_DEBUG_VERBOSE);
				result.injectionDebugParanoid |= GetSymbol(symbols, SymbolsData.ACTK_INJECTION_DEBUG_PARANOID);
				result.wallhackDebug |= GetSymbol(symbols, SymbolsData.ACTK_WALLHACK_DEBUG);
				result.excludeObfuscation |= GetSymbol(symbols, SymbolsData.ACTK_EXCLUDE_OBFUSCATION);
				result.preventReadPhoneState |= GetSymbol(symbols, SymbolsData.ACTK_PREVENT_READ_PHONE_STATE);
				result.preventInternetPermission |= GetSymbol(symbols, SymbolsData.ACTK_PREVENT_INTERNET_PERMISSION);
			}

			return result;
		}

		private bool GetSymbol(string symbols, string symbol)
		{
			var result = false;

			if (symbols == symbol)
			{
				result = true;
			}
			else if (symbols.StartsWith(symbol + ';'))
			{
				result = true;
			}
			else if (symbols.EndsWith(';' + symbol))
			{
				result = true;
			}
			else if (symbols.Contains(';' + symbol + ';'))
			{
				result = true;
			}

			return result;
		}

		private void SetSymbol(string symbol)
		{
			var groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
			foreach (var buildTargetGroup in groups)
			{
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;

				var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
				if (symbols.Length == 0)
				{
					symbols = symbol;
				}
				else
				{
					if (symbols.EndsWith(";"))
					{
						symbols += symbol;
					}
					else
					{
						symbols += ';' + symbol;
					}
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
			}
		}

		private void RemoveSymbol(string symbol)
		{
			var groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
			foreach (var buildTargetGroup in groups)
			{
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;

				var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

				if (symbols == symbol)
				{
					symbols = string.Empty;
				}
				else if (symbols.StartsWith(symbol + ';'))
				{
					symbols = symbols.Remove(0, symbol.Length + 1);
				}
				else if (symbols.EndsWith(';' + symbol))
				{
					symbols = symbols.Remove(symbols.LastIndexOf(';' + symbol, StringComparison.Ordinal), symbol.Length + 1);
				}
				else if (symbols.Contains(';' + symbol + ';'))
				{
					symbols = symbols.Replace(';' + symbol + ';', ";");
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
			}
		}
	}
}
#endif