using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(FixedCam))]
public class FixedCamEditor : Editor
{
    FixedCam cam;
    private void OnEnable()
    {
        cam = target as FixedCam;
    }
    private void OnSceneGUI()
    {
        Transform camTransform = cam.transform;
        int handleType = EditorGUIUtility.hotControl;
        Tool currentTool = Tools.current;
        switch (currentTool)
        {
            case Tool.Move:
                Vector3 point = camTransform.TransformPoint(cam.Point);
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(cam, "Moved Camera Point");
                    EditorUtility.SetDirty(cam);
                    cam.Point = camTransform.InverseTransformPoint(point);
                }
                break;
            case Tool.Rotate:
                Quaternion Direction = Quaternion.Euler(cam.LookDirection);
                EditorGUI.BeginChangeCheck();
                Direction = Handles.DoRotationHandle(Direction, camTransform.TransformPoint(cam.Point));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(cam, "Rotated Camera Point");
                    EditorUtility.SetDirty(cam);
                    cam.LookDirection = Direction.eulerAngles;
                }
                break;
            default:
                break;
        }
    }
}
