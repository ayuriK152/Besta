using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class EditorUI : MonoBehaviour
{
    static EditorUI _uiInstance;
    static EditorUI UIInstance { get { return _uiInstance; } }
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
        if (EditorController._isGridScrolling)
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
        if (!EditorController._isGridScrolling)
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

    public void OnOffsetChange()
    {
        EditorController.patternOffset = int.Parse(offsetInputField.text);
        EditorController.PatternSettingChangeAction.Invoke();
    }

    public void OnSaveButtonClick()
    {
        EditorController.PatternSaveAction.Invoke();
    }
}
