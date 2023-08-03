using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class EditorUI : MonoBehaviour
{
    TMP_Dropdown beatChangeDropdown;
    TMP_Dropdown noteModeChangeDropdown;
    public Slider editorPlayValueSlider;
    TMP_InputField baseBPMInputField;
    TMP_InputField offsetInputField;
    TMP_InputField patternTitleInputField;
    TextMeshProUGUI originTimeText;
    TextMeshProUGUI progressTimeText;
    TextMeshProUGUI progressPercetageText;
    TextMeshProUGUI maxComboText;

    void Start()
    {
        #region UI바인딩 및 초기화
        beatChangeDropdown = GameObject.Find("BeatSelector").transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        beatChangeDropdown.value = 5;
        noteModeChangeDropdown = GameObject.Find("NoteModeSelector").transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        noteModeChangeDropdown.value = 0;
        editorPlayValueSlider = GameObject.Find("PlayValueSlider").transform.GetComponent<Slider>();
        editorPlayValueSlider.value = 0;
        baseBPMInputField = GameObject.Find("BPMSetting").transform.Find("InputField").GetComponent<TMP_InputField>();
        baseBPMInputField.text = EditorController.baseBPM.ToString();
        offsetInputField = GameObject.Find("OffsetSetting").transform.Find("InputField").GetComponent<TMP_InputField>();
        offsetInputField.text = EditorController.patternOffset.ToString();
        originTimeText = GameObject.Find("TimeProgress").transform.Find("Origin").GetComponent<TextMeshProUGUI>();
        progressTimeText = GameObject.Find("TimeProgress").transform.Find("Current").GetComponent<TextMeshProUGUI>();
        progressPercetageText = GameObject.Find("Progress").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        progressPercetageText.text = "0.0%";
        maxComboText = GameObject.Find("MaxCombo").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        maxComboText.text = "0";
        patternTitleInputField = GameObject.Find("PatternNameSetting").transform.Find("InputField").GetComponent<TMP_InputField>();
        #endregion

        EditorController.PatternMaxComboUpdateAction -= OnMaxComboChange;
        EditorController.PatternMaxComboUpdateAction += OnMaxComboChange;
    }

    void Update()
    {
        if (EditorController.isGridScrolling)
        {
            editorPlayValueSlider.value = EditorController.currentPlayValue;
            progressPercetageText.text = $"{(Math.Truncate(editorPlayValueSlider.value * 10000) / 100).ToString("F2")}%";
        }
        if (EditorController.isPlayValueChanged)
        {
            editorPlayValueSlider.value = EditorController.currentPlayValue;
            EditorController.isPlayValueChanged = false;
        }

    }

    public void OnBeatChange()
    {
        EditorController.editorBeat = (Beat)beatChangeDropdown.value;
        if (EditorController.BeatChangeAction != null)
            EditorController.BeatChangeAction.Invoke(EditorController.editorBeat);
    }

    public void OnNoteModeChange()
    {
        EditorController.editorNoteMode = (EditorNoteMode)noteModeChangeDropdown.value;
    }

    public void OnPlayValueChange()
    {
        if (!EditorController.isGridScrolling)
        {
            progressPercetageText.text = $"{(Math.Truncate(editorPlayValueSlider.value * 10000) / 100).ToString("F2")}%";
            EditorController.currentPlayValue = editorPlayValueSlider.value;
            EditorController.PlayValueChangeAction.Invoke(true);
        }
    }

    public void OnBaseBPMChange()
    {
        EditorController.baseBPM = int.Parse(baseBPMInputField.text);
        EditorController.PatternSettingChangeAction.Invoke();
    }

    public void OnBaseBPMChangeByController()
    {
        baseBPMInputField.text = EditorController.baseBPM.ToString();
    }

    public void OnOffsetChange()
    {
        EditorController.patternOffset = int.Parse(offsetInputField.text);
        EditorController.PatternSettingChangeAction.Invoke();
    }

    public void OnOffsetChangeByController()
    {
        offsetInputField.text = EditorController.patternOffset.ToString();
    }

    public void OnSaveButtonClick()
    {
        EditorController.PatternSaveAction.Invoke();
    }

    public void OnLoadButtonClick()
    {
        EditorController.PatternLoadAction.Invoke(EditorUtility.OpenFilePanel("Select pattern data file", $"{Application.dataPath}/Patterns", "json"));
    }

    public void OnCreateButtonClick()
    {
        EditorController.PatternCreateAction.Invoke();
    }

    public void OnMaxComboChange()
    {
        maxComboText.text = EditorController.totalCombo.ToString();
    }

    public void OnTitleUpdate()
    {
        EditorController.patternTitle = patternTitleInputField.text;
        EditorController.PatternSettingChangeAction.Invoke();
    }

    public void OnTitleUpdateByController()
    {
        patternTitleInputField.text = EditorController.patternTitle.ToString();
    }

    public void OriginTimeUIUpdate()
    {
        int sec = Managers.Sound.managerAudioSource.clip.samples / Managers.Sound.managerAudioSource.clip.frequency;
        int ms = (int)((Managers.Sound.managerAudioSource.clip.samples - Managers.Sound.managerAudioSource.clip.frequency * sec) / (float)Managers.Sound.managerAudioSource.clip.frequency * 1000);
        int min = sec / 60;
        sec -= min * 60;
        if (min / 10 > 0)
            originTimeText.text = $" / {min}.";
        else
            originTimeText.text = $" / 0{min}.";
        if (sec / 10 > 0)
            originTimeText.text += $"{sec}:";
        else
            originTimeText.text += $"0{sec}:";
        if (ms / 100 > 0)
            originTimeText.text += $"{ms}";
        else if (ms / 10 > 0)
            originTimeText.text += $"0{ms}";
        else
            originTimeText.text += $"00{ms}";
    }

    public void TimeProgressUIUpdate()
    {
        int sec = Managers.Sound.managerAudioSource.timeSamples / Managers.Sound.managerAudioSource.clip.frequency;
        int ms = (int)((Managers.Sound.managerAudioSource.timeSamples - Managers.Sound.managerAudioSource.clip.frequency * sec) / (float)Managers.Sound.managerAudioSource.clip.frequency * 1000);
        int min = sec / 60;
        sec -= min * 60;
        if (min / 10 > 0)
            progressTimeText.text = $"{min}.";
        else
            progressTimeText.text = $"0{min}.";
        if (sec / 10 > 0)
            progressTimeText.text += $"{sec}:";
        else
            progressTimeText.text += $"0{sec}:";
        if (ms / 100 > 0)
            progressTimeText.text += $"{ms}";
        else if (ms / 10 > 0)
            progressTimeText.text += $"0{ms}";
        else
            progressTimeText.text += $"00{ms}";
    }
}
