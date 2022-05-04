using UnityEngine;
using UnityEditor;

namespace Beebyte.Obfuscator
{
	public class OptionsManager
	{
		public static readonly string importAssetName = "ObfuscatorOptionsImport";
		public static readonly string optionsAssetName = "ObfuscatorOptions";

		public static readonly string defaultImportPath = @"Assets/Editor/Beebyte/Obfuscator/ObfuscatorOptionsImport.asset";
		public static readonly string defaultOptionsPath = @"Assets/Editor/Beebyte/Obfuscator/ObfuscatorOptions.asset";

		public static Options LoadOptions()
		{
			if (HasInstallFiles()) Install();

			Options o = LoadAsset(optionsAssetName);

			if (o == null)
			{
				Debug.LogError("Failed to load " + optionsAssetName + " asset at " + defaultOptionsPath);
				return null;
			}
			else return o;
		}

		private static bool HasInstallFiles()
		{
			return LoadAsset(importAssetName) != null;
		}

		private static Options LoadAsset(string name)
		{
			string path = GetAssetPath(name);

			return LoadAssetAtPath(path);
		}

		private static void Install()
		{
			Options importOptions = LoadAsset(importAssetName);
			if (importOptions == null)
			{
				Debug.LogError("Could not find " + importAssetName + ".asset - aborting installation.");
				return;
			}

			string importPath = GetAssetPath(importAssetName);
			string newOptionsPath = GetInstallPathFromImport(importPath);

			Options o = LoadAssetAtPath(newOptionsPath);

			if (o != null)
			{
				bool overwrite = EditorUtility.DisplayDialog("Obfuscator Installation", "ObfuscatorOptions already exists, would you like to replace it with new default options?", "Use new defaults", "Keep existing settings");
				if (overwrite) AssetDatabase.MoveAssetToTrash(newOptionsPath);
				else
				{
					AssetDatabase.MoveAssetToTrash(importPath);
					return;
				}
			}

			//Copy & Delete instead of Move, otherwise future installs think that ObfuscatorOptions is actually ObfuscatorOptionsImport
			AssetDatabase.CopyAsset(importPath, newOptionsPath);
			AssetDatabase.DeleteAsset(importPath);
			AssetDatabase.Refresh();
		}

		private static string GetDefaultPath(string assetName)
		{
			if (importAssetName.Equals(assetName)) return defaultImportPath;
			else if (optionsAssetName.Equals(assetName)) return defaultOptionsPath;
			else return null;
		}

#if UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1
		private static string GetAssetPath(string name)
		{
			return GetDefaultPath(name);
		}

		private static Options LoadAssetAtPath(string path)
		{
			return (Options)Resources.LoadAssetAtPath(path, typeof(Options));
		}
#else
		private static string GetAssetPath(string name)
		{
			string GUID = SearchForAssetGUID(name);

			if (GUID != null) return AssetDatabase.GUIDToAssetPath(GUID);
			else return null;
		}

		private static Options LoadAssetAtPath(string path)
		{
			return AssetDatabase.LoadAssetAtPath<Options>(path);
		}

		private static string SearchForAssetGUID(string assetName)
		{
			string[] optionPaths = AssetDatabase.FindAssets(assetName);

			if (optionPaths.Length == 0) return null;
			else if (optionPaths.Length == 1) return optionPaths[0];
			else
			{
				Debug.LogError("Multiple " + assetName + " assets found! Aborting");
				return null;
			}
		}
#endif

		private static string GetInstallPathFromImport(string importPath)
		{
			return importPath.Replace(importAssetName + ".asset", optionsAssetName + ".asset");
		}
	}
}
