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

    void Start()
    {
        musicListInstantiateObject = GameObject.Find("Content");
        musicPanelPrefab = Resources.Load<GameObject>("Prefabs/MusicPanel");
        Object[] datas = Resources.LoadAll("Patterns", typeof(TextAsset));
        List<MusicPattern> tempPatternList = new List<MusicPattern>();
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
    }

    public static IEnumerator MusicPreviewFadein()
    {
        float currentTime = 0;
        while (currentTime < 1)
        {
            currentTime += Time.deltaTime;
            Managers.Sound.managerAudioSource.volume = Mathf.Lerp(0, 0.3f, currentTime / 1);
            yield return null;
        }
        yield break;
    }
}
