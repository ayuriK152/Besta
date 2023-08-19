using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class InputManager
{
    public Action<MouseEvent, MousePointer> MouseAction = null;
    public Action <KeyCode>KeyDownAction= null;
    public Action<KeyCode> KeyPressAction = null;
    public Action<KeyCode> KeyUpAction = null;
    public Action<MouseScroll> ScrollAction = null;

    bool _pressed = false;
    float _pressedTime = 0f;

    public void OnUpdate()
    {
        /** 키보드 입력 관리 */
        if (KeyDownAction != null)
        {
            if (Input.GetKeyDown(KeyCode.S))
                KeyDownAction.Invoke(KeyCode.S);

            if (Input.GetKeyDown(KeyCode.D))
                KeyDownAction.Invoke(KeyCode.D);

            if (Input.GetKeyDown(KeyCode.L))
                KeyDownAction.Invoke(KeyCode.L);

            if (Input.GetKeyDown(KeyCode.Semicolon))
                KeyDownAction.Invoke(KeyCode.Semicolon);

            if (Input.GetKeyDown(KeyCode.F3))
                KeyDownAction.Invoke(KeyCode.F3);

            if (Input.GetKeyDown(KeyCode.Escape))
                KeyDownAction.Invoke(KeyCode.Escape);

            if (Input.GetKeyDown(KeyCode.Return))       // 엔터
                KeyDownAction.Invoke(KeyCode.Return);
        }

        if (KeyPressAction != null)
        {
            if (Input.GetKey(KeyCode.S))
            {
                KeyPressAction.Invoke(KeyCode.S);
            }
            if (Input.GetKey(KeyCode.D))
            {
                KeyPressAction.Invoke(KeyCode.D);
            }
            if (Input.GetKey(KeyCode.L))
            {
                KeyPressAction.Invoke(KeyCode.L);
            }
            if (Input.GetKey(KeyCode.Semicolon))
            {
                KeyPressAction.Invoke(KeyCode.Semicolon);
            }
        }

        if (KeyUpAction != null)
        {
            if (Input.GetKeyUp(KeyCode.S))
            {
                KeyUpAction.Invoke(KeyCode.S);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                KeyUpAction.Invoke(KeyCode.D);
            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                KeyUpAction.Invoke(KeyCode.L);
            }
            if (Input.GetKeyUp(KeyCode.Semicolon))
            {
                KeyUpAction.Invoke(KeyCode.Semicolon);
            }
        }

        /** 마우스 입력 관리 */
        if (MouseAction != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())      // UI 클릭
                return;

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

        /** 마우스 스크롤 입력 관리 */
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
