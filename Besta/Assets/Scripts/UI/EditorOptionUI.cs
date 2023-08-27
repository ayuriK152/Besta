using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorOptionUI : MonoBehaviour
{
    TextMeshProUGUI musicSoundValueText;
    Slider musicSoundSlider;
    Button applyButton;

    private void Awake()
    {
        musicSoundValueText = transform.Find("ScrollView/Viewport/Content/MusicSoundSetting/Value").GetComponent<TextMeshProUGUI>();
        musicSoundSlider = transform.Find("ScrollView/Viewport/Content/MusicSoundSetting/Slider").GetComponent<Slider>();
        musicSoundSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("MusicSoundValue"));
        musicSoundValueText.text = $"{PlayerPrefs.GetInt("MusicSoundValue")}";

        applyButton = transform.Find("ApplyButton").GetComponent<Button>();
    }

    public void OnMusicSoundValueChange()
    {
        PlayerPrefs.SetInt("MusicSoundValue", (int)musicSoundSlider.value);
        musicSoundValueText.text = PlayerPrefs.GetInt("MusicSoundValue").ToString();
        Managers.Sound.managerAudioSource.volume = PlayerPrefs.GetInt("MusicSoundValue") / 100.0f;
    }

    public void OnClickApplyButton()
    {
        gameObject.SetActive(false);
    }
}
