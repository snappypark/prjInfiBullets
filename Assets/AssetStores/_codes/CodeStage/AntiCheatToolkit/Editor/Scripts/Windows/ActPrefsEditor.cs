#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_EDITOR_WIN
using Microsoft.Win32;
#elif UNITY_EDITOR_OSX
using System.IO;
#else // LINUX
using System.IO;
using System.Xml;
#endif

namespace CodeStage.AntiCheat.EditorCode.Windows
{
	internal class ActPrefsEditor : EditorWindow
	{
		protected const int RECORDS_PER_PAGE = 50;

		protected const string DEFAULT_STRING = "[^_; = ElinaKristinaMyGirlsLoveYou'16 = ;_^]";
		protected const float DEFAULT_FLOAT = float.MinValue + 2016.0122f;
		protected const int DEFAULT_INT = int.MinValue + 20130524;

		protected const string UNKNOWN_VALUE_DESCRIPTION = "Value corrupted / wrong Unity version";
		protected const string UNSUPPORTED_VALUE_DESCRIPTION = "Not editable value";
		protected const string STRING_TOO_LONG = "String is too long";
		protected const string CANT_DECRYPT = "Can't decrypt with specified key";

		// 180, 255, 180, 255
		// note the 2 alpha - it's to make disabled components look as usual
		private readonly Color obscuredColor = new Color(0.706f, 1f, 0.706f, 2f);

		private static ActPrefsEditor instance;

		[SerializeField]
		private SortingType sortingType = SortingType.KeyAscending;

		[SerializeField]
		private string searchPattern;

		[SerializeField]
		private List<PrefsRecord> allRecords;

		[SerializeField]
		private List<PrefsRecord> filteredRecords;

		[SerializeField]
		private Vector2 scrollPosition;

		[SerializeField]
		private int recordsCurrentPage;

		[SerializeField]
		private int recordsTotalPages;

		[SerializeField]
		private bool addingNewRecord;

		[SerializeField]
		private int newRecordType;

		[SerializeField]
		private bool newRecordEncrypted;

		[SerializeField]
		private string newRecordKey;

		[SerializeField]
		private string newRecordStringValue;

		[SerializeField]
		private int newRecordIntValue;

		[SerializeField]
		private float newRecordFloatValue;

		[UnityEditor.MenuItem(ActEditorGlobalStuff.WINDOWS_MENU_PATH + "Prefs Editor as Tab...", false, 500)]
		internal static void ShowWindow()
		{
			var myself = GetWindow<ActPrefsEditor>(false, "Prefs Editor", true);
			myself.minSize = new Vector2(500, 300);
			myself.RefreshData();
		}

		[UnityEditor.MenuItem(ActEditorGlobalStuff.WINDOWS_MENU_PATH + "Prefs Editor as Utility...", false, 501)]
		internal static void ShowWindowUtility()
		{
			var myself = GetWindow<ActPrefsEditor>(true, "Prefs Editor", true);
			myself.minSize = new Vector2(500, 300);
			myself.RefreshData();
		}

		[DidReloadScripts]
		private static void OnRecompile()
		{ 
			if (instance) instance.Repaint();
		}
		 
		private void OnEnable()
		{
			instance = this;
			//RefreshData();
		}

		#region GUI

		// ----------------------------------------------------------------------------
		// GUI
		// ----------------------------------------------------------------------------

