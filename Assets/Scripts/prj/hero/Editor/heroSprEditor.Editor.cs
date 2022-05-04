using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(heroSprEditor))]
public class heroSprEditorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        heroSprEditor panel = (heroSprEditor)target;

        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        
        if (panel.gameObject.activeSelf && GUILayout.Button("create sprite"))
            panel.MakeSprite();

        if (panel.gameObject.activeSelf && GUILayout.Button("create aim positions"))
            panel.MakeAimPoses();

        GUILayout.EndHorizontal();
    }
}
