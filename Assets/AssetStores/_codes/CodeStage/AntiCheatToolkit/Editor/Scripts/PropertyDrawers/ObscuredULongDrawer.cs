#if UNITY_EDITOR

using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredULong))]
	internal class ObscuredULongDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			var hiddenValue = prop.FindPropertyRelative("hiddenValue");
			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			var inited = prop.FindPropertyRelative("inited");
			var fakeValue = prop.FindPropertyRelative("fakeValue");
			var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

			var currentCryptoKey = (ulong)cryptoKey.longValue;
			ulong val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = ObscuredULong.cryptoKeyEditor;
					cryptoKey.longValue = (long)ObscuredULong.cryptoKeyEditor;
				}
				hiddenValue.longValue = (long)ObscuredULong.Encrypt(0, currentCryptoKey);
				inited.boolValue = true;
			}
			else
			{
				val = ObscuredULong.Decrypt((ulong)hiddenValue.longValue, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = (ulong)EditorGUI.LongField(position, label, (long)val);
			if (EditorGUI.EndChangeCheck())
			{
				hiddenValue.longValue = (long)ObscuredULong.Encrypt(val, currentCryptoKey);
				fakeValue.longValue = (long)val;
				fakeValueActive.boolValue = true;
			}
			
			ResetBoldFont();
		}
	}
}
#endif