using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

[AddComponentMenu("UI/ButtonEvent", 30)]
public class ButtonEvent : MonoBehaviour
{
    public event Action Down;
    public event Action Drag;
    public event Action Up;
    public Color ColorDown;
    public Key Key = Key.None;
    private Vector2 MinPosition, MaxPosition;
    private Color ColorUp;
    private bool IsTouching = false;
    public RectTransform RectTransform;
    private Image Image;
    private void OnDown()
    {
        if (ColorDown.a != 0)
            Image.color = ColorDown;
        Down?.Invoke();
    }

    private void OnDrag()
    {
        Drag?.Invoke();
    }

    private void OnUp()
    {
        Image.color = ColorUp;
        Up?.Invoke();
    }

    void Start()
    {
        if (RectTransform == null)
            RectTransform = GetComponent<RectTransform>();
        Image = GetComponent<Image>();
        ColorUp = Image.color;
    }

    private void OnDisable()
    {
        if (Image != null)
            Image.color = ColorUp;
    }

    void Update()
    {
        MinPosition = (Vector2)RectTransform.position + RectTransform.rect.min;
        MaxPosition = (Vector2)RectTransform.position + RectTransform.rect.max;

        if (Application.platform == RuntimePlatform.Android) Sensor();
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer) { MouseInput(); KeyboardInput(); }
    }

    private void KeyboardInput()
    {
        if (Key != Key.None)
        {
            var KeyClicked = Keyboard.current[Key];
            if (KeyClicked.wasPressedThisFrame) OnDown();
            else if (KeyClicked.isPressed) OnDrag();
            else if (KeyClicked.wasReleasedThisFrame) OnUp();
        }
    }

    private void MouseInput()
    {
        var mousePosition = Mouse.current.position.ReadValue();

        if (IsMouseover(mousePosition))
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) { OnDown(); IsTouching = true; }
            else if (Mouse.current.leftButton.isPressed && IsTouching) OnDrag();
            else if (Mouse.current.leftButton.wasReleasedThisFrame) { OnUp(); IsTouching = false; }
        }
        else if (IsTouching)
        {
            IsTouching = false;
            OnUp();
        }
    }

    private void Sensor()
    {
        var touches = Touchscreen.current.touches;
        TouchControl touch = null;
        for (int i = 0; i < touches.Count; i++)
        {
            if (IsMouseover(new Vector2(touches[i].position.x.ReadValue(), touches[i].position.y.ReadValue())))
            {
                touch = touches[i];
                break;
            }
        }
        if (touch != null)
        {
            var touchActual = touch.ReadValue();
            switch (touchActual.phase)
            {
                case TouchPhase.Began:
                    if (!IsTouching)
                    {
                        IsTouching = true;
                        OnDown();
                    }
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (IsTouching)
                        OnDrag();
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    IsTouching = false;
                    OnUp();
                    break;
            }
        }
        else
        {
            if (IsTouching)
            {
                IsTouching = false;
                OnUp();
            }
        }
    }

    public bool IsMouseover(Vector3 position)
    {
        return position.x > MinPosition.x && position.y > MinPosition.y &&
            position.x < MaxPosition.x && position.y < MaxPosition.y;
    }
}
