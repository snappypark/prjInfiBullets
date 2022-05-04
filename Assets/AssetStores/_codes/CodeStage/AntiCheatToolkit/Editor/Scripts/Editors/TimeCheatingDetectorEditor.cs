#if UNITY_EDITOR
using CodeStage.AntiCheat.Detectors;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.Editors
{
	[CustomEditor(typeof (TimeCheatingDetector))]
	internal class TimeCheatingDetectorEditor : ActDetectorEditor
	{
#if !UNITY_WINRT
		private SerializedProperty interval;
		private SerializedProperty threshold;
		protected override void FindUniqueDetectorProperties()
		{
			interval = serializedObject.FindProperty("interval");
			threshold = serializedObject.FindProperty("threshold");
		}

		protected override void DrawUniqueDetectorProperties()
		{
			EditorGUILayout.PropertyField(interval);
			EditorGUILayout.PropertyField(threshold);

			GUILayout.Label("<b>Needs Internet connection!</b>", ActEditorGUI.RichMiniLabel);

		}
#else
		protected override void DrawUniqueDetectorProperties()
		{
			GUILayout.Label("<b>Doesn't supported on Universal Windows Platform!</b>", ActEditorGUI.RichLabel);
		}
#endif
		
	}
}
#endif