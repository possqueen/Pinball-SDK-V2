using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pixelplacement;
public class SplineObjectSpawner : EditorWindow
{
    private ScriptableObject scriptable;
    private SerializedObject spawner;
    public Spline spline;
    public GameObject obj;
    public int Count;

    [MenuItem("Pinball SDK/Tools/Spline Object Spawner")]
    static void Init()
    {
        SplineObjectSpawner window = (SplineObjectSpawner)EditorWindow.GetWindow(typeof(SplineObjectSpawner), false, "Spline Object Spawner", true);
        window.Show();
    }

    private void OnEnable()
    {
        scriptable = this;
        spawner = new SerializedObject(scriptable);
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("This tool spawns a set count of items along a spline.", MessageType.Info);
        spawner.Update();
        EditorGUILayout.PropertyField(spawner.FindProperty("spline"));
        EditorGUILayout.PropertyField(spawner.FindProperty("obj"));
        EditorGUILayout.PropertyField(spawner.FindProperty("Count"));
        spawner.ApplyModifiedProperties();
        if (!spline)
        {
            EditorGUILayout.HelpBox("No spline has been assigned.", MessageType.Error);
        }

        if (!obj)
        {
            EditorGUILayout.HelpBox("No prefab has been assigned.", MessageType.Error);
        }

        if (spline && obj)
        {
            if (Count > 0)
            {
                if (GUILayout.Button("Spawn"))
                {
                    SpawnObjects();
                }
            } else
            {
                EditorGUILayout.HelpBox("Cannot spawn 0 prefabs.", MessageType.Error);
            }
        }
    }

    void SpawnObjects()
    {
        spline.CalculateLength();
        float length = spline.Length;
        GameObject parent = new GameObject("Spline Ring Group");
        parent.transform.position = spline.transform.position;
        for (int i = 0; i < Count; i++)
        {
            GameObject p = (GameObject)PrefabUtility.InstantiatePrefab(obj);
            string name = obj.name + " " + i.ToString();
            p.name = name;
            Transform objTransform = p.transform;
            float point = (1f / Count) * i;
            objTransform.position = spline.GetPosition(point);
            objTransform.forward = spline.GetDirection(point);
            objTransform.parent = parent.transform;
            Debug.Log("Spawned " + name + " at spline percentage " + point);
            Undo.RecordObject(p, "Spawned Object");
            EditorUtility.SetDirty(p);
        }
    }


}