		private void OnGUI()
		{
			if (allRecords == null) allRecords = new List<PrefsRecord>();
			if (filteredRecords == null) filteredRecords = new List<PrefsRecord>();

			using (ActEditorGUI.Horizontal(ActEditorGUI.Toolbar))
			{
				if (GUILayout.Button(new GUIContent("+", "Create new prefs record."), EditorStyles.toolbarButton, GUILayout.Width(20)))
				{
					addingNewRecord = true;
				}

				if (GUILayout.Button(new GUIContent("Refresh", "Re-read and re-parse all prefs."), EditorStyles.toolbarButton, GUILayout.Width(50)))
				{
					RefreshData();
					GUIUtility.keyboardControl = 0;
					scrollPosition = Vector2.zero;
					recordsCurrentPage = 0;
				}

				EditorGUI.BeginChangeCheck();
				sortingType = (SortingType)EditorGUILayout.EnumPopup(sortingType, EditorStyles.toolbarDropDown, GUILayout.Width(110));
				if (EditorGUI.EndChangeCheck())
				{
					ApplySorting();
				}

				GUILayout.Space(10);

				EditorGUI.BeginChangeCheck();
				searchPattern = ActEditorGUI.SearchToolbar(searchPattern);
				if (EditorGUI.EndChangeCheck())
				{
					ApplyFiltering();
				}

				EditorGUI.BeginChangeCheck();
				EditorGUIUtility.labelWidth = 80;
				GUILayout.Label(new GUIContent("Crypto key", "ObscuredPrefs crypto key to use."), ActEditorGUI.ToolbarLabel, GUILayout.ExpandWidth(false));

				var newKey = EditorGUILayout.TextField(ObscuredPrefs.CryptoKey, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
				EditorGUIUtility.labelWidth = 0;
				if (EditorGUI.EndChangeCheck())
				{
					ObscuredPrefs.CryptoKey = newKey;
				}
			}

			if (addingNewRecord)
			{
				using (ActEditorGUI.Horizontal(ActEditorGUI.PanelWithBackground))
				{
					string[] types = {"String", "Int", "Float"};
					newRecordType = EditorGUILayout.Popup(newRecordType, types, GUILayout.Width(50));

					newRecordEncrypted = GUILayout.Toggle(newRecordEncrypted, new GUIContent("E", "Create new pref as encrypted ObscuredPref?"), ActEditorGUI.CompactButton, GUILayout.Width(25));

					var guiColor = GUI.color;
					if (newRecordEncrypted)
					{
						GUI.color = obscuredColor;
					}

					GUILayout.Label("Key:", GUILayout.ExpandWidth(false));
					newRecordKey = EditorGUILayout.TextField(newRecordKey);
					GUILayout.Label("Value:", GUILayout.ExpandWidth(false));

					if (newRecordType == 0)
					{
						newRecordStringValue = EditorGUILayout.TextField(newRecordStringValue);
					}
					else if (newRecordType == 1)
					{
						newRecordIntValue = EditorGUILayout.IntField(newRecordIntValue);
					}
					else
					{
						newRecordFloatValue = EditorGUILayout.FloatField(newRecordFloatValue);
					}

					GUI.color = guiColor;

					if (GUILayout.Button("OK", ActEditorGUI.CompactButton, GUILayout.Width(30)))
					{
						if (string.IsNullOrEmpty(newRecordKey) ||
						    (newRecordType == 0 && string.IsNullOrEmpty(newRecordStringValue)) ||
						    (newRecordType == 1 && newRecordIntValue == 0) ||
						    (newRecordType == 2 && Mathf.Abs(newRecordFloatValue) < 0.00000001f))
						{
							ShowNotification(new GUIContent("Please fill in the pref first!"));
						}
						else
						{
							PrefsRecord newRecord;

							if (newRecordType == 0)
							{
								newRecord = new PrefsRecord(newRecordKey, newRecordStringValue, newRecordEncrypted);
							}
							else if (newRecordType == 1)
							{
								newRecord = new PrefsRecord(newRecordKey, newRecordIntValue, newRecordEncrypted);
							}
							else
							{
								newRecord = new PrefsRecord(newRecordKey, newRecordFloatValue, newRecordEncrypted);
							}

							if (newRecord.Save())
							{
								allRecords.Add(newRecord);
								ApplySorting();
								CloseNewRecordPanel();
							}
						}
					}

					if (GUILayout.Button("Cancel", ActEditorGUI.CompactButton, GUILayout.Width(60)))
					{
						CloseNewRecordPanel();
					}
				}
			}

			using (ActEditorGUI.Vertical(ActEditorGUI.PanelWithBackground))
			{
				GUILayout.Space(5);

				DrawRecordsPages();

				GUILayout.Space(5);

				GUI.enabled = filteredRecords.Count > 0;
				using (ActEditorGUI.Horizontal())
				{
					if (GUILayout.Button("Encrypt ALL", ActEditorGUI.CompactButton))
					{
						if (EditorUtility.DisplayDialog("Obscure ALL prefs in list?", "This will apply obscuration to ALL unobscured prefs in the list.\nAre you sure you wish to do this?", "Yep", "Oh, no!"))
						{
							foreach (var record in filteredRecords)
							{
								record.Encrypt();
							}
							GUIUtility.keyboardControl = 0;
							ApplySorting();
						}
					}

					if (GUILayout.Button("Decrypt ALL", ActEditorGUI.CompactButton))
					{
						if (EditorUtility.DisplayDialog("UnObscure ALL prefs in list?", "This will remove obscuration from ALL obscured prefs in the list if possible.\nAre you sure you wish to do this?", "Yep", "Oh, no!"))
						{
							foreach (var record in filteredRecords)
							{
								record.Decrypt();
							}
							GUIUtility.keyboardControl = 0;
							ApplySorting();
						}
					}

					if (GUILayout.Button("Save ALL", ActEditorGUI.CompactButton))
					{
						if (EditorUtility.DisplayDialog("Save changes to ALL prefs in list?", "Are you sure you wish to save changes to ALL prefs in the list? This can't be undone!", "Yep", "Oh, no!"))
						{
							foreach (var record in filteredRecords)
							{
								record.Save();
							}
							GUIUtility.keyboardControl = 0;
							ApplySorting();
						}
					}

					if (GUILayout.Button("Delete ALL", ActEditorGUI.CompactButton))
					{
						if (EditorUtility.DisplayDialog("Delete ALL prefs in list?", "Are you sure you wish to delete the ALL prefs in the list? This can't be undone!", "Yep", "Oh, no!"))
						{
							foreach (var record in filteredRecords)
							{
								record.Delete();
							}
							
							RefreshData();
							GUIUtility.keyboardControl = 0;
						}
					}
				}
				GUI.enabled = true;
			}
		}

		private void CloseNewRecordPanel()
		{
			addingNewRecord = false;
			newRecordKey = string.Empty;
			newRecordStringValue = string.Empty;
			newRecordIntValue = 0;
			newRecordFloatValue = 0;
			GUIUtility.keyboardControl = 0;
		}

		private void DrawRecordsPages()
		{
            /*
			recordsTotalPages = Mathf.Max(1,(int)Mathf.Ceiling((double)filteredRecords.Count / RECORDS_PER_PAGE));

			if (recordsCurrentPage < 0) recordsCurrentPage = 0;
			if (recordsCurrentPage + 1 > recordsTotalPages) recordsCurrentPage = recordsTotalPages - 1;

			var fromRecord = recordsCurrentPage * RECORDS_PER_PAGE;
			var toRecord = fromRecord + Mathf.Min(RECORDS_PER_PAGE, filteredRecords.Count - fromRecord);

			if (recordsTotalPages > 1)
			{
				GUILayout.Label("Prefs " + fromRecord + " - " + toRecord + " from " + filteredRecords.Count);
			}

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			for (var i = fromRecord; i < toRecord; i++)
			{
				bool recordRemoved;
				DrawRecord(i, out recordRemoved);
				if (recordRemoved)
				{
					break;
				}
			}
			GUILayout.EndScrollView();

			if (recordsTotalPages <= 1) return;

			GUILayout.Space(5);
			using (ActEditorGUI.Horizontal())
			{
				GUILayout.FlexibleSpace();

				GUI.enabled = recordsCurrentPage > 0;
				if (GUILayout.Button("<<", GUILayout.Width(50)))
				{
					RemoveNotification();
					recordsCurrentPage = 0;
					scrollPosition = Vector2.zero;
				}
				if (GUILayout.Button("<", GUILayout.Width(50)))
				{
					RemoveNotification();
					recordsCurrentPage--;
					scrollPosition = Vector2.zero;
				}
				GUI.enabled = true;
				GUILayout.Label(recordsCurrentPage + 1 + " of " + recordsTotalPages, ActEditorGUI.CenteredLabel, GUILayout.Width(100));
				GUI.enabled = recordsCurrentPage < recordsTotalPages - 1;
				if (GUILayout.Button(">", GUILayout.Width(50)))
				{
					RemoveNotification();
					recordsCurrentPage++;
					scrollPosition = Vector2.zero;
				}
				if (GUILayout.Button(">>", GUILayout.Width(50)))
				{
					RemoveNotification();
					recordsCurrentPage = recordsTotalPages - 1;
					scrollPosition = Vector2.zero;
				}
				GUI.enabled = true;

				GUILayout.FlexibleSpace();
			}//*/
        }

        protected void DrawRecord(int recordIndex, out bool recordRemoved)
		{
			recordRemoved = false;
			var record = filteredRecords[recordIndex];

			ActEditorGUI.Separator();

			using (ActEditorGUI.Horizontal(ActEditorGUI.PanelWithBackground))
			{
				if (GUILayout.Button(new GUIContent("X", "Delete this pref."), ActEditorGUI.CompactButton, GUILayout.Width(20)))
				{
					record.Delete();
					allRecords.Remove(record);
					filteredRecords.Remove(record);
					recordRemoved = true;
					return;
				}

				GUI.enabled = record.dirtyValue || record.dirtyKey && record.prefType != PrefsRecord.PrefsType.Unknown;
				if (GUILayout.Button(new GUIContent("S", "Save changes in this pref."), ActEditorGUI.CompactButton, GUILayout.Width(20)))
				{
					record.Save();
					GUIUtility.keyboardControl = 0;
				}
				GUI.enabled = true;

				GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;

				if (record.Obscured)
				{
					GUI.enabled &= record.obscuredType == ObscuredPrefs.DataType.String ||
								   record.obscuredType == ObscuredPrefs.DataType.Int ||
								   record.obscuredType == ObscuredPrefs.DataType.Float;
					if (GUILayout.Button(new GUIContent("D", "Decrypt this pref using ObscuredPrefs"), ActEditorGUI.CompactButton, GUILayout.Width(25)))
					{
						record.Decrypt();
						GUIUtility.keyboardControl = 0;
					}
				}
				else
				{
					if (GUILayout.Button(new GUIContent("E", "Encrypt this pref using ObscuredPrefs"), ActEditorGUI.CompactButton, GUILayout.Width(25)))
					{
						record.Encrypt();
						GUIUtility.keyboardControl = 0;
					}
				}
				GUI.enabled = true;

				if (GUILayout.Button(new GUIContent("...", "Other operations"), ActEditorGUI.CompactButton, GUILayout.Width(25)))
				{
					ShowOtherMenu(record);
				}

				var guiColor = GUI.color;
				if (record.Obscured)
				{
					GUI.color = obscuredColor;
				}

				GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;

				if (record.Obscured && !(record.obscuredType == ObscuredPrefs.DataType.String ||
				                         record.obscuredType == ObscuredPrefs.DataType.Int ||
				                         record.obscuredType == ObscuredPrefs.DataType.Float))
				{
					GUI.enabled = false;
					EditorGUILayout.TextField(record.Key, GUILayout.MaxWidth(200), GUILayout.MinWidth(50));
					GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;
				}
				else
				{
					record.Key = EditorGUILayout.TextField(record.Key, GUILayout.MaxWidth(200), GUILayout.MinWidth(50));
				}
				
				if ((record.prefType == PrefsRecord.PrefsType.String && !record.Obscured) || (record.Obscured && record.obscuredType == ObscuredPrefs.DataType.String))
				{
					// to avoid TextMeshGenerator error because of too much characters
					if (record.StringValue.Length > 16382)
					{
						GUI.enabled = false;
						EditorGUILayout.TextField(STRING_TOO_LONG, GUILayout.MinWidth(150));
						GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;
					}
					else
					{
						record.StringValue = EditorGUILayout.TextField(record.StringValue, GUILayout.MinWidth(150));
					}
				}
				else if (record.prefType == PrefsRecord.PrefsType.Int || (record.Obscured && record.obscuredType == ObscuredPrefs.DataType.Int))
				{
					record.IntValue = EditorGUILayout.IntField(record.IntValue, GUILayout.MinWidth(150));
				}
				else if (record.prefType == PrefsRecord.PrefsType.Float || (record.Obscured && record.obscuredType == ObscuredPrefs.DataType.Float))
				{
					record.FloatValue = EditorGUILayout.FloatField(record.FloatValue, GUILayout.MinWidth(150));
				}
				else if (record.Obscured)
				{
					GUI.enabled = false;
					EditorGUILayout.TextField(UNSUPPORTED_VALUE_DESCRIPTION, GUILayout.MinWidth(150));
					GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;
				}
				else
				{
					GUI.enabled = false;
					EditorGUILayout.TextField(UNKNOWN_VALUE_DESCRIPTION, GUILayout.MinWidth(150));
					GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;
				}
				GUI.color = guiColor;
				GUI.enabled = true;

				EditorGUILayout.LabelField(record.DisplayType, GUILayout.Width(70));
			}
		}

		private void ShowOtherMenu(PrefsRecord record)
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Copy to clipboard"), false, () =>
			{
				EditorGUIUtility.systemCopyBuffer = record.ToString();
			});

