using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [System.Serializable] public class InputAxis
    {
        public string Name;
        public string GamepadAxis;
        public string Positive;
        public string Negative;
        public float KeyboardDamping;
        float dampVelocity;
        [HideInInspector]public float value;
        float TargetAxis = 0;

        public void UpdateAxis()
        {
            //Check if there's gamepad input
            if (Input.GetAxisRaw(GamepadAxis) != 0)
            {
                value = Input.GetAxisRaw(GamepadAxis);
            }
            else
            {
                if (Input.GetKeyDown(Positive)) TargetAxis = 1f;
                if (Input.GetKeyDown(Negative)) TargetAxis = -1f;
                if (!Input.GetKey(Positive) && !Input.GetKey(Negative)) TargetAxis = 0;
                value = Mathf.SmoothDamp(value, TargetAxis, ref dampVelocity, KeyboardDamping);
            }
        }
    }

    [System.Serializable] public class InputButton
    {
        public string Name;
        public string GamepadInput;
        public string KeyboardInput;
    }

    public static InputManager instance;
    public enum InputBehavior { Get, Down, Up }
    public InputAxis[] Axes;
    public InputButton[] Buttons;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        foreach (InputAxis a in Axes)
        {
            a.UpdateAxis();
        }
    }

    public float GetAxis(string name)
    {
        string lName = name.ToLower();
        float value = 0;
        for (int i = 0; i < Axes.Length; i++)
        {
            string targName = Axes[i].Name.ToLower();
            if (targName == lName)
            {
                value = Axes[i].value;
            }
        }

        return value;
    }

    public bool GetButton(string name, InputBehavior inputType)
    {
        bool active = false;
        string lName = name.ToLower();
        for (int i = 0; i < Buttons.Length; i++)
        {
            string targName = Buttons[i].Name.ToLower();
            if (lName == targName)
            {
                switch (inputType)
                {
                    case InputBehavior.Get:
                        bool keyGet = Input.GetKey(Buttons[i].KeyboardInput);
                        bool buttonGet = Input.GetKey(Buttons[i].GamepadInput);
                        active = keyGet || buttonGet;
                        break;
                    case InputBehavior.Down:
                        bool keyDown = Input.GetKeyDown(Buttons[i].KeyboardInput);
                        bool buttonDown = Input.GetKeyDown(Buttons[i].GamepadInput);
                        active = keyDown || buttonDown;
                        break;
                    case InputBehavior.Up:
                        bool keyUp = Input.GetKeyUp(Buttons[i].KeyboardInput);
                        bool buttonUp = Input.GetKeyUp(Buttons[i].GamepadInput);
                        active = keyUp || buttonUp;
                        break;
                }
            }
        }
        return active;
    }
}
