using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _managerInstance;
    static Managers ManagerInstance { get { Init(); return _managerInstance; } }

    private static InputManager _input = new InputManager();
    private static SoundManager _sound = new SoundManager();
    private static DataManager _data = new DataManager();
    private static IngameManager _game = new IngameManager();

    public static InputManager Input { get { return _input; } }
    public static SoundManager Sound { get { return _sound; } }
    public static DataManager Data { get { return _data; } }
    public static IngameManager Game { get { return _game; } }

    void Awake()
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
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            _managerInstance = go.GetComponent<Managers>();
        }

        _sound.Init();
    }
}