			if (record.Obscured)
			{
				menu.AddItem(new GUIContent("Copy obscured raw data to clipboard"), false, () =>
				{
					EditorGUIUtility.systemCopyBuffer = record.ToString(true);
				});
			}

			menu.ShowAsContext();
		}

		#endregion

		private void RefreshData()
		{
			var keys = new List<string>();

#if UNITY_EDITOR_WIN
			keys.AddRange(ReadKeysWin());
#elif UNITY_EDITOR_OSX
			keys.AddRange(ReadKeysOSX());
#else // LINUX
			keys.AddRange(ReadKeysLinux());
#endif

			keys.Remove("UnityGraphicsQuality");
			keys.Remove("UnitySelectMonitor");
			keys.Remove("Screenmanager Resolution Width");
			keys.Remove("Screenmanager Resolution Height");
			keys.Remove("Screenmanager Is Fullscreen mode");
			keys.Remove("unity.cloud_userid");
			keys.Remove("unity.player_session_background_time");
			keys.Remove("unity.player_session_elapsed_time");
			keys.Remove("unity.player_sessionid");

			if (allRecords == null) allRecords = new List<PrefsRecord>();
			if (filteredRecords == null) filteredRecords = new List<PrefsRecord>();

			allRecords.Clear();
			filteredRecords.Clear();

			var keysCount = keys.Count;
			var showProgress = keysCount >= 500;

			for (var i = 0; i < keysCount; i++)
			{
				var keyName = keys[i];
				if (showProgress)
				{
					if (EditorUtility.DisplayCancelableProgressBar("Reading PlayerPrefs [" + (i + 1) + " of " + keysCount + "]", "Reading " + keyName, (float)i/keysCount))
					{
						break;
					}
				}
				allRecords.Add(new PrefsRecord(keyName));
			}

			if (showProgress) EditorUtility.ClearProgressBar();

			ApplySorting();
		}

