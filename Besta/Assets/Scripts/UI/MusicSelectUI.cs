using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class MusicSelectUI : MonoBehaviour
{
    public Image currentImage;
    public TextMeshProUGUI currentName;
    public TextMeshProUGUI currentArtist;
    public TextMeshProUGUI currentBPM;
    public GameObject optionUIObj;
    public GameObject exitPanelObj;
    public TextMeshProUGUI musicSoundValueText;
    public Slider musicSoundSlider;
    public TextMeshProUGUI effectSoundValueText;
    public Slider effectSoundSlider;
    public TextMeshProUGUI userOffsetValueText;
    public Slider userOffsetSlider;
    public TextMeshProUGUI judgeLineHeightValueText;
    public Slider judgeLineHeightSlider;
    public TextMeshProUGUI notePositionValueText;
    public Slider notePositionSlider;
    public TextMeshProUGUI slideSpeedValueText;
    public Slider slideSpeedSlider;
    TMP_Dropdown screenResolutionDropdown;
    TMP_Dropdown fullScreenDropdown;

    private void Awake()
    {
        currentImage = transform.Find("MusicDetailPreview/CurrentImage").GetComponent<Image>();
        currentName = transform.Find("MusicDetailPreview/CurrentName").GetComponent<TextMeshProUGUI>();
        currentArtist = transform.Find("MusicDetailPreview/CurrentArtist").GetComponent<TextMeshProUGUI>();
        currentBPM = transform.Find("MusicDetailPreview/CurrentBPM").GetComponent<TextMeshProUGUI>();

        optionUIObj = transform.Find("OptionPanel").gameObject;
        exitPanelObj = transform.Find("ExitPanel").gameObject;

        musicSoundValueText = GameObject.Find("MusicSoundSetting").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        musicSoundSlider = GameObject.Find("MusicSoundSetting").transform.Find("Slider").GetComponent<Slider>();
        musicSoundSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("MusicSoundValue"));
        musicSoundValueText.text = $"{PlayerPrefs.GetInt("MusicSoundValue")}";

        effectSoundValueText = GameObject.Find("EffectSoundSetting").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        effectSoundSlider = GameObject.Find("EffectSoundSetting").transform.Find("Slider").GetComponent<Slider>();
        effectSoundSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("EffectSoundValue"));
        effectSoundValueText.text = $"{PlayerPrefs.GetInt("EffectSoundValue")}";

        userOffsetValueText = GameObject.Find("UserOffsetSetting").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        userOffsetSlider = GameObject.Find("UserOffsetSetting").transform.Find("Slider").GetComponent<Slider>();
        userOffsetSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("UserOffset"));
        userOffsetValueText.text = $"{PlayerPrefs.GetInt("UserOffset")}ms";

        judgeLineHeightValueText = GameObject.Find("JudgeLineHeightSetting").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        judgeLineHeightSlider = GameObject.Find("JudgeLineHeightSetting").transform.Find("Slider").GetComponent<Slider>();
        judgeLineHeightSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("JudgeLineHeight"));
        if (judgeLineHeightSlider.value > 0)
            judgeLineHeightValueText.text = $"+{PlayerPrefs.GetInt("JudgeLineHeight")}";
        else
            judgeLineHeightValueText.text = $"{PlayerPrefs.GetInt("JudgeLineHeight")}";

        notePositionValueText = GameObject.Find("NotePositionSetting").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        notePositionSlider = GameObject.Find("NotePositionSetting").transform.Find("Slider").GetComponent<Slider>();
        notePositionSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("NotePosition"));
        if (notePositionSlider.value > 0)
            notePositionValueText.text = $"+{PlayerPrefs.GetInt("NotePosition")}";
        else
            notePositionValueText.text = $"{PlayerPrefs.GetInt("NotePosition")}";

        slideSpeedValueText = GameObject.Find("SlideSpeedSetting").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        slideSpeedSlider = GameObject.Find("SlideSpeedSetting").transform.Find("Slider").GetComponent<Slider>();
        slideSpeedSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("SlideSpeed"));
        slideSpeedValueText.text = $"{(int)slideSpeedSlider.value / 10}.{slideSpeedSlider.value % 10}";

        screenResolutionDropdown = GameObject.Find("ScreenResolutionSetting").transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        fullScreenDropdown = GameObject.Find("FullScreenSetting").transform.Find("Dropdown").GetComponent<TMP_Dropdown>();

        optionUIObj.SetActive(false);
        exitPanelObj.SetActive(false);
    }

    public void OnMusicSoundValueChange()
    {
        PlayerPrefs.SetInt("MusicSoundValue", (int)musicSoundSlider.value);
        musicSoundValueText.text = PlayerPrefs.GetInt("MusicSoundValue").ToString();
        Managers.Sound.managerAudioSource.volume = PlayerPrefs.GetInt("MusicSoundValue") / 100.0f;
    }

    public void OnEffectSoundValueChange()
    {
        PlayerPrefs.SetInt("EffectSoundValue", (int)effectSoundSlider.value);
        effectSoundValueText.text = PlayerPrefs.GetInt("EffectSoundValue").ToString();
    }

    public void OnUserOffsetValueChange()
    {
        PlayerPrefs.SetInt("UserOffset", (int)userOffsetSlider.value);
        userOffsetValueText.text = $"{PlayerPrefs.GetInt("UserOffset")}ms";
    }

    public void OnJudgeLineHeightValueChange()
    {
        PlayerPrefs.SetInt("JudgeLineHeight", (int)judgeLineHeightSlider.value);
        if (judgeLineHeightSlider.value > 0)
            judgeLineHeightValueText.text = $"+{PlayerPrefs.GetInt("JudgeLineHeight")}";
        else
            judgeLineHeightValueText.text = $"{PlayerPrefs.GetInt("JudgeLineHeight")}";
    }

    public void OnNotePositionValueChange()
    {
        PlayerPrefs.SetInt("NotePosition", (int)notePositionSlider.value);
        if (notePositionSlider.value > 0)
            notePositionValueText.text = $"+{PlayerPrefs.GetInt("NotePosition")}";
        else
            notePositionValueText.text = $"{PlayerPrefs.GetInt("NotePosition")}";
    }

    public void OnSlideSpeedValueChange()
    {
        PlayerPrefs.SetInt("SlideSpeed", (int)slideSpeedSlider.value);
        slideSpeedValueText.text = $"{(int)slideSpeedSlider.value / 10}.{slideSpeedSlider.value % 10}";
    }

    public void OnChangeScreenResolution()
    {
        switch (screenResolutionDropdown.value)
        {
            case (int)ScreenResolution._1920x1080:
                Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
                break;
            case (int)ScreenResolution._1600x900:
                Screen.SetResolution(1600, 900, Screen.fullScreenMode);
                break;
            case (int)ScreenResolution._1440x810:
                Screen.SetResolution(1440, 810, Screen.fullScreenMode);
                break;
            case (int)ScreenResolution._1366x768:
                Screen.SetResolution(1366, 768, Screen.fullScreenMode);
                break;
            case (int)ScreenResolution._1280x720:
                Screen.SetResolution(1280, 720, Screen.fullScreenMode);
                break;
            case (int)ScreenResolution._1024x576:
                Screen.SetResolution(1024, 576, Screen.fullScreenMode);
                break;
        }
    }

    public void OnChangeFullScreenMode()
    {
        switch (fullScreenDropdown.value)
        {
            case (int)Define.FullScreenMode.Fullscreen:
                Screen.fullScreenMode = UnityEngine.FullScreenMode.FullScreenWindow;
                break;
            case (int)Define.FullScreenMode.Exclusive:
                Screen.fullScreenMode = UnityEngine.FullScreenMode.ExclusiveFullScreen;
                break;
            case (int)Define.FullScreenMode.Windowed:
                Screen.fullScreenMode = UnityEngine.FullScreenMode.Windowed;
                break;
        }
    }
}
