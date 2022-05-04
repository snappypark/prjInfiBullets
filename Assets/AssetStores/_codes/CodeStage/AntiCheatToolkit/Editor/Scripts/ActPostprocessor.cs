#if UNITY_EDITOR
#define ACTK_DEBUG
#undef ACTK_DEBUG

#define ACTK_DEBUG_VERBOSE
#undef ACTK_DEBUG_VERBOSE

#define ACTK_DEBUG_PARANIOD
#undef ACTK_DEBUG_PARANIOD

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
#if UNITY_2017_1_OR_NEWER
using UnityEditor.Build;
#endif
using UnityEditor.Callbacks;
using Debug = UnityEngine.Debug;

#if (ACTK_DEBUG || ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
using System.Diagnostics;
#endif

namespace CodeStage.AntiCheat.EditorCode
{
#if UNITY_2017_1_OR_NEWER
	internal class ActPostprocessor : AssetPostprocessor, IActiveBuildTargetChanged
#else
	internal class ActPostprocessor : AssetPostprocessor
#endif
	{
		private static readonly List<AllowedAssembly> allowedAssemblies = new List<AllowedAssembly>();
		private static readonly List<string> allLibraries = new List<string>();

#if (ACTK_DEBUG || ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
		[UnityEditor.MenuItem("Anti-Cheat Toolkit/Force Injection Detector data collection")]
		private static void CallInjectionScan()
		{
			InjectionAssembliesScan(true); 
		}
#endif

		// called by Unity
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			if (!EditorPrefs.GetBool(ActEditorGlobalStuff.PREFS_INJECTION_ENABLED)) return;
			if (!IsInjectionDetectorTargetCompatible())
			{
				InjectionDetectorTargetCompatibleCheck();
				return;
			}

			if (deletedAssets.Length > 0)
			{
				foreach (var deletedAsset in deletedAssets)
				{
					if (deletedAsset.IndexOf(ActEditorGlobalStuff.INJECTION_DATA_FILE, StringComparison.Ordinal) > -1 && !EditorApplication.isCompiling)
					{
#if (ACTK_DEBUG || ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
						Debug.LogWarning("Looks like Injection Detector data file was accidentally removed! Re-creating...\nIf you wish to remove " + ActEditorGlobalStuff.INJECTION_DATA_FILE + " file, just disable Injection Detecotr in the ACTk Settings window.");
#endif
						InjectionAssembliesScan();
					}
				}
			}
		}

		// called by Unity
		[DidReloadScripts]
		private static void ScriptsWereReloaded()
		{
#if !UNITY_2017_1_OR_NEWER
			EditorUserBuildSettings.activeBuildTargetChanged += OnBuildTargetChanged;
#endif

			if (EditorPrefs.GetBool(ActEditorGlobalStuff.PREFS_INJECTION_ENABLED))
			{
				InjectionAssembliesScan();
			}
		}

		internal static void InjectionAssembliesScan()
		{
			InjectionAssembliesScan(false);
		}

		internal static void InjectionAssembliesScan(bool forced)
		{
			if (!IsInjectionDetectorTargetCompatible() && !forced)
			{
				InjectionDetectorTargetCompatibleCheck();
				return;
			}

#if (ACTK_DEBUG || ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
			Stopwatch sw = Stopwatch.StartNew();
#if (ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Injection Detector Assemblies Scan\n");
			Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Paths:\n" +

			          "Assets: " + ActEditorGlobalStuff.ASSETS_PATH + "\n" +
			          "Assemblies: " + ActEditorGlobalStuff.ASSEMBLIES_PATH + "\n" +
			          "Injection Detector Data: " + ActEditorGlobalStuff.INJECTION_DATA_PATH);
			sw.Start();
#endif
#endif

#if (ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Looking for all assemblies in current project...");
			sw.Start();
#endif
			allLibraries.Clear();
			allowedAssemblies.Clear();

			allLibraries.AddRange(ActEditorGlobalStuff.FindLibrariesAt(ActEditorGlobalStuff.assetsPath));
			allLibraries.AddRange(ActEditorGlobalStuff.FindLibrariesAt(ActEditorGlobalStuff.assembliesPath));
#if (ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Total libraries found: " + allLibraries.Count);
			sw.Start();
#endif
			const string editorSubdir = "/editor/";
			var assembliesPathLowerCase = ActEditorGlobalStuff.ASSEMBLIES_PATH_RELATIVE.ToLower();
			foreach (var libraryPath in allLibraries)
			{
				var libraryPathLowerCase = libraryPath.ToLower();
#if (ACTK_DEBUG_PARANIOD)
				sw.Stop();
				Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Checking library at the path: " + libraryPathLowerCase);
				sw.Start();
#endif
				if (libraryPathLowerCase.Contains(editorSubdir)) continue;
				if (libraryPathLowerCase.Contains("-editor.dll") && libraryPathLowerCase.Contains(assembliesPathLowerCase)) continue;

				try
				{
					var assName = AssemblyName.GetAssemblyName(libraryPath);
					var name = assName.Name;
					var hash = ActEditorGlobalStuff.GetAssemblyHash(assName);

					var allowed = allowedAssemblies.FirstOrDefault(allowedAssembly => allowedAssembly.name == name);

					if (allowed != null)
					{
						allowed.AddHash(hash);
					}
					else
					{
						allowed = new AllowedAssembly(name, new[] {hash});
						allowedAssemblies.Add(allowed);
					}
				}
				catch
				{
					// not a valid IL assembly, skipping
				}
			}

#if (ACTK_DEBUG || ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
			sw.Stop();
			string trace = ActEditorGlobalStuff.LOG_PREFIX + "Found assemblies (" + allowedAssemblies.Count + "):\n";

