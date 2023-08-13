using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectUI : MonoBehaviour
{
    public Image currentImage;
    public TextMeshProUGUI currentName;
    public TextMeshProUGUI currentArtist;
    public TextMeshProUGUI currentBPM;
    public GameObject optionUIObj;
    public TextMeshProUGUI musicSoundValueSign;
    public Slider musicSoundSlider;

    private void Awake()
    {
        currentImage = transform.Find("MusicDetailPreview").Find("CurrentImage").GetComponent<Image>();
        currentName = transform.Find("MusicDetailPreview").Find("CurrentName").GetComponent<TextMeshProUGUI>();
        currentArtist = transform.Find("MusicDetailPreview").Find("CurrentArtist").GetComponent<TextMeshProUGUI>();
        currentBPM = transform.Find("MusicDetailPreview").Find("CurrentBPM").GetComponent<TextMeshProUGUI>();
        optionUIObj = transform.Find("OptionPanel").gameObject;
        musicSoundValueSign = GameObject.Find("MusicSoundSetting").transform.Find("ValueSign").GetComponent<TextMeshProUGUI>();
        musicSoundSlider = GameObject.Find("MusicSoundSetting").transform.Find("Slider").GetComponent<Slider>();
        musicSoundSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicSoundValue"));
        musicSoundValueSign.text = Math.Truncate(musicSoundSlider.value * 100).ToString();
        optionUIObj.active = false;
    }

    public void OnMusicSoundValueChange()
    {
        PlayerPrefs.SetFloat("MusicSoundValue", ((float)Math.Ceiling(musicSoundSlider.value * 100) * 0.01f));
        musicSoundValueSign.text = Math.Round(PlayerPrefs.GetFloat("MusicSoundValue") * 100).ToString();
        Managers.Sound.managerAudioSource.volume = PlayerPrefs.GetFloat("MusicSoundValue");
    }
}
