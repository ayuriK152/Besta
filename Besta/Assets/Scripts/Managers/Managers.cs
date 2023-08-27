using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _managerInstance = null;
    static Managers ManagerInstance { get { return _managerInstance; } }

    private static InputManager _input = new InputManager();
    private static SoundManager _sound = new SoundManager();
    private static DataManager _data = new DataManager();
    private static IngameManager _game = new IngameManager();
    private static SceneManagerEx _scene = new SceneManagerEx();
    private static UIManager _ui = new UIManager();

    public static InputManager Input { get { return _input; } }
    public static SoundManager Sound { get { return _sound; } }
    public static DataManager Data { get { return _data; } }
    public static IngameManager Game { get { return _game; } }
    public static SceneManagerEx Scene { get { return _scene; } }
    public static UIManager UI { get { return _ui; } }

    public static string currentVersion = "alpha 1.0.0";
    public static string currentEditorVersion = "alpha 1.0.0";

    void Awake()
    {
        if (_managerInstance != null)
        {
            Destroy(gameObject);
            return;
        }
        Init();
    }

    void Update()
    {
        Input.OnUpdate();
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

        Sound.Init();
        Scene.Init();
        Data.Init();
        Game.Init();
    }
}
