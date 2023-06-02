using TMPro;
using UnityEngine;
using static Define;

public class EditorUI : MonoBehaviour
{
    TMP_Dropdown beatChangeDropdown;

    private void Start()
    {
        beatChangeDropdown = GameObject.Find("BeatSelector").transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        beatChangeDropdown.value = 3;
        EditorController.BeatChangeAction.Invoke(EditorController.editorBeat);
    }

    public void OnBeatChange()
    {
        EditorController.editorBeat = (Beat)beatChangeDropdown.value;
        EditorController.BeatChangeAction.Invoke(EditorController.editorBeat);
    }
}
