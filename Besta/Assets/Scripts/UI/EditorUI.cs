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

    void Start()
    {
        beatChangeDropdown = GameObject.Find("BeatSelector").transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        beatChangeDropdown.value = 3;
        noteModeChangeDropdown = GameObject.Find("NoteModeSelector").transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        noteModeChangeDropdown.value = 0;
        editorPlayValueSlider = transform.Find("PlayValueSlider").transform.GetComponent<Slider>();
        editorPlayValueSlider.value = 0;
        baseBPMInputField = GameObject.Find("BPMSetting").transform.Find("InputField").GetComponent<TMP_InputField>();
        baseBPMInputField.text = EditorController.baseBPM.ToString();
        offsetInputField = GameObject.Find("OffsetSetting").transform.Find("InputField").GetComponent<TMP_InputField>();
        offsetInputField.text = EditorController.patternOffset.ToString();
    }

    void Update()
    {
        if (EditorController.isGridScrolling)
            editorPlayValueSlider.value = EditorController.currentPlayValue;
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
}
