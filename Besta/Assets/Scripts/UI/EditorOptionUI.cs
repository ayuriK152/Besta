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
    Toggle fullScreenToggle;
    string contentPath;

    private void Awake()
    {
        contentPath = "ScrollView/Viewport/Content/";

        musicSoundValueText = transform.Find($"{contentPath}MusicSoundSetting/Value").GetComponent<TextMeshProUGUI>();
        musicSoundSlider = transform.Find($"{contentPath}MusicSoundSetting/Slider").GetComponent<Slider>();
        effectSoundValueText = transform.Find($"{contentPath}EffectSoundSetting/Value").GetComponent<TextMeshProUGUI>();
        effectSoundSlider = transform.Find($"{contentPath}EffectSoundSetting/Slider").GetComponent<Slider>();
        screenResolutionDropdown = transform.Find($"{contentPath}ScreenSettings/ScreenResolutionSetting/Dropdown").GetComponent<TMP_Dropdown>();
        fullScreenToggle = transform.Find($"{contentPath}ScreenSettings/FullScreenSetting/Toggle").GetComponent<Toggle>();

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

        fullScreenToggle.isOn = Screen.fullScreen;
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
                Screen.SetResolution(1920, 1080, fullScreenToggle.isOn);
                break;
            case (int)ScreenResolution._1600x900:
                Screen.SetResolution(1600, 900, fullScreenToggle.isOn);
                break;
            case (int)ScreenResolution._1440x810:
                Screen.SetResolution(1440, 810, fullScreenToggle.isOn);
                break;
            case (int)ScreenResolution._1366x768:
                Screen.SetResolution(1366, 768, fullScreenToggle.isOn);
                break;
            case (int)ScreenResolution._1280x720:
                Screen.SetResolution(1280, 720, fullScreenToggle.isOn);
                break;
            case (int)ScreenResolution._1024x576:
                Screen.SetResolution(1024, 576, fullScreenToggle.isOn);
                break;
        }
    }

    public void OnToggleFullScreen()
    {
        Screen.fullScreen = fullScreenToggle.isOn;
    }
}
