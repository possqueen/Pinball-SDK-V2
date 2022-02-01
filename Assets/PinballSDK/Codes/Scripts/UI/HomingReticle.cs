using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Pinball/UI/Homing Reticle")]
public class HomingReticle : MonoBehaviour
{
    RectTransform canvas;
    public RectTransform reticle;
    public bool Active;

    private void Start()
    {
        canvas = GetComponent<RectTransform>();
    }

    private void Update()
    {
        reticle.gameObject.SetActive(Active);
    }

    public void SetReticle (Vector3 TargetPos)
    {
        Vector2 IconPos = Camera.main.WorldToScreenPoint(TargetPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, IconPos, null, out Vector2 pos);
        reticle.anchoredPosition = pos;
    }
}
