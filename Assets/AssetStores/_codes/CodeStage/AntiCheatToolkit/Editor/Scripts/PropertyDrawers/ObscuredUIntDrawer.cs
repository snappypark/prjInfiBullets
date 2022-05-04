#if UNITY_EDITOR

using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredUInt))]
	internal class ObscuredUIntDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			var hiddenValue = prop.FindPropertyRelative("hiddenValue");
			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			var inited = prop.FindPropertyRelative("inited");
			var fakeValue = prop.FindPropertyRelative("fakeValue");
			var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

			var currentCryptoKey = (uint)cryptoKey.intValue;
			uint val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = ObscuredUInt.cryptoKeyEditor;
					cryptoKey.intValue = (int)ObscuredUInt.cryptoKeyEditor;
				}
				hiddenValue.intValue = (int)ObscuredUInt.Encrypt(0, currentCryptoKey);
				inited.boolValue = true;
			}
			else
			{
				val = ObscuredUInt.Decrypt((uint)hiddenValue.intValue, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = (uint)EditorGUI.IntField(position, label, (int)val);
			if (EditorGUI.EndChangeCheck())
			{
				hiddenValue.intValue = (int)ObscuredUInt.Encrypt(val, currentCryptoKey);
				fakeValue.intValue = (int)val;
				fakeValueActive.boolValue = true;
			}

			
			ResetBoldFont();
		}
	}
}
#endif