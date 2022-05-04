#if UNITY_EDITOR
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredString))]
	internal class ObscuredStringDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			var hiddenValue = prop.FindPropertyRelative("hiddenValue");
			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			var inited = prop.FindPropertyRelative("inited");
			var fakeValue = prop.FindPropertyRelative("fakeValue");
			var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

			var currentCryptoKey = cryptoKey.stringValue;
			var val = string.Empty;

			if (!inited.boolValue)
			{
				if (string.IsNullOrEmpty(currentCryptoKey))
				{
					currentCryptoKey = cryptoKey.stringValue = ObscuredString.cryptoKeyEditor;
				}
				inited.boolValue = true;
				EncryptAndSetBytes(val, hiddenValue, currentCryptoKey);
				fakeValue.stringValue = val;
			}
			else
			{
				var size = hiddenValue.FindPropertyRelative("Array.size");
				var showMixed = size.hasMultipleDifferentValues;

				if (!showMixed)
				{
					for (var i = 0; i < hiddenValue.arraySize; i++)
					{
						showMixed |= hiddenValue.GetArrayElementAtIndex(i).hasMultipleDifferentValues;
						if (showMixed) break;
					}
				}

				if (!showMixed)
				{
					var bytes = new byte[hiddenValue.arraySize];
					for (var i = 0; i < hiddenValue.arraySize; i++)
					{
						bytes[i] = (byte)hiddenValue.GetArrayElementAtIndex(i).intValue;
					}

					val = ObscuredString.EncryptDecrypt(GetString(bytes), currentCryptoKey);
				}
				else
				{
					EditorGUI.showMixedValue = true;
				}
			}

			var dataIndex = prop.propertyPath.IndexOf("Array.data[", System.StringComparison.Ordinal);

			if (dataIndex >= 0)
			{
				dataIndex += 11;

				var index = "Element " + prop.propertyPath.Substring(dataIndex, prop.propertyPath.IndexOf("]", dataIndex, System.StringComparison.Ordinal) - dataIndex);
				label.text = index;
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.TextField(position, label, val);
			if (EditorGUI.EndChangeCheck())
			{
				EncryptAndSetBytes(val, hiddenValue, currentCryptoKey);
				fakeValue.stringValue = val;
				fakeValueActive.boolValue = true;
			}

			EditorGUI.showMixedValue = false;

			/*if (prop.isInstantiatedPrefab)
			{
				SetBoldDefaultFont(prop.prefabOverride);
			}*/
			ResetBoldFont();
		}
 
		private static void EncryptAndSetBytes(string val, SerializedProperty prop, string key)
		{
			var encrypted = ObscuredString.EncryptDecrypt(val, key);
			var encryptedBytes = GetBytes(encrypted);

			prop.ClearArray();
			prop.arraySize = encryptedBytes.Length;

			for (var i = 0; i < encryptedBytes.Length; i++)
			{
				prop.GetArrayElementAtIndex(i).intValue = encryptedBytes[i];
			}
		}

		private static byte[] GetBytes(string str)
		{
			var bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		private static string GetString(byte[] bytes)
		{
			var chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}
	}
}
#endif