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
            musicList.Add(Instantiate(musicPanelPrefab, musicListInstantiateObject.transform).GetComponent<MusicPanel>());
            musicList[musicList.Count - 1].name = p.name;
            musicList[musicList.Count - 1].artist = p.artist;
            musicList[musicList.Count - 1].bpm = p.bpm;
            musicList[musicList.Count - 1].UpdateInfo();
        }
        musicList[0].OnClickSelectButton();

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
            Managers.Sound.managerAudioSource.volume = Mathf.Lerp(0, PlayerPrefs.GetFloat("MusicSoundValue"), currentTime / 1);
            yield return null;
        }
        yield break;
    }

    void PlayerKeyDown(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.F3:
                if (!ui.optionUIObj.activeSelf)
                    ui.optionUIObj.active = true;
                break;
            case KeyCode.Escape:
                if (ui.optionUIObj.activeSelf)
                    ui.optionUIObj.active = false;
                break;
        }
    }
}
