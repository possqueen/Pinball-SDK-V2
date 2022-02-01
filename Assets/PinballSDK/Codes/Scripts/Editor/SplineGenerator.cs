using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pixelplacement;

public class SplineGenerator : EditorWindow
{
    
    public Transform splineHolder;
    public List<Transform> Points;
    ScriptableObject obj;
    SerializedObject sObj;

    [MenuItem("Pinball SDK/Tools/Spline Generator")]
    static void Init()
    {
        SplineGenerator window = (SplineGenerator)EditorWindow.GetWindow(typeof(SplineGenerator), false, "Spline Generator", true);
        window.Show();
    }

    private void OnEnable()
    {
        obj = this;
        sObj = new SerializedObject(obj);
    }

    private void OnGUI()
    {
        sObj.Update();
        EditorGUILayout.PropertyField(sObj.FindProperty("splineHolder"));
        EditorGUILayout.PropertyField(sObj.FindProperty("Points"), true);
        if (splineHolder != null)
        {
            if (GUILayout.Button("Get Points"))
            {
                GetPoints();
            }
        }

        sObj.ApplyModifiedProperties();
        if (Points.Count > 0 && HasPoints())
        {
            if (GUILayout.Button ("Rename Handles"))
            {
                RenameHandles();
            }
            if (GUILayout.Button("Generate Spline"))
            {
                CreateSpline();
            }
        }
    }

    void GetPoints()
    {
        foreach (Transform t in splineHolder)
        {
            if (t.name.Contains("Point"))
            {
                if (!Points.Contains(t)) Points.Add(t);
            }
        }
        sObj.ApplyModifiedProperties();
    }

    void RenameHandles()
    {
        foreach (Transform t in Points)
        {
            foreach (Transform h in t)
            {
                if (h.name.Contains("Left"))
                {
                    h.name = "Left Handle";
                }
                if (h.name.Contains("Right"))
                {
                    h.name = "Right Handle";
                }
            }
        }
    }

    void CreateSpline()
    {
        GameObject splineObject = new GameObject("Generated Spline");
        splineObject.transform.parent = splineHolder;
        Spline spline = splineObject.AddComponent<Spline>();
        spline.AddAnchors(Points.Count - 2);
        for (int i = 0; i < Points.Count; i++)
        {
            Transform inTang = Points[i].Find("Left Handle");
            Transform outTang = Points[i].Find("Right Handle");
            spline.Anchors[i].tangentMode = TangentMode.Aligned;
            spline.Anchors[i].transform.position = Points[i].transform.position;
            spline.Anchors[i].InTangent.position = inTang.position;
            spline.Anchors[i].OutTangent.position = outTang.position;
        }
        Selection.activeGameObject = splineObject;
    }

    bool HasPoints()
    {
        bool full = true;
        for (int i = 0; i < Points.Count; i++)
        {
            if (Points[i] == null)
                full = false;
        }
        return full;
    }
}
