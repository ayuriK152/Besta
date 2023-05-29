using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    static Manager _managerInstance;
    static Manager ManagerInstance { get { Init(); return _managerInstance; } }

    private static InputManager _input = new InputManager();
    public static InputManager Input { get { return _input; } set { _input = value; } }
    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if (_managerInstance == null)
        {
            GameObject go = GameObject.Find("@Manager");
            if (go == null)
            {
                go = new GameObject { name = "@Manager" };
                go.AddComponent<Manager>();
            }

            DontDestroyOnLoad(go);
            _managerInstance = go.GetComponent<Manager>();
        }
    }
}
