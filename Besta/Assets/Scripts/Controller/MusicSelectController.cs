using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Datas;

public class MusicSelectController : MonoBehaviour
{
    GameObject musicListInstantiateObject;
    GameObject musicPanelPrefab;
    List<MusicPanel> musicList = new List<MusicPanel>();

    public static Coroutine fadeinCoroutine;

    MusicSelectUI ui;

    void Start()
    {
        musicListInstantiateObject = GameObject.Find("Content");
        musicPanelPrefab = Resources.Load<GameObject>("Prefabs/MusicPanel");
        Object[] datas = Resources.LoadAll("Patterns", typeof(TextAsset));
        List<MusicPattern> tempPatternList = new List<MusicPattern>();

        ui = Managers.UI.currentSceneUI as MusicSelectUI;

        foreach (Object d in datas)
            tempPatternList.Add(Managers.Data.LoadJsonData<MusicPattern>(d as TextAsset));
        foreach (MusicPattern p in tempPatternList)
        {
            GameObject panelObject = Instantiate(musicPanelPrefab, musicListInstantiateObject.transform);
            panelObject.name = $"{musicList.Count}-{p.name}";
            musicList.Add(panelObject.GetComponent<MusicPanel>());
            musicList[musicList.Count - 1].name = p.name;
            musicList[musicList.Count - 1].artist = p.artist;
            musicList[musicList.Count - 1].bpm = p.bpm;
            musicList[musicList.Count - 1].UpdateInfo();
        }
        Managers.Sound.managerAudioSource.clip = null;
        musicList[PlayerPrefs.GetInt("LastSelectMusic")].OnClickSelectButton();

        Managers.Input.KeyDownAction = null;
        Managers.Input.KeyDownAction -= PlayerKeyDown;
        Managers.Input.KeyDownAction += PlayerKeyDown;
    }

    public static IEnumerator MusicPreviewFadein()
    {
        float currentTime = 0;
        while (currentTime < 1)
        {
            currentTime += Time.deltaTime;
            Managers.Sound.managerAudioSource.volume = Mathf.Lerp(0, PlayerPrefs.GetInt("MusicSoundValue") / 100.0f, currentTime / 1);
            yield return null;
        }
        yield break;
    }

    void PlayerKeyDown(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.F3:
                if (!ui.exitPanelObj.activeSelf)
                {
                    if (!ui.optionUIObj.activeSelf)
                        ui.optionUIObj.SetActive(true);
                }
                break;
            case KeyCode.Escape:
                if (ui.optionUIObj.activeSelf)
                    ui.optionUIObj.SetActive(false);
                else if (!ui.exitPanelObj.activeSelf)
                    ui.exitPanelObj.SetActive(true);
                else if (ui.exitPanelObj.activeSelf)
                    ui.exitPanelObj.SetActive(false);
                break;
            case KeyCode.Return:
                if (ui.exitPanelObj.activeSelf)
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN
                    Application.Quit();
#endif
                break;
        }
    }
}
