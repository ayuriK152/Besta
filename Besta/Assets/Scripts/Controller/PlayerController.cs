using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Start()
    {
        Managers.Input.KeyAction -= PlayerKeyDown;
        Managers.Input.KeyAction += PlayerKeyDown;
    }

    void Update()
    {
        
    }

    public void PlayerKeyDown(KeyCode key)
    {
        Debug.Log(key.ToString());
    }
}
