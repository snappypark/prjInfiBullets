#if UNITY_EDITOR

using System;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeStage.AntiCheat.EditorCode
{
	/// <summary>
	/// Class with utility functions to help with ACTk migrations after updates.
	/// </summary>
	public class MigrateUtils
	{
		/// <summary>
		/// Checks all prefabs in project for old version of obscured types and tries to migrate values to the new version.
		/// </summary>
		[UnityEditor.MenuItem(ActEditorGlobalStuff.WINDOWS_MENU_PATH + "Migrate obscured types on prefabs...", false, 1100)]
		public static void MigrateObscuredTypesOnPrefabs()
		{
			if (!EditorUtility.DisplayDialog("ACTk Obscured types migration",
					"Are you sure you wish to scan all prefabs in your project and automatically migrate values to the new format?",
					"Yes", "No"))
			{
				Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Obscured types migration was cancelled by user.");
				return;
			}

			AssetDatabase.SaveAssets();

			var assets = AssetDatabase.FindAssets("t:ScriptableObject t:Prefab");
			//string[] prefabs = AssetDatabase.FindAssets("TestPrefab");
			var touchedCount = 0;
			var count = assets.Length;
			for (var i = 0; i < count; i++)
			{
				if (EditorUtility.DisplayCancelableProgressBar("Looking through objects", "Object " + (i + 1) + " from " + count,
					i / (float)count))
				{
					Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Obscured types migration was cancelled by user.");
					break;
				}

				var guid = assets[i];
				var path = AssetDatabase.GUIDToAssetPath(guid);

				var objects = AssetDatabase.LoadAllAssetsAtPath(path);
				foreach (var unityObject in objects)
				{
					if (unityObject == null) continue;
					if (unityObject.name == "Deprecated EditorExtensionImpl") continue;

					var modified = false;
					var so = new SerializedObject(unityObject);
					var sp = so.GetIterator();

					if (sp == null) continue;

					while (sp.NextVisible(true))
					{
						if (sp.propertyType != SerializedPropertyType.Generic) continue;

						var type = sp.type;

						switch (type)
						{
							case "ObscuredBool":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueChanged = sp.FindPropertyRelative("fakeValueChanged");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var currentCryptoKey = cryptoKey.intValue;
									var real = ObscuredBool.Decrypt(hiddenValue.intValue, (byte) currentCryptoKey);
									var fake = fakeValue.boolValue;

									if (real != fake)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);
										fakeValue.boolValue = real;
										if (fakeValueChanged != null) fakeValueChanged.boolValue = true;
										if (fakeValueActive != null) fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
							break;

							case "ObscuredDouble":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								if (hiddenValue == null) continue;

								var hiddenValue1 = hiddenValue.FindPropertyRelative("b1");
								var hiddenValue2 = hiddenValue.FindPropertyRelative("b2");
								var hiddenValue3 = hiddenValue.FindPropertyRelative("b3");
								var hiddenValue4 = hiddenValue.FindPropertyRelative("b4");
								var hiddenValue5 = hiddenValue.FindPropertyRelative("b5");
								var hiddenValue6 = hiddenValue.FindPropertyRelative("b6");
								var hiddenValue7 = hiddenValue.FindPropertyRelative("b7");
								var hiddenValue8 = hiddenValue.FindPropertyRelative("b8");

								var hiddenValueOld = sp.FindPropertyRelative("hiddenValueOld");

								if (hiddenValueOld != null && hiddenValueOld.isArray && hiddenValueOld.arraySize == 8)
								{
									hiddenValue1.intValue = hiddenValueOld.GetArrayElementAtIndex(0).intValue;
									hiddenValue2.intValue = hiddenValueOld.GetArrayElementAtIndex(1).intValue;
									hiddenValue3.intValue = hiddenValueOld.GetArrayElementAtIndex(2).intValue;
									hiddenValue4.intValue = hiddenValueOld.GetArrayElementAtIndex(3).intValue;
									hiddenValue5.intValue = hiddenValueOld.GetArrayElementAtIndex(4).intValue;
									hiddenValue6.intValue = hiddenValueOld.GetArrayElementAtIndex(5).intValue;
									hiddenValue7.intValue = hiddenValueOld.GetArrayElementAtIndex(6).intValue;
									hiddenValue8.intValue = hiddenValueOld.GetArrayElementAtIndex(7).intValue;

									hiddenValueOld.arraySize = 0;

									Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Migrated property " + sp.displayName + ":" + type +
									          " at the object " + unityObject.name + "\n" + path);
									modified = true;
								}

								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var union = new LongBytesUnion();
									union.b8.b1 = (byte)hiddenValue1.intValue;
									union.b8.b2 = (byte)hiddenValue2.intValue;
									union.b8.b3 = (byte)hiddenValue3.intValue;
									union.b8.b4 = (byte)hiddenValue4.intValue;
									union.b8.b5 = (byte)hiddenValue5.intValue;
									union.b8.b6 = (byte)hiddenValue6.intValue;
									union.b8.b7 = (byte)hiddenValue7.intValue;
									union.b8.b8 = (byte)hiddenValue8.intValue;

									var currentCryptoKey = cryptoKey.longValue;
									var real = ObscuredDouble.Decrypt(union.l, currentCryptoKey);
									var fake = fakeValue.doubleValue;

									if (Mathf.Abs((float)(real - fake)) > 0.0000001d)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);
											
										fakeValue.doubleValue = real;
										fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
								break;

							case "ObscuredFloat":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								if (hiddenValue == null) continue;

								var hiddenValue1 = hiddenValue.FindPropertyRelative("b1");
								var hiddenValue2 = hiddenValue.FindPropertyRelative("b2");
								var hiddenValue3 = hiddenValue.FindPropertyRelative("b3");
								var hiddenValue4 = hiddenValue.FindPropertyRelative("b4");

								var hiddenValueOld = sp.FindPropertyRelative("hiddenValueOld");

								if (hiddenValueOld != null && hiddenValueOld.isArray && hiddenValueOld.arraySize == 4)
								{
									hiddenValue1.intValue = hiddenValueOld.GetArrayElementAtIndex(0).intValue;
									hiddenValue2.intValue = hiddenValueOld.GetArrayElementAtIndex(1).intValue;
									hiddenValue3.intValue = hiddenValueOld.GetArrayElementAtIndex(2).intValue;
									hiddenValue4.intValue = hiddenValueOld.GetArrayElementAtIndex(3).intValue;

									hiddenValueOld.arraySize = 0;

									Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Migrated property " + sp.displayName + ":" + type +
									          " at the object " + unityObject.name + "\n" + path);
									modified = true;
								}

								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var union = new IntBytesUnion();
									union.b4.b1 = (byte)hiddenValue1.intValue;
									union.b4.b2 = (byte)hiddenValue2.intValue;
									union.b4.b3 = (byte)hiddenValue3.intValue;
									union.b4.b4 = (byte)hiddenValue4.intValue;

									var currentCryptoKey = cryptoKey.intValue;
									var real = ObscuredFloat.Decrypt(union.i, currentCryptoKey);
									var fake = fakeValue.floatValue;
									if (Mathf.Abs(real - fake) > 0.0000001f)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);

										fakeValue.floatValue = real;
										fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
								break;

							case "ObscuredInt":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var currentCryptoKey = cryptoKey.intValue;
									var real = ObscuredInt.Decrypt(hiddenValue.intValue, currentCryptoKey);
									var fake = fakeValue.intValue;

									if (real != fake)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);
										fakeValue.intValue = real;
										fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
								break;
							case "ObscuredLong":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var currentCryptoKey = cryptoKey.longValue;
									var real = ObscuredLong.Decrypt(hiddenValue.longValue, currentCryptoKey);
									var fake = fakeValue.longValue;

									if (real != fake)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);
										fakeValue.longValue = real;
										fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
								break;

							case "ObscuredShort":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var currentCryptoKey = (short)cryptoKey.intValue;
									var real = ObscuredShort.EncryptDecrypt((short)hiddenValue.intValue, currentCryptoKey);
									var fake = (short)fakeValue.intValue;

									if (real != fake)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);
										fakeValue.intValue = real;
										fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
								break;
							case "ObscuredString":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var currentCryptoKey = cryptoKey.stringValue;
									var bytes = new byte[hiddenValue.arraySize];
									for (var j = 0; j < hiddenValue.arraySize; j++)
									{
										bytes[j] = (byte)hiddenValue.GetArrayElementAtIndex(j).intValue;
									}

									var real = ObscuredString.EncryptDecrypt(GetString(bytes), currentCryptoKey);
									var fake = fakeValue.stringValue;

									if (real != fake)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);
										fakeValue.stringValue = real;
										fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
								break;
							case "ObscuredUInt":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var currentCryptoKey = (uint)cryptoKey.intValue;
									var real = ObscuredUInt.Decrypt((uint)hiddenValue.intValue, currentCryptoKey);
									var fake = (uint)fakeValue.intValue;

									if (real != fake)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);
										fakeValue.intValue = (int)real;
										fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
								break;

							case "ObscuredULong":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var currentCryptoKey = (ulong)cryptoKey.longValue;
									var real = ObscuredULong.Decrypt((ulong)hiddenValue.longValue, currentCryptoKey);
									var fake = (ulong)fakeValue.longValue;

									if (real != fake)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);
										fakeValue.longValue = (long)real;
										fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
								break;
							case "ObscuredVector2":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								if (hiddenValue == null) continue;

								var hiddenValueX = hiddenValue.FindPropertyRelative("x");
								var hiddenValueY = hiddenValue.FindPropertyRelative("y");
									
								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var ev = new ObscuredVector2.RawEncryptedVector2();
									ev.x = hiddenValueX.intValue;
									ev.y = hiddenValueY.intValue;

									var currentCryptoKey = cryptoKey.intValue;
									var real = ObscuredVector2.Decrypt(ev, currentCryptoKey);
									var fake = fakeValue.vector2Value;

									if (real != fake)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);
										fakeValue.vector2Value = real;
										fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
								break;

							case "ObscuredVector3":
							{
								var hiddenValue = sp.FindPropertyRelative("hiddenValue");
								if (hiddenValue == null) continue;

								var hiddenValueX = hiddenValue.FindPropertyRelative("x");
								var hiddenValueY = hiddenValue.FindPropertyRelative("y");
								var hiddenValueZ = hiddenValue.FindPropertyRelative("z");

								var cryptoKey = sp.FindPropertyRelative("currentCryptoKey");
								var fakeValue = sp.FindPropertyRelative("fakeValue");
								var fakeValueActive = sp.FindPropertyRelative("fakeValueActive");
								var inited = sp.FindPropertyRelative("inited");

								if (inited != null && inited.boolValue)
								{
									var ev = new ObscuredVector3.RawEncryptedVector3();
									ev.x = hiddenValueX.intValue;
									ev.y = hiddenValueY.intValue;
									ev.z = hiddenValueZ.intValue;

									var currentCryptoKey = cryptoKey.intValue;
									var real = ObscuredVector3.Decrypt(ev, currentCryptoKey);
									var fake = fakeValue.vector3Value;

									if (real != fake)
									{
										Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Fixed property " + sp.displayName + ":" + type +
										          " at the object " + unityObject.name + "\n" + path);
										fakeValue.vector3Value = real;
										fakeValueActive.boolValue = true;
										modified = true;
									}
								}
							}
								break;
						}
					}

					if (modified)
					{
						touchedCount++;
						so.ApplyModifiedProperties();
						EditorUtility.SetDirty(unityObject);
					}
				}
			}

			AssetDatabase.SaveAssets();

			EditorUtility.ClearProgressBar();

			if (touchedCount > 0)
				Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "Migrated obscured types on " + touchedCount + " objects.");
			else
				Debug.Log(ActEditorGlobalStuff.LOG_PREFIX + "No objects were found for obscured types migration.");
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

		[StructLayout(LayoutKind.Explicit)]
		private struct LongBytesUnion
		{
			[FieldOffset(0)]
			public long l;

			[FieldOffset(0)]
			public ACTkByte8 b8;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct IntBytesUnion
		{
			[FieldOffset(0)]
			public int i;

			[FieldOffset(0)]
			public ACTkByte4 b4;
		}
	}
}
#endif