		private void ApplyFiltering()
		{
			filteredRecords.Clear();
			if (string.IsNullOrEmpty(searchPattern))
			{
				filteredRecords.AddRange(allRecords);
			}
			else
			{
				foreach (var record in allRecords)
				{
					if (record.Key.ToLowerInvariant().Contains(searchPattern.Trim().ToLowerInvariant()))
					{
						filteredRecords.Add(record);
					}
				}
			}
		}

		private void ApplySorting()
		{
			switch (sortingType)
			{
				case SortingType.KeyAscending:
					allRecords.Sort(PrefsRecord.SortByNameAscending);
					break;
				case SortingType.KeyDescending:
					allRecords.Sort(PrefsRecord.SortByNameDescending);
					break;
				case SortingType.Type:
					allRecords.Sort(PrefsRecord.SortByType);
					break;
				case SortingType.Obscurance:
					allRecords.Sort(PrefsRecord.SortByObscurance);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			ApplyFiltering();
		}

#if UNITY_EDITOR_WIN

		private string[] ReadKeysWin()
		{
#if UNITY_5_5_OR_NEWER
			var registryLocation = Registry.CurrentUser.CreateSubKey("Software\\Unity\\UnityEditor\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);
#else
			var registryLocation = Registry.CurrentUser.CreateSubKey("Software\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);
#endif
			if (registryLocation == null)
			{
				return new string[0];
			}

			var names = registryLocation.GetValueNames();
			var result = new string[names.Length];

			for (var i = 0; i < names.Length; i++)
			{
				var key = names[i];
				if (key.IndexOf('_') > 0)
				{
					result[i] = key.Substring(0, key.LastIndexOf('_'));
				}
				else
				{
					result[i] = key;
				}
			}

			return result;
		}

#elif UNITY_EDITOR_OSX

		private string[] ReadKeysOSX()
		{
			var plistPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/unity." + 
				PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist";

			if (!File.Exists (plistPath)) 
			{
				return new string[0];
			}

			var parsedPlist = (Dictionary<string, object>)Plist.readPlist(plistPath);

			var keys = new string[parsedPlist.Keys.Count];
			parsedPlist.Keys.CopyTo (keys, 0);

			return keys;
		}

#else // LINUX!

		private string[] ReadKeysLinux()
		{
			var prefsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/.config/unity3d/" + 
				PlayerSettings.companyName + "/" + PlayerSettings.productName + "/prefs";

			if (!File.Exists(prefsPath)) 
			{
				return new string[0];
			}

			var prefsXML = new XmlDocument();
			prefsXML.Load(prefsPath);
			var prefsList = prefsXML.SelectNodes("/unity_prefs/pref");

			var keys = new string[prefsList.Count];

			for (var i = 0; i < keys.Length; i++)
			{
				keys[i] = prefsList[i].Attributes["name"].Value;
			}

			return keys;
		}

#endif

		private enum SortingType : byte
		{
			KeyAscending = 0,
			KeyDescending = 2,
			Type = 5,
			Obscurance = 10
		}

#region PrefsRecord

		// ----------------------------------------------------------------------------
		// PrefsRecord class
		// ----------------------------------------------------------------------------

		[Serializable]
		internal class PrefsRecord
		{
			internal PrefsType prefType = PrefsType.Unknown;
			internal ObscuredPrefs.DataType obscuredType = ObscuredPrefs.DataType.Unknown;

			internal bool dirtyKey;
			internal bool dirtyValue;

			[SerializeField]
			private string savedKey;

			[SerializeField]
			private string key;

			internal string Key
			{
				get { return key; }
				set
				{
					if (value == key) return;
					key = value;

					dirtyKey = true;
				}
			}

			[SerializeField]
			private string stringValue;

			internal string StringValue
			{
				get { return stringValue; }
				set
				{
					if (value == stringValue) return;

					stringValue = value;
					dirtyValue = true;
				}
			}

			[SerializeField]
			private int intValue;

			internal int IntValue
			{
				get { return intValue; }
				set
				{
					if (value == intValue) return;

					intValue = value;
					dirtyValue = true;
				}
			}

			[SerializeField]
			private float floatValue;

			internal float FloatValue
			{
				get { return floatValue; }
				set
				{
					if (Mathf.Abs(value - floatValue) < 0.0000001f) return;

					floatValue = value;
					dirtyValue = true;
				}
			}

			internal string DisplayValue
			{
				get
				{
					switch (prefType)
					{
						case PrefsType.Unknown:
							return UNKNOWN_VALUE_DESCRIPTION;
						case PrefsType.String:
							return IsEditableObscuredValue() ? stringValue : UNSUPPORTED_VALUE_DESCRIPTION;
						case PrefsType.Int:
							return intValue.ToString();
						case PrefsType.Float:
							return floatValue.ToString(CultureInfo.InvariantCulture);
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}

			internal string DisplayType
			{
				get { return Obscured ? obscuredType.ToString() : prefType.ToString(); }
			}

			internal static int SortByNameAscending(PrefsRecord n1, PrefsRecord n2)
			{
				return string.CompareOrdinal(n1.key, n2.key);
			}

			internal static int SortByNameDescending(PrefsRecord n1, PrefsRecord n2)
			{
				var result = string.CompareOrdinal(n2.key, n1.key);
				return result;
			}

			internal static int SortByType(PrefsRecord n1, PrefsRecord n2)
			{
				var result = string.CompareOrdinal(n1.DisplayType, n2.DisplayType);
				if (result == 0)
				{
					return SortByNameAscending(n1, n2);
				}
				return result;
			}

			internal static int SortByObscurance(PrefsRecord n1, PrefsRecord n2)
			{
				var result = n1.Obscured.CompareTo(n2.Obscured);

				if (result == 0)
				{
					return SortByNameAscending(n1, n2);
				}

				return result;
			}

			[SerializeField]
			internal bool Selected { get; set; }

			[SerializeField]
			internal bool Obscured { get; set; }

			internal PrefsRecord(string newKey, string value, bool encrypted)
			{
				key = savedKey = newKey;
				stringValue = value;

				prefType = PrefsType.String;

				if (encrypted)
				{
					obscuredType = ObscuredPrefs.DataType.String;
					Obscured = true;
				}
			}

			internal PrefsRecord(string newKey, int value, bool encrypted)
			{
				key = savedKey = newKey;
				intValue = value;

				if (encrypted)
				{
					prefType = PrefsType.String;
					obscuredType = ObscuredPrefs.DataType.Int;
					Obscured = true;
				}
				else
				{
					prefType = PrefsType.Int;
				}
			}

			internal PrefsRecord(string newKey, float value, bool encrypted)
			{
				key = savedKey = newKey;
				floatValue = value;

				if (encrypted)
				{
					prefType = PrefsType.String;
					obscuredType = ObscuredPrefs.DataType.Float;
					Obscured = true;
				}
				else
				{
					prefType = PrefsType.Float;
				}
			}

			internal PrefsRecord(string originalKey)
			{
				key = savedKey = originalKey;

				ReadValue();

				// only string prefs may be obscured
				if (prefType == PrefsType.String)
				{
					Obscured = IsValueObscured(stringValue);

					if (Obscured)
					{
						key = DecryptKey(key);

						if (obscuredType == ObscuredPrefs.DataType.String)
						{
							stringValue = ObscuredPrefs.DecryptStringValue(key, stringValue, DEFAULT_STRING);
							if (stringValue == DEFAULT_STRING) stringValue = CANT_DECRYPT;
						}
						else if (obscuredType == ObscuredPrefs.DataType.Int)
						{
							intValue = ObscuredPrefs.DecryptIntValue(key, stringValue, DEFAULT_INT);
							if (intValue == DEFAULT_INT)
							{
								obscuredType = ObscuredPrefs.DataType.String;
								stringValue = CANT_DECRYPT;
							}
						}
						else if (obscuredType == ObscuredPrefs.DataType.Float)
						{
							floatValue = ObscuredPrefs.DecryptFloatValue(key, stringValue, DEFAULT_FLOAT);
							if (floatValue == DEFAULT_FLOAT)
							{
								obscuredType = ObscuredPrefs.DataType.String;
								stringValue = CANT_DECRYPT;
							}
						}
					}
				}
			}

			internal bool Save(bool newRecord = false)
			{
				var savedString = stringValue;
				string newSavedKey;

				if (Obscured)
				{
					savedString = GetEncryptedValue();
					newSavedKey = GetEncryptedKey();
				}
				else
				{
					newSavedKey = key;
				}

				if (newSavedKey != savedKey && PlayerPrefs.HasKey(newSavedKey))
				{
					if (!EditorUtility.DisplayDialog("Pref overwrite", "Pref with name " + key + " already exists!\n" + "Are you sure you wish to overwrite it?", "Yes", "No"))
					{
						return false;
					}
				}

				if (dirtyKey)
				{
					PlayerPrefs.DeleteKey(savedKey);
				}

				switch (prefType)
				{
					case PrefsType.Unknown:
						Debug.LogError(ActEditorGlobalStuff.LOG_PREFIX + "Can't save Pref of unknown type!");
						break;
					case PrefsType.String:
						PlayerPrefs.SetString(newSavedKey, savedString);
						break;
					case PrefsType.Int:
						PlayerPrefs.SetInt(newSavedKey, intValue);
						break;
					case PrefsType.Float:
						PlayerPrefs.SetFloat(newSavedKey, floatValue);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				savedKey = newSavedKey;

				dirtyKey = false;
				dirtyValue = false;

				PlayerPrefs.Save();

				return true;
			}

			internal void Delete()
			{
				PlayerPrefs.DeleteKey(savedKey);
				PlayerPrefs.Save();
			}

			internal void Encrypt()
			{
				if (Obscured) return;

				var success = true;

				switch (prefType)
				{
					case PrefsType.Unknown:
						success = false;
						Debug.LogError(ActEditorGlobalStuff.LOG_PREFIX + "Can't encrypt pref of unknown type!");
						break;
					case PrefsType.String:
						obscuredType = ObscuredPrefs.DataType.String;
						break;
					case PrefsType.Int:
						obscuredType = ObscuredPrefs.DataType.Int;
						break;
					case PrefsType.Float:
						obscuredType = ObscuredPrefs.DataType.Float;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				if (success)
				{
					prefType = PrefsType.String;
					Obscured = true;
					dirtyValue = dirtyKey = true;
				}
			}

			internal void Decrypt()
			{
				if (!Obscured) return;
				if (!IsEditableObscuredValue()) return;

				var success = true;

				switch (obscuredType)
				{
					case ObscuredPrefs.DataType.Int:
						prefType = PrefsType.Int;
						break;
					case ObscuredPrefs.DataType.String:
						prefType = PrefsType.String;
						break;
					case ObscuredPrefs.DataType.Float:
						prefType = PrefsType.Float;
						break;
					case ObscuredPrefs.DataType.UInt:
					case ObscuredPrefs.DataType.Double:
					case ObscuredPrefs.DataType.Long:
					case ObscuredPrefs.DataType.Bool:
					case ObscuredPrefs.DataType.ByteArray:
					case ObscuredPrefs.DataType.Vector2:
					case ObscuredPrefs.DataType.Vector3:
					case ObscuredPrefs.DataType.Quaternion:
					case ObscuredPrefs.DataType.Color:
					case ObscuredPrefs.DataType.Rect:
						instance.ShowNotification(new GUIContent("Type " + obscuredType + " isn't supported"));
						success = false;
						break;
					case ObscuredPrefs.DataType.Unknown:
						instance.ShowNotification(new GUIContent("Can't decrypt " + key));
						success = false;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				if (success)
				{
					Obscured = false;
					obscuredType = ObscuredPrefs.DataType.Unknown;
					dirtyValue = dirtyKey = true;
				}
			}

			internal string GetEncryptedKey()
			{
				return ObscuredPrefs.EncryptKey(key);
			}

			internal string GetEncryptedValue()
			{
				string savedString;

				switch (obscuredType)
				{
					case ObscuredPrefs.DataType.Int:
						savedString = ObscuredPrefs.EncryptIntValue(key, intValue);
						break;
					case ObscuredPrefs.DataType.String:
						savedString = ObscuredPrefs.EncryptStringValue(key, stringValue);
						break;
					case ObscuredPrefs.DataType.Float:
						savedString = ObscuredPrefs.EncryptFloatValue(key, floatValue);
						break;
					case ObscuredPrefs.DataType.Unknown:
					case ObscuredPrefs.DataType.UInt:
					case ObscuredPrefs.DataType.Double:
					case ObscuredPrefs.DataType.Long:
					case ObscuredPrefs.DataType.Bool:
					case ObscuredPrefs.DataType.ByteArray:
					case ObscuredPrefs.DataType.Vector2:
					case ObscuredPrefs.DataType.Vector3:
					case ObscuredPrefs.DataType.Quaternion:
					case ObscuredPrefs.DataType.Color:
					case ObscuredPrefs.DataType.Rect:
						savedString = stringValue;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				return savedString;
			}

			internal bool IsEditableObscuredValue()
			{
				return (obscuredType == ObscuredPrefs.DataType.Int) || (obscuredType == ObscuredPrefs.DataType.String) || (obscuredType == ObscuredPrefs.DataType.Float);
			}

			internal string ToString(bool raw = false)
			{
				string result;

				if (raw)
				{
					result = "Key: " + GetEncryptedKey() + Environment.NewLine + "Value: " + GetEncryptedValue();
				}
				else
				{
					result = "Key: " + key + Environment.NewLine + "Value: " + DisplayValue;
				}

				return result;
			}

			private void ReadValue()
			{
				var floatTry = PlayerPrefs.GetFloat(key, DEFAULT_FLOAT);
				if (Mathf.Abs(floatTry - DEFAULT_FLOAT) > 0.0000001f)
				{
					prefType = PrefsType.Float;
					floatValue = floatTry;
					return;
				}

				var intTry = PlayerPrefs.GetInt(key, DEFAULT_INT);
				if (intTry != DEFAULT_INT)
				{
					prefType = PrefsType.Int;
					intValue = intTry;
					return;
				}

				var stringTry = PlayerPrefs.GetString(key, DEFAULT_STRING);
				if (stringTry != DEFAULT_STRING)
				{
					prefType = PrefsType.String;
					stringValue = stringTry;
					return;
				}
			}

			private string DecryptKey(string encryptedKey)
			{
				string decryptedKey;

				try
				{
					var bytes = Convert.FromBase64String(encryptedKey);
					decryptedKey = Encoding.UTF8.GetString(bytes);
					decryptedKey = ObscuredString.EncryptDecrypt(decryptedKey, ObscuredPrefs.CryptoKey);
				}
				catch
				{
					decryptedKey = string.Empty;
				}

				return decryptedKey;
			}

			private bool IsValueObscured(string value)
			{
				var validBase64String = (value.Length%4 == 0) && Regex.IsMatch(value, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
				if (!validBase64String) return false;

				var dataType = ObscuredPrefs.GetRawValueType(value);
				if (!Enum.IsDefined(typeof(ObscuredPrefs.DataType), dataType) || dataType == ObscuredPrefs.DataType.Unknown)
				{
					return false;
				}

				obscuredType = dataType;

				return true;
			}

			internal enum PrefsType : byte
			{
				Unknown,
				String,
				Int,
				Float
			}
		}
#endregion
	}
}
#endif