using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[ExecuteInEditMode]
public class SidescrollTrigger : MonoBehaviour
{
    BoxCollider col;
    public Vector3 PlaneRotation;
    public bool ShowRotationGizmo;
    public bool ShowTrigger;
    public Color triggerColor;
    [HideInInspector] public Vector3 X_Axis;
    [HideInInspector] public Vector3 Y_Axis;
    [HideInInspector] public Vector3 Z_Axis;
    Quaternion AxisRotation;
    [Range(5f, 10f)]public float AxisLineThickness = 5f;

    private void Update()
    {
        col = GetComponent<BoxCollider>();
        AxisRotation = Quaternion.Euler(PlaneRotation);
        X_Axis = AxisRotation * Vector3.right;
        Y_Axis = AxisRotation * Vector3.up;
        Z_Axis = AxisRotation * Vector3.forward;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, new Vector3(0.1f, 0.1f, 0.1f));
        Handles.color = Color.red;
        Handles.DrawLine(transform.position, transform.position + X_Axis, AxisLineThickness);
        Handles.color = Color.green;
        Handles.DrawLine(transform.position, transform.position + Y_Axis, AxisLineThickness);
        Handles.color = Color.blue;
        Handles.DrawLine(transform.position, transform.position + Z_Axis, AxisLineThickness);

        if (ShowTrigger)
        {
            Gizmos.color = triggerColor;
            Gizmos.DrawCube(transform.position + col.center, col.size);
        }
    }
}
