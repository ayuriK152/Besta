using TMPro;
using UnityEngine;

public class UIManager
{
    public object currentSceneUI;

    public void UpdateUIScript()
    {
        GameObject go = GameObject.Find("BaseCanvas");
        switch (Managers.Scene.currentScene)
        {
            case Define.Scene.PatternEditor:
                currentSceneUI = go.GetComponent<EditorUI>();
                Debug.Log("PatternEditor UI Loaded");
                break;
            case Define.Scene.Ingame:
                currentSceneUI = go.GetComponent<IngameUI>();
                Debug.Log("Ingame UI Loaded");
                break;
            case Define.Scene.MusicSelect:
                currentSceneUI = go.GetComponent<MusicSelectUI>();
                Debug.Log("MusicSelect UI Loaded");
                break;
            case Define.Scene.GameResult:
                currentSceneUI = go.GetComponent<GameResultUI>();
                Debug.Log("CurrentScene UI Loaded");
                break;
        }

        TextMeshProUGUI versionText = GameObject.Find("Version").GetComponent<TextMeshProUGUI>();
        if (Managers.Scene.currentScene != Define.Scene.PatternEditor)
        {
            if (versionText != null)
                versionText.text = Managers.currentVersion;
        }
        else
        {
            if (versionText != null)
                versionText.text = Managers.currentEditorVersion;
        }
    }
}
