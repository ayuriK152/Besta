using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class MusicSelectOptionUI : MonoBehaviour
{
    TextMeshProUGUI musicSoundValueText;
    Slider musicSoundSlider;
    TextMeshProUGUI effectSoundValueText;
    Slider effectSoundSlider;
    TextMeshProUGUI userOffsetValueText;
    Slider userOffsetSlider;
    TextMeshProUGUI judgeLineHeightValueText;
    Slider judgeLineHeightSlider;
    TextMeshProUGUI notePositionValueText;
    Slider notePositionSlider;
    TextMeshProUGUI slideSpeedValueText;
    Slider slideSpeedSlider;
    TMP_Dropdown screenResolutionDropdown;
    TMP_Dropdown fullScreenDropdown;

    string contentPath;

    private void Awake()
    {
        contentPath = "ScrollView/Viewport/Content";
        musicSoundValueText = transform.Find($"{contentPath}/SoundSettings/Area/MusicSoundSetting/Value").GetComponent<TextMeshProUGUI>();
        musicSoundSlider = transform.Find($"{contentPath}/SoundSettings/Area/MusicSoundSetting/Slider").GetComponent<Slider>();
        musicSoundSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("MusicSoundValue"));
        musicSoundValueText.text = $"{PlayerPrefs.GetInt("MusicSoundValue")}";

        effectSoundValueText = transform.Find($"{contentPath}/SoundSettings/Area/EffectSoundSetting/Value").GetComponent<TextMeshProUGUI>();
        effectSoundSlider = transform.Find($"{contentPath}/SoundSettings/Area/EffectSoundSetting/Slider").GetComponent<Slider>();
        effectSoundSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("EffectSoundValue"));
        effectSoundValueText.text = $"{PlayerPrefs.GetInt("EffectSoundValue")}";

        userOffsetValueText = transform.Find($"{contentPath}/GameSettings/Area/UserOffsetSetting/Value").GetComponent<TextMeshProUGUI>();
        userOffsetSlider = transform.Find($"{contentPath}/GameSettings/Area/UserOffsetSetting/Slider").GetComponent<Slider>();
        userOffsetSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("UserOffset"));
        userOffsetValueText.text = $"{PlayerPrefs.GetInt("UserOffset")}ms";

        judgeLineHeightValueText = transform.Find($"{contentPath}/GameSettings/Area/JudgeLineHeightSetting/Value").GetComponent<TextMeshProUGUI>();
        judgeLineHeightSlider = transform.Find($"{contentPath}/GameSettings/Area/JudgeLineHeightSetting/Slider").GetComponent<Slider>();
        judgeLineHeightSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("JudgeLineHeight"));
        if (judgeLineHeightSlider.value > 0)
            judgeLineHeightValueText.text = $"+{PlayerPrefs.GetInt("JudgeLineHeight")}";
        else
            judgeLineHeightValueText.text = $"{PlayerPrefs.GetInt("JudgeLineHeight")}";

        notePositionValueText = transform.Find($"{contentPath}/GameSettings/Area/NotePositionSetting/Value").GetComponent<TextMeshProUGUI>();
        notePositionSlider = transform.Find($"{contentPath}/GameSettings/Area/NotePositionSetting/Slider").GetComponent<Slider>();
        notePositionSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("NotePosition"));
        if (notePositionSlider.value > 0)
            notePositionValueText.text = $"+{PlayerPrefs.GetInt("NotePosition")}";
        else
            notePositionValueText.text = $"{PlayerPrefs.GetInt("NotePosition")}";

        slideSpeedValueText = transform.Find($"{contentPath}/GameSettings/Area/SlideSpeedSetting/Value").GetComponent<TextMeshProUGUI>();
        slideSpeedSlider = transform.Find($"{contentPath}/GameSettings/Area/SlideSpeedSetting/Slider").GetComponent<Slider>();
        slideSpeedSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("SlideSpeed"));
        slideSpeedValueText.text = $"{(int)slideSpeedSlider.value / 10}.{slideSpeedSlider.value % 10}";

        screenResolutionDropdown = transform.Find($"{contentPath}/ScreenSettings/Area/ScreenResolutionSetting/Dropdown").GetComponent<TMP_Dropdown>();
        fullScreenDropdown = transform.Find($"{contentPath}/ScreenSettings/Area/FullScreenSetting/Dropdown").GetComponent<TMP_Dropdown>();
        Init();
    }

    void Init()
    {
        switch (Screen.width)
        {
            case 1920:
                screenResolutionDropdown.value = (int)ScreenResolution._1920x1080;
                break;
            case 1600:
                screenResolutionDropdown.value = (int)ScreenResolution._1600x900;
                break;
            case 1440:
                screenResolutionDropdown.value = (int)ScreenResolution._1440x810;
                break;
            case 1366:
                screenResolutionDropdown.value = (int)ScreenResolution._1366x768;
                break;
            case 1280:
                screenResolutionDropdown.value = (int)ScreenResolution._1280x720;
                break;
            case 1024:
                screenResolutionDropdown.value = (int)ScreenResolution._1024x576;
                break;
        }

        switch (Screen.fullScreenMode)
        {
            case UnityEngine.FullScreenMode.FullScreenWindow:
                fullScreenDropdown.value = (int)Define.FullScreenMode.Fullscreen;
                break;
            case UnityEngine.FullScreenMode.ExclusiveFullScreen:
                fullScreenDropdown.value = (int)Define.FullScreenMode.Exclusive;
                break;
            case UnityEngine.FullScreenMode.Windowed:
                fullScreenDropdown.value = (int)Define.FullScreenMode.Windowed;
                break;
        }
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
