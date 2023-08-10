using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Datas;

public class MusicSelectUI : MonoBehaviour
{
    public void OnTempLoadClick()
    {
#if UNITY_EDITOR
        Managers.Game.currentLoadedPattern = Managers.Data.LoadJsonData<MusicPattern>(EditorUtility.OpenFilePanel("Choose music pattern","","json"));
#elif UNITY_STANDALONE_WIN
        Managers.Game.currentLoadedPattern = Managers.Data.LoadJsonData<MusicPattern>(StandaloneFileBrowser.OpenFilePanel("Choose music pattern","","json", false)[0]);
#endif
        Managers.Game.currentLoadedPattern.ReloadMusic();
        Managers.Sound.managerAudioSource.clip = Managers.Game.currentLoadedPattern.musicSource;
        Managers.Scene.LoadScene(Define.Scene.Ingame);
    }
}
