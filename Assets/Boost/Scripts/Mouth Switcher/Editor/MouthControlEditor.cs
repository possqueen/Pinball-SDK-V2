using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MouthController))]
public class MouthControlEditor : Editor
{
    MouthController m;
    SerializedObject t;
    private void OnEnable()
    {
        m = target as MouthController;
        t = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Mouth Switcher for ripped Sonic Generations model. \n" +
            "Place this script anywhere on your player object and assign the model's Head bone \n" +
            "to automatically assign the proper Mouth bones, or choose to assign them manually.", MessageType.Info);
        if (!m.ManuallyAssign)
        {
            if (m.Head == null)
                EditorGUILayout.HelpBox("Head transform has not been assigned, cannot assign mouth transforms.", MessageType.Error);
        } else
        {
            if (m.Head == null)
                EditorGUILayout.HelpBox("Missing transform: Head", MessageType.Error);
            if (m.MouthRight == null)
                EditorGUILayout.HelpBox("Missing transform: MouthReference", MessageType.Error);
            if (m.MouthLeft == null)
                EditorGUILayout.HelpBox("Missing transform: MouthReference_L", MessageType.Error);
            if (m.camera == null)
                EditorGUILayout.HelpBox("Missing transform: Camera", MessageType.Error);
        }
        EditorGUILayout.PropertyField(t.FindProperty("ManuallyAssign"));
        EditorGUILayout.PropertyField(t.FindProperty("Head"));

        if (m.ManuallyAssign)
        {
            EditorGUILayout.PropertyField(t.FindProperty("MouthRight"));
            EditorGUILayout.PropertyField(t.FindProperty("MouthLeft"));
            EditorGUILayout.PropertyField(t.FindProperty("camera"));
        }
        t.ApplyModifiedProperties();
        t.Update();
    }
}
