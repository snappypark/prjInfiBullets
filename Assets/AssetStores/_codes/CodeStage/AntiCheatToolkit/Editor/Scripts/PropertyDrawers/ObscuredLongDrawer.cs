#if UNITY_EDITOR

using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredLong))]
	internal class ObscuredLongDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			var hiddenValue = prop.FindPropertyRelative("hiddenValue");
			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			var inited = prop.FindPropertyRelative("inited");
			var fakeValue = prop.FindPropertyRelative("fakeValue");
			var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

			var currentCryptoKey = cryptoKey.longValue;
			long val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.longValue = ObscuredLong.cryptoKeyEditor;
				}
				hiddenValue.longValue = ObscuredLong.Encrypt(0, currentCryptoKey);
				inited.boolValue = true;
				fakeValue.longValue = 0;
			}
			else
			{
				val = ObscuredLong.Decrypt(hiddenValue.longValue, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.LongField(position, label, val);
			if (EditorGUI.EndChangeCheck())
			{
				hiddenValue.longValue = ObscuredLong.Encrypt(val, currentCryptoKey);
				fakeValue.longValue = val;
				fakeValueActive.boolValue = true;
			}
			
			ResetBoldFont();
		}
	}
}
#endif