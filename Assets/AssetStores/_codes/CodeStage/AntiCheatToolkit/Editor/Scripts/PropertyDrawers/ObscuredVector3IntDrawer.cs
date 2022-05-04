#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredVector3Int))]
	internal class ObscuredVector3IntDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			var hiddenValue = prop.FindPropertyRelative("hiddenValue");
			var hiddenValueX = hiddenValue.FindPropertyRelative("x");
			var hiddenValueY = hiddenValue.FindPropertyRelative("y");
			var hiddenValueZ = hiddenValue.FindPropertyRelative("z");
			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			var inited = prop.FindPropertyRelative("inited");
			var fakeValue = prop.FindPropertyRelative("fakeValue");
			var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

			var currentCryptoKey = cryptoKey.intValue;
			var val = Vector3Int.zero;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.intValue = ObscuredVector3Int.cryptoKeyEditor;
				}
				var ev = ObscuredVector3Int.Encrypt(Vector3Int.zero, currentCryptoKey);
				hiddenValueX.intValue = ev.x;
				hiddenValueY.intValue = ev.y;
				hiddenValueZ.intValue = ev.z;
                inited.boolValue = true;

				fakeValue.vector3IntValue = Vector3Int.zero;
			}
			else
			{
				var ev = new ObscuredVector3Int.RawEncryptedVector3Int
				{
					x = hiddenValueX.intValue,
					y = hiddenValueY.intValue,
					z = hiddenValueZ.intValue
				};
				val = ObscuredVector3Int.Decrypt(ev, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.Vector3IntField(position, label, val);
			if (EditorGUI.EndChangeCheck())
			{
				var ev = ObscuredVector3Int.Encrypt(val, currentCryptoKey);
				hiddenValueX.intValue = ev.x;
				hiddenValueY.intValue = ev.y;
				hiddenValueZ.intValue = ev.z;

				fakeValue.vector3IntValue = val;
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