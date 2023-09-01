using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectUI : MonoBehaviour
{
    public Image currentImage;
    public TextMeshProUGUI currentName;
    public TextMeshProUGUI currentArtist;
    public TextMeshProUGUI currentBPM;
    public GameObject optionUIObj;
    public GameObject exitPanelObj;

    void Awake()
    {
        currentImage = transform.Find("MusicDetailPreview/CurrentImage").GetComponent<Image>();
        currentName = transform.Find("MusicDetailPreview/CurrentName").GetComponent<TextMeshProUGUI>();
        currentArtist = transform.Find("MusicDetailPreview/CurrentArtist").GetComponent<TextMeshProUGUI>();
        currentBPM = transform.Find("MusicDetailPreview/CurrentBPM").GetComponent<TextMeshProUGUI>();

        optionUIObj = transform.Find("OptionPanel").gameObject;
        exitPanelObj = transform.Find("ExitPanel").gameObject;
    }

    void Start()
    {
        optionUIObj.SetActive(false);
        exitPanelObj.SetActive(false);
    }
}
