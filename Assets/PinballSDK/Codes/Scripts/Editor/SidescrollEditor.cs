using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SidescrollTrigger))]
public class SidescrollEditor : Editor
{
    SidescrollTrigger trig;

    private void OnEnable()
    {
        trig = target as SidescrollTrigger;
    }

    private void OnSceneGUI()
    {
        Transform trans = trig.transform;
        Quaternion rotation = Quaternion.Euler(trig.PlaneRotation);
        if (trig.ShowRotationGizmo)
        {
            EditorGUI.BeginChangeCheck();
            rotation = Handles.DoRotationHandle(rotation, trans.position);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(trig, "Changed 2D Plane Rotation");
                EditorUtility.SetDirty(trig);
                trig.PlaneRotation = rotation.eulerAngles;
            }
        }
    }
}
