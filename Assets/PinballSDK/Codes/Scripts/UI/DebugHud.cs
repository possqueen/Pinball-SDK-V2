using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DebugHud : MonoBehaviour
{
    [Header("Player Components")]
    public InputManager input;
    public PlayerController controller;
    public PlayerActions actions;

    [Header("Input Variables")]
    public RectTransform InputHandle;
    Vector2 FieldSize = new Vector2(100, 100);
    public float Padding = 0.2f;

    private void Update()
    {
        Vector2 Input = new Vector2(input.GetAxis("Horizontal"), input.GetAxis("Vertical"));
        Input = Vector2.ClampMagnitude(Input, 1f);
        Vector2 Pos = Input * FieldSize * Padding;
        InputHandle.anchoredPosition = Pos;
    }
}
