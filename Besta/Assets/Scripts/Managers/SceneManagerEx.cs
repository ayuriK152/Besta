using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public Define.Scene currentScene;

    public void Init()
    {
        System.Enum.TryParse(SceneManager.GetActiveScene().name, true, out currentScene);
        Managers.UI.UpdateUIScript();
    }

    public void LoadScene(Define.Scene sceneName)
    {
        SceneManager.LoadScene(GetSceneName(sceneName));
        System.Enum.TryParse(SceneManager.GetActiveScene().name, true, out currentScene);
        Managers.UI.UpdateUIScript();
    }

    public string GetSceneName(Define.Scene sceneName)
    {
        return System.Enum.GetName(typeof(Define.Scene), sceneName);
    }
}
