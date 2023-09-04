using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class EditorOptionUI : MonoBehaviour
{
    TextMeshProUGUI musicSoundValueText;
    Slider musicSoundSlider;
    TextMeshProUGUI effectSoundValueText;
    Slider effectSoundSlider;
    TMP_Dropdown screenResolutionDropdown;
    TMP_Dropdown fullScreenDropdown;
    string contentPath;

    private void Awake()
    {
        contentPath = "ScrollView/Viewport/Content/";

        musicSoundValueText = transform.Find($"{contentPath}MusicSoundSetting/Value").GetComponent<TextMeshProUGUI>();
        musicSoundSlider = transform.Find($"{contentPath}MusicSoundSetting/Slider").GetComponent<Slider>();
        effectSoundValueText = transform.Find($"{contentPath}EffectSoundSetting/Value").GetComponent<TextMeshProUGUI>();
        effectSoundSlider = transform.Find($"{contentPath}EffectSoundSetting/Slider").GetComponent<Slider>();
        screenResolutionDropdown = transform.Find($"{contentPath}ScreenSettings/ScreenResolutionSetting/Dropdown").GetComponent<TMP_Dropdown>();
        fullScreenDropdown = transform.Find($"{contentPath}ScreenSettings/FullScreenSetting/Dropdown").GetComponent<TMP_Dropdown>();

        Init();
    }

    void Init()
    {
        musicSoundSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("MusicSoundValue"));
        musicSoundValueText.text = $"{PlayerPrefs.GetInt("MusicSoundValue")}";
        effectSoundSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("EffectSoundValue"));
        effectSoundValueText.text = $"{PlayerPrefs.GetInt("EffectSoundValue")}";
        EditorController.effectSoundValue = PlayerPrefs.GetInt("EffectSoundValue") / 100.0f;

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
        EditorController.effectSoundValue = PlayerPrefs.GetInt("EffectSoundValue") / 100.0f;
    }

    public void OnClickApplyButton()
    {
        gameObject.SetActive(false);
    }

    public void OnChangeScreenResolution()
    {
        switch(screenResolutionDropdown.value)
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