			foreach (AllowedAssembly allowedAssembly in allowedAssemblies)
			{
				trace += "  Name: " + allowedAssembly.name + "\n";
				trace = allowedAssembly.hashes.Aggregate(trace, (current, hash) => current + ("    Hash: " + hash + "\n"));
			}

			Debug.Log(trace);
			sw.Start();
#endif
			if (!Directory.Exists(ActEditorGlobalStuff.resourcesPath))
			{
#if (ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
				sw.Stop();
				Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Creating resources folder: " + ActEditorGlobalStuff.RESOURCES_PATH);
				sw.Start();
#endif
				Directory.CreateDirectory(ActEditorGlobalStuff.resourcesPath);
			}

			ActEditorGlobalStuff.RemoveReadOnlyAttribute(ActEditorGlobalStuff.injectionDataPath);
			var bw = new BinaryWriter(new FileStream(ActEditorGlobalStuff.injectionDataPath, FileMode.Create, FileAccess.Write, FileShare.Read));
			var allowedAssembliesCount = allowedAssemblies.Count;

			int totalWhitelistedAssemblies;

#if (ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Processing default whitelist");
			sw.Start();
#endif

			var defaultWhitelistPath = ActEditorGlobalStuff.ResolveInjectionDefaultWhitelistPath();
			if (File.Exists(defaultWhitelistPath))
			{
				var br = new BinaryReader(new FileStream(defaultWhitelistPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
				var assembliesCount = br.ReadInt32();
				totalWhitelistedAssemblies = assembliesCount + allowedAssembliesCount;

				bw.Write(totalWhitelistedAssemblies);

				for (var i = 0; i < assembliesCount; i++)
				{
					bw.Write(br.ReadString());
				}
				br.Close();
			}
			else
			{
#if (ACTK_DEBUG || ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
				sw.Stop();
#endif
				bw.Close();
				Debug.LogError(ActEditorGlobalStuff.LOG_PREFIX + "Can't find " + ActEditorGlobalStuff.INJECTION_DEFAULT_WHITELIST_FILE + " file!\nPlease, report to " + ActEditorGlobalStuff.REPORT_EMAIL);
				return;
			}

#if (ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Processing user whitelist");
			sw.Start();
#endif

			var userWhitelistPath = ActEditorGlobalStuff.ResolveInjectionUserWhitelistPath();
			if (File.Exists(userWhitelistPath))
			{
				var br = new BinaryReader(new FileStream(userWhitelistPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
				var assembliesCount = br.ReadInt32();

				bw.Seek(0, SeekOrigin.Begin);
				bw.Write(totalWhitelistedAssemblies + assembliesCount);
				bw.Seek(0, SeekOrigin.End);
				for (var i = 0; i < assembliesCount; i++)
				{
					bw.Write(br.ReadString());
				}
				br.Close();
			}

#if (ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Processing project assemblies");
			sw.Start();
#endif

			for (var i = 0; i < allowedAssembliesCount; i++)
			{
				var assembly = allowedAssemblies[i];
				var name = assembly.name;
				var hashes = "";

				for (var j = 0; j < assembly.hashes.Length; j++)
				{
					hashes += assembly.hashes[j];
					if (j < assembly.hashes.Length - 1)
					{
						hashes += ActEditorGlobalStuff.INJECTION_DATA_SEPARATOR;
					}
				}

				var line = ObscuredString.EncryptDecrypt(name + ActEditorGlobalStuff.INJECTION_DATA_SEPARATOR + hashes, "Elina");

#if (ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
				Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Writing assembly:\n" + name + ActEditorGlobalStuff.INJECTION_DATA_SEPARATOR + hashes);
#endif
				bw.Write(line);
			}

			bw.Close();
#if (ACTK_DEBUG || ACTK_DEBUG_VERBOSE || ACTK_DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Assemblies scan duration: " + sw.ElapsedMilliseconds + " ms.");
#endif

			if (allowedAssembliesCount == 0)
			{
				Debug.LogError(ActEditorGlobalStuff.LOG_PREFIX + "Can't find any assemblies!\nPlease, report to " + ActEditorGlobalStuff.REPORT_EMAIL);
			}

			AssetDatabase.Refresh();
			//EditorApplication.UnlockReloadAssemblies();
		}

		public static bool IsInjectionDetectorTargetCompatible()
		{
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_ANDROID
			return true;
#else
			return false;
#endif
		}

		private static void InjectionDetectorTargetCompatibleCheck()
		{
			if (!IsInjectionDetectorTargetCompatible())
			{
				if (!File.Exists(ActEditorGlobalStuff.injectionDataPath)) return;
				Debug.LogWarning(ActEditorGlobalStuff.LOG_PREFIX + "Injection Detector is not available on selected platform (" + EditorUserBuildSettings.activeBuildTarget + ") and will be disabled!");
				ActEditorGlobalStuff.CleanInjectionDetectorData();
			}
		}

#if UNITY_2017_1_OR_NEWER
		public int callbackOrder { get { return 0; } }
		public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
		{
			InjectionDetectorTargetCompatibleCheck();
		}
#else
		private static void OnBuildTargetChanged()
		{
			InjectionDetectorTargetCompatibleCheck();
		}
#endif

	}
}
#endif