#if UNITY_EDITOR

using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredDouble))]
	internal class ObscuredDoubleDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			var hiddenValue = prop.FindPropertyRelative("hiddenValue");
			var hiddenValue1 = hiddenValue.FindPropertyRelative("b1");
			var hiddenValue2 = hiddenValue.FindPropertyRelative("b2");
			var hiddenValue3 = hiddenValue.FindPropertyRelative("b3");
			var hiddenValue4 = hiddenValue.FindPropertyRelative("b4");
			var hiddenValue5 = hiddenValue.FindPropertyRelative("b5");
			var hiddenValue6 = hiddenValue.FindPropertyRelative("b6");
			var hiddenValue7 = hiddenValue.FindPropertyRelative("b7");
			var hiddenValue8 = hiddenValue.FindPropertyRelative("b8");

			var hiddenValueOld = prop.FindPropertyRelative("hiddenValueOld");
			SerializedProperty hiddenValueOld1 = null;
			SerializedProperty hiddenValueOld2 = null;
			SerializedProperty hiddenValueOld3 = null;
			SerializedProperty hiddenValueOld4 = null;
			SerializedProperty hiddenValueOld5 = null;
			SerializedProperty hiddenValueOld6 = null;
			SerializedProperty hiddenValueOld7 = null;
			SerializedProperty hiddenValueOld8 = null;

			if (hiddenValueOld != null && hiddenValueOld.isArray && hiddenValueOld.arraySize == 8)
			{
				hiddenValueOld1 = hiddenValueOld.GetArrayElementAtIndex(0);
				hiddenValueOld2 = hiddenValueOld.GetArrayElementAtIndex(1);
				hiddenValueOld3 = hiddenValueOld.GetArrayElementAtIndex(2);
				hiddenValueOld4 = hiddenValueOld.GetArrayElementAtIndex(3);
				hiddenValueOld5 = hiddenValueOld.GetArrayElementAtIndex(4);
				hiddenValueOld6 = hiddenValueOld.GetArrayElementAtIndex(5);
				hiddenValueOld7 = hiddenValueOld.GetArrayElementAtIndex(6);
				hiddenValueOld8 = hiddenValueOld.GetArrayElementAtIndex(7);
			}

			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			var inited = prop.FindPropertyRelative("inited");
			var fakeValue = prop.FindPropertyRelative("fakeValue");
			var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

			var currentCryptoKey = cryptoKey.longValue;

			var union = new LongBytesUnion();
			double val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.longValue = ObscuredDouble.cryptoKeyEditor;
				}
				inited.boolValue = true;

				union.l = ObscuredDouble.Encrypt(0, currentCryptoKey);

				hiddenValue1.intValue = union.b8.b1;
				hiddenValue2.intValue = union.b8.b2;
				hiddenValue3.intValue = union.b8.b3;
				hiddenValue4.intValue = union.b8.b4;
				hiddenValue5.intValue = union.b8.b5;
				hiddenValue6.intValue = union.b8.b6;
				hiddenValue7.intValue = union.b8.b7;
				hiddenValue8.intValue = union.b8.b8;
			}
			else
			{
				if (hiddenValueOld != null && hiddenValueOld.isArray && hiddenValueOld.arraySize == 8)
				{
					union.b8.b1 = (byte)hiddenValueOld1.intValue;
					union.b8.b2 = (byte)hiddenValueOld2.intValue;
					union.b8.b3 = (byte)hiddenValueOld3.intValue;
					union.b8.b4 = (byte)hiddenValueOld4.intValue;
					union.b8.b5 = (byte)hiddenValueOld5.intValue;
					union.b8.b6 = (byte)hiddenValueOld6.intValue;
					union.b8.b7 = (byte)hiddenValueOld7.intValue;
					union.b8.b8 = (byte)hiddenValueOld8.intValue;
				}
				else
				{
					union.b8.b1 = (byte)hiddenValue1.intValue;
					union.b8.b2 = (byte)hiddenValue2.intValue;
					union.b8.b3 = (byte)hiddenValue3.intValue;
					union.b8.b4 = (byte)hiddenValue4.intValue;
					union.b8.b5 = (byte)hiddenValue5.intValue;
					union.b8.b6 = (byte)hiddenValue6.intValue;
					union.b8.b7 = (byte)hiddenValue7.intValue;
					union.b8.b8 = (byte)hiddenValue8.intValue;
				}

				val = ObscuredDouble.Decrypt(union.l, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.DoubleField(position, label, val);
			if (EditorGUI.EndChangeCheck())
			{
				union.l = ObscuredDouble.Encrypt(val, currentCryptoKey);

				hiddenValue1.intValue = union.b8.b1;
				hiddenValue2.intValue = union.b8.b2;
				hiddenValue3.intValue = union.b8.b3;
				hiddenValue4.intValue = union.b8.b4;
				hiddenValue5.intValue = union.b8.b5;
				hiddenValue6.intValue = union.b8.b6;
				hiddenValue7.intValue = union.b8.b7;
				hiddenValue8.intValue = union.b8.b8;

				if (hiddenValueOld != null && hiddenValueOld.isArray && hiddenValueOld.arraySize == 8)
				{
					hiddenValueOld.arraySize = 0;
				}

				fakeValue.doubleValue = val;
				fakeValueActive.boolValue = true;
			}
			
			ResetBoldFont();
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct LongBytesUnion
		{
			[FieldOffset(0)]
			public long l;

			[FieldOffset(0)]
			public ACTkByte8 b8;
		}
	}
}
#endif