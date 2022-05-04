#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredVector2Int))]
	internal class ObscuredVector2IntDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{

			var hiddenValue = prop.FindPropertyRelative("hiddenValue");
			var hiddenValueX = hiddenValue.FindPropertyRelative("x");
			var hiddenValueY = hiddenValue.FindPropertyRelative("y");
			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			var inited = prop.FindPropertyRelative("inited");
			var fakeValue = prop.FindPropertyRelative("fakeValue");
			var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

			var currentCryptoKey = cryptoKey.intValue;
			var val = Vector2Int.zero;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.intValue = ObscuredVector2Int.cryptoKeyEditor;
				}
				var ev = ObscuredVector2Int.Encrypt(Vector2Int.zero, currentCryptoKey);
				hiddenValueX.intValue = ev.x;
				hiddenValueY.intValue = ev.y;
                inited.boolValue = true;
				fakeValue.vector2IntValue = Vector2Int.zero;
			}
			else
			{
				var ev = new ObscuredVector2Int.RawEncryptedVector2Int
				{
					x = hiddenValueX.intValue,
					y = hiddenValueY.intValue
				};
				val = ObscuredVector2Int.Decrypt(ev, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.Vector2IntField(position, label, val);
			if (EditorGUI.EndChangeCheck())
			{
				var ev = ObscuredVector2Int.Encrypt(val, currentCryptoKey);
				hiddenValueX.intValue = ev.x;
				hiddenValueY.intValue = ev.y;
				fakeValue.vector2IntValue = val;
				fakeValueActive.boolValue = true;
			}
			
			ResetBoldFont();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.wideMode ? EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight * 2f;
		}
	}
}
#endif