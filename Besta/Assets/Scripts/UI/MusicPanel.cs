using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Datas;

public class MusicPanel : MonoBehaviour
{
    public string name;
    public string artist;
    public int bpm;

    TextMeshProUGUI nameUI;
    TextMeshProUGUI artistUI;
    Image albumArt;

    MusicSelectUI ui;

    private void Awake()
    {
        nameUI = transform.Find("TextArea").Find("Name").GetComponent<TextMeshProUGUI>();
        artistUI = transform.Find("TextArea").Find("Artist").GetComponent<TextMeshProUGUI>();
        albumArt = transform.Find("Image").GetComponent<Image>();
        ui = Managers.UI.currentSceneUI as MusicSelectUI;
    }

    public void OnClickSelectButton()
    {
        AudioClip tempMusic = Resources.Load<AudioClip>($"Patterns/{name}/music");
        if (Managers.Sound.managerAudioSource.clip == tempMusic)
        {
            Managers.Sound.managerAudioSource.Stop();
            Managers.Game.currentLoadedPattern = Managers.Data.LoadJsonData<MusicPattern>(Resources.Load<TextAsset>($"Patterns/{name}/data"));
            Managers.Game.currentLoadedPattern.ReloadMusic();
            Managers.Sound.managerAudioSource.clip = Managers.Game.currentLoadedPattern.musicSource;
            Managers.Sound.managerAudioSource.timeSamples = 0;
            Managers.Scene.LoadScene(Define.Scene.Ingame);
        }
        else
        {
            if (MusicSelectController.fadeinCoroutine != null)
                StopCoroutine(MusicSelectController.fadeinCoroutine);
            UpdateDetail();
            Managers.Sound.managerAudioSource.clip = tempMusic;
            Managers.Sound.managerAudioSource.time = 60;
            Managers.Sound.managerAudioSource.Play();
            MusicSelectController.fadeinCoroutine = StartCoroutine(MusicSelectController.MusicPreviewFadein());
        }
    }

    public void UpdateInfo()
    {
        nameUI.text = name;
        artistUI.text = artist;
        albumArt.sprite = Resources.Load<Sprite>($"Patterns/{name}/image");
        /*
        Texture2D tempTexture = Managers.Data.LoadImageFile($"{Application.dataPath}/Patterns/{name}/image.png");
        albumArt.sprite = Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(0.5f, 0.5f));
        */
    }

    public void UpdateDetail()
    {
        ui.currentImage.sprite = albumArt.sprite;
        ui.currentName.text = name;
        ui.currentArtist.text = $"Artist : {artist}";
        ui.currentBPM.text = $"BPM : {bpm.ToString()}";
    }
}
