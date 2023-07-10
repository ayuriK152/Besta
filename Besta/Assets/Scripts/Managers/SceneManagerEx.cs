using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public void LoadScene(Define.Scene sceneName)
    {
        SceneManager.LoadScene(GetSceneName(sceneName));
    }

    public string GetSceneName(Define.Scene sceneName)
    {
        return System.Enum.GetName(typeof(Define.Scene), sceneName);
    }
}
