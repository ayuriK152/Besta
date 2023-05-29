using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        Manager.Input.MouseAction += MouseEventTestMethod;
    }

    void Update()
    {
        
    }

    void MouseEventTestMethod(MouseEvent mouseEvent, MousePointer mousePointer)
    {
        Debug.Log(mousePointer.ToString() + mouseEvent.ToString());
    }
}
