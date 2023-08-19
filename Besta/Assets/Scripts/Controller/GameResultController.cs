using UnityEngine;
using static Define;

public class GameResultController : MonoBehaviour
{
    void Start()
    {
        Managers.Input.KeyDownAction = null;
        Managers.Input.KeyDownAction -= PlayerKeyDown;
        Managers.Input.KeyDownAction += PlayerKeyDown;
        GameController.isPlaying = false;
    }

    void PlayerKeyDown(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Return:
                Managers.Game.Init();
                Managers.Scene.LoadScene(Scene.MusicSelect);
                break;
        }
    }
}
