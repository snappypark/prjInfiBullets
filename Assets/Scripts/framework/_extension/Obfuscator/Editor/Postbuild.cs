/*
 * Copyright (c) 2015-2016 Beebyte Limited. All rights reserved.
 */
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;

namespace Beebyte.Obfuscator
{
	public class Postbuild
	{
		private static Options options = null;
		private static readonly IDictionary<string, string> dllsToRevert = new Dictionary<string, string>();

		private static bool obfuscatedAfterScene = false;
		private static bool noCSharpScripts = false;
		private static bool monoBehaviourAssetsNeedReverting = false;
		private static bool dllsNeedRestoring = false;

		[InitializeOnLoad]
		public static class PostbuildStatic
		{
			/*
			 * Often Unity's EditorApplication.update delegate is reset. Because it's so important to restore
			 * renamed MonoBehaviour assets we assign here where it will be called after scripts are compiled.
			 */ 
			static PostbuildStatic()
			{
				EditorApplication.update += RestoreAssets;
				EditorApplication.update += RestoreOriginalDlls;
			}
		}

		[PostProcessBuild(1)]
		private static void PostBuildHook(UnityEditor.BuildTarget buildTarget, string pathToBuildProject)
		{
			if (obfuscatedAfterScene == false)
			{
				if (noCSharpScripts) Debug.LogWarning("No obfuscation required because no C# scripts were found");
				else Debug.LogError("Failed to obfuscate");
			}
			else
			{
				if (monoBehaviourAssetsNeedReverting) RestoreAssets();
				if (dllsNeedRestoring) RestoreOriginalDlls();
			}
			Clear();
		}

		private static void Clear()
		{
			obfuscatedAfterScene = false;
			noCSharpScripts = false;

			if (options != null && options.obfuscateMonoBehaviourClassNames == false) Obfuscator.Clear();
		}

		/**
		 * When multiple DLLs are obfuscated, usually the extra DLLs need to be reverted back to their original state.
		 * This method backs up the DLLs to be reverted after obfuscation is complete (or failed).
		 */
		private static void backupDlls(ICollection<string> locations)
		{
			if (locations.Count > 0)
			{
				EditorApplication.update += RestoreOriginalDlls;
				dllsNeedRestoring = true;
			}

			foreach (string location in locations)
			{
				string backupLocation = location + ".pre";

				//This throws an exception if the backup already exists - we want this to happen
				System.IO.File.Copy(location, backupLocation);

				dllsToRevert.Add(backupLocation, location);
			}
		}

		[PostProcessScene(1)]
		public static void Obfuscate()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode && !obfuscatedAfterScene)
			{
				try
				{
					EditorApplication.LockReloadAssemblies();

					HashSet<string> dlls = new HashSet<string>();

					foreach (string assemblyName in Config.permanentDLLs)
					{
						dlls.Add(FindDllLocation(assemblyName));
					}
					backupDlls(dlls);

					foreach (string assemblyName in Config.temporaryDLLs)
					{
						dlls.Add(FindDllLocation(assemblyName));
					}

					if (dlls.Count == 0) noCSharpScripts = true;
					else
					{
						if (options == null) options = OptionsManager.LoadOptions();

						Obfuscator.FixHexBug(options);

						if (options.enabled)
						{
							Obfuscator.SetExtraAssemblyDirectories(Config.extraAssemblyDirectories);
							Obfuscator.Obfuscate(dlls, options, EditorUserBuildSettings.activeBuildTarget);

							if (options.obfuscateMonoBehaviourClassNames)
							{
								/*
								 * RestoreAssets must be called via the update delegate because [PostProcessBuild] is not guaranteed to be called
								 */
								EditorApplication.update += RestoreAssets;
								monoBehaviourAssetsNeedReverting = true;
							}
						}
						obfuscatedAfterScene = true;
					}
				}
				finally
				{
					EditorApplication.UnlockReloadAssemblies();
				}
			}
		}

		private static string FindDllLocation(string suffix)
		{
			foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					if (assembly.Location == null || assembly.Location.Equals(string.Empty))
					{
						DisplayFailedAssemblyParseWarning(assembly);
					}
					else if (assembly.Location.EndsWith(suffix))
					{
						return assembly.Location;
					}
				}
				catch (System.NotSupportedException)
				{
					DisplayFailedAssemblyParseWarning(assembly);
				}
			}
			return null;
		}

		private static void DisplayFailedAssemblyParseWarning(System.Reflection.Assembly assembly)
		{
			Debug.LogWarning("Could not parse dynamically created assembly (string.Empty location) " + assembly.FullName + ". If you extend classes from within this assembly that in turn extend from MonoBehaviour you will need to manually annotate these classes with [Skip]");
		}

		/**
		 * This method restores obfuscated MonoBehaviour cs files to their original names.
		 */
		private static void RestoreAssets()
		{
#if UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
#else
			if (BuildPipeline.isBuildingPlayer == false)
			{
#endif
				try
				{
					EditorApplication.LockReloadAssemblies();
					Obfuscator.RevertAssetObfuscation();
					monoBehaviourAssetsNeedReverting = false;
					EditorApplication.update -= RestoreAssets;
				}
				finally
				{
					EditorApplication.UnlockReloadAssemblies();
				}
#if UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
#else
			}
#endif
			Obfuscator.Clear();
		}

		private static void deleteObfuscatedDll(string target)
		{
			int attempts = 60;
			while (attempts > 0)
			{
				try
				{
					attemptToDeleteObfuscatedDll(target);
					if (attempts < 60) Debug.LogWarning("Successfully accessed " + target);
					return;
				}
				catch (Exception e)
				{
					Debug.LogWarning("Failed to access " + target + " - Retrying...");
					Thread.Sleep(500);
					if (--attempts <= 0) throw e;
				}
			}
		}

		private static void attemptToDeleteObfuscatedDll(string target)
		{
			if (File.Exists(target)) File.Delete(target);
		}
		/**
		 * This method restores original Dlls back into the project.
		 * DLLs declared within permanentDLLs will be restored from this method.
		 */
		private static void RestoreOriginalDlls()
		{
#if UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
#else
			if (BuildPipeline.isBuildingPlayer == false)
			{
#endif
				foreach (string location in dllsToRevert.Keys)
				{
					try
					{
						if (File.Exists(location))
						{
							string target = dllsToRevert[location];

							deleteObfuscatedDll(target);

							File.Move(location, dllsToRevert[location]);
						}
					}
					catch (Exception e)
					{
						Debug.LogError("Could not restore original DLL to " + dllsToRevert[location] + "\n" + e);
					}
				}
				dllsToRevert.Clear();
				EditorApplication.update -= RestoreOriginalDlls;
				dllsNeedRestoring = false;
#if UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
#else
			}
#endif
		}
	}
}
