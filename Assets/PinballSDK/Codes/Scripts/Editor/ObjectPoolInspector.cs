using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
[CustomEditor(typeof(ObjectPool))]
public class ObjectPoolInspector : Editor
{
    private ObjectPool poolManager;
    private ReorderableList poolList;

    private void OnEnable()
    {
        poolManager = (ObjectPool)target;
        poolList = new ReorderableList(serializedObject, serializedObject.FindProperty("ObjectPools"), true, true, true, true);
        poolList.drawHeaderCallback = DrawHeader;
        poolList.drawElementCallback = DrawElement;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.Space();
        poolList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    void DrawElement (Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = poolList.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        float widthStep = rect.width / 4;
        float Padding = 10;
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, widthStep - Padding, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Name"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(rect.x + widthStep, rect.y, widthStep - Padding, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Prefab"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(rect.x + widthStep * 2, rect.y, widthStep - Padding, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Amount"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(rect.x + widthStep * 3, rect.y, widthStep - Padding, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Container"), GUIContent.none);
    }

    void DrawHeader (Rect rect)
    {
        float widthStep = rect.width / 4;
        float Padding = 10;
        float posOffset = 5;
        EditorGUI.LabelField(new Rect (rect.x, rect.y, widthStep - Padding, EditorGUIUtility.singleLineHeight), "Name");
        EditorGUI.LabelField(new Rect(rect.x + widthStep, rect.y, widthStep - Padding, EditorGUIUtility.singleLineHeight), "Prefab");
        EditorGUI.LabelField(new Rect(rect.x + widthStep * 2, rect.y, widthStep - Padding, EditorGUIUtility.singleLineHeight), "Amount");
        EditorGUI.LabelField(new Rect(rect.x + widthStep * 3, rect.y, widthStep - Padding, EditorGUIUtility.singleLineHeight), "Holder");
    }
}
