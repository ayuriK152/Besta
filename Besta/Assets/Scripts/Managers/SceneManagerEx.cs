using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public Define.Scene currentScene;

    public void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(Define.Scene sceneName)
    {
        SceneManager.LoadScene(GetSceneName(sceneName));
        // System.Enum.TryParse(GetSceneName(sceneName), true, out currentScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        System.Enum.TryParse(SceneManager.GetActiveScene().name, true, out currentScene);
        Managers.UI.UpdateUIScript();
    }

    public string GetSceneName(Define.Scene sceneName)
    {
        return System.Enum.GetName(typeof(Define.Scene), sceneName);
    }
}
