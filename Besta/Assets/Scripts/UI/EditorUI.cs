using TMPro;
using UnityEngine;
using static Define;

public class EditorUI : MonoBehaviour
{
    TMP_Dropdown beatChangeDropdown;
    TMP_Dropdown noteModeChangeDropdown;

    private void Start()
    {
        beatChangeDropdown = GameObject.Find("BeatSelector").transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        beatChangeDropdown.value = 3;
        noteModeChangeDropdown = GameObject.Find("NoteModeSelector").transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        noteModeChangeDropdown.value = 0;
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
}
