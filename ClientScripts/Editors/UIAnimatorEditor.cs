using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIAnimator)),CanEditMultipleObjects]
public class UIAnimatorEditor : Editor
{
    SerializedProperty playstart;
    SerializedProperty startDelay;

    SerializedProperty onCloseToAnimatorClose;
    SerializedProperty onCloseToAnimatorOpen;

    SerializedProperty onOpenToAnimatorClose;
    SerializedProperty onOpenToAnimatorOpen;

    GUIStyle foldoutStyle;

    private void OnEnable()
    {
        playstart = serializedObject.FindProperty("playOnStart");
        startDelay = serializedObject.FindProperty("startDelay");

        onCloseToAnimatorClose = serializedObject.FindProperty("onCloseToAnimatorClose");
        onCloseToAnimatorOpen = serializedObject.FindProperty("onCloseToAnimatorOpen");

        onOpenToAnimatorClose = serializedObject.FindProperty("onOpenToAnimatorClose");
        onOpenToAnimatorOpen = serializedObject.FindProperty("onOpenToAnimatorOpen");

        foldoutStyle = new GUIStyle();
        foldoutStyle.fontStyle = FontStyle.Bold;
        foldoutStyle.margin = new RectOffset(10, 0, 5, 0);
    }

    bool closeFoldout = true;
    bool openFoldout = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);


        EditorGUILayout.PropertyField(playstart, new GUIContent("Play Animation After Delay"));

        if (playstart.boolValue)
            EditorGUILayout.PropertyField(startDelay, new GUIContent("Delay Before Animation"));

        closeFoldout = EditorGUILayout.Foldout(closeFoldout, "When Close is Complete:", foldoutStyle);
        if (closeFoldout)
        {
            EditorGUILayout.PropertyField(onCloseToAnimatorClose, new GUIContent("Close Chained UI"));
            EditorGUILayout.PropertyField(onCloseToAnimatorOpen, new GUIContent("Open Chained UI"));
        }

        openFoldout = EditorGUILayout.Foldout(openFoldout, "When Open is Complete:", foldoutStyle);
        if (openFoldout)
        {
            EditorGUILayout.PropertyField(onOpenToAnimatorOpen, new GUIContent("Open Chained UI"));
            EditorGUILayout.PropertyField(onOpenToAnimatorClose, new GUIContent("Close Chained UI"));
        }

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}
