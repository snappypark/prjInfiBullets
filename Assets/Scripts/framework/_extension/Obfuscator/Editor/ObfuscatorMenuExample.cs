/*
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

//Options are read in the same way as normal Obfuscation, i.e. from the ObfuscatorOptions.asset
namespace Beebyte.Obfuscator
{
	public class ObfuscatorMenuExample 
	{
		private static Options options = null;

		[MenuItem("Tools/Obfuscate External DLL")]
		private static void ObfuscateExternalDll()
		{
			string dllPath = @"C:\path\to\External.dll";

			Debug.Log("Obfuscating");

			if (System.IO.File.Exists(dllPath))
			{
				if (options == null) options = OptionsManager.LoadOptions();

				bool oldSkipRenameOfAllPublicMonobehaviourFields = options.skipRenameOfAllPublicMonobehaviourFields;
				try
				{
					 //Preserving monobehaviour public field names is an common step for obfuscating external DLLs that
					 //allow MonoBehaviours to be dragged into the scene's hierarchy.
					options.skipRenameOfAllPublicMonobehaviourFields = true;

					//Consider setting this hidden value to false to allow classes like EditorWindow to be obfuscated.
					//ScriptableObjects would normally be treated as Serializable to avoid breaking loading/saving,
					//but for Editor windows this might not be necessary.
					//options.treatScriptableObjectsAsSerializable = false;

					Beebyte.Obfuscator.Obfuscator.Obfuscate(dllPath, options, EditorUserBuildSettings.activeBuildTarget);
				}
				finally
				{
					options.skipRenameOfAllPublicMonobehaviourFields = oldSkipRenameOfAllPublicMonobehaviourFields;
					EditorUtility.ClearProgressBar();
				}
			}
			else Debug.Log("Obfuscating could not find file at " + dllPath);
		}
	}
}
*/
