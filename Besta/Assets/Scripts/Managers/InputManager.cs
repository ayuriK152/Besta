using System;
using UnityEngine;
using static Define;

public class InputManager
{
    public Action<MouseEvent, MousePointer> MouseAction = null;
    public Action KeyAction = null;
    public Action<MouseScroll> ScrollAction = null;

    bool _pressed = false;
    float _pressedTime = 0f;

    public void OnUpdate()
    {
        /** Ű���� �Է� ���� */
        if (Input.anyKeyDown && KeyAction != null)
        {
            KeyAction.Invoke();
        }

        /** ���콺 �Է� ���� */
        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_pressed)
                {
                    MouseAction.Invoke(MouseEvent.PointerDown, MousePointer.Left);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(MouseEvent.Press, MousePointer.Left);
                _pressed = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_pressed)
                {
                    if (Time.time < _pressedTime + 0.2f)
                        MouseAction.Invoke(MouseEvent.Click, MousePointer.Left);
                    MouseAction.Invoke(MouseEvent.PointerUp, MousePointer.Left);
                }
                _pressed = false;
                _pressedTime = 0f;
            }

            if (Input.GetMouseButtonDown(1))
            {
                MouseAction.Invoke(MouseEvent.PointerDown, MousePointer.Right);
            }
        }

        /** ���콺 ��ũ�� �Է� ���� */
        if (ScrollAction != null)
        {
            if (Input.mouseScrollDelta.y < 0)
            {
                ScrollAction.Invoke(MouseScroll.Down);
            }
            if (Input.mouseScrollDelta.y > 0)
            {
                ScrollAction.Invoke(MouseScroll.Up);
            }
        }
    }

    public void Clear()
    {
        MouseAction = null;
    }
}
