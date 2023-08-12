using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectUI : MonoBehaviour
{
    public Image currentImage;
    public TextMeshProUGUI currentName;
    public TextMeshProUGUI currentArtist;
    public TextMeshProUGUI currentBPM;

    private void Awake()
    {
        currentImage = transform.Find("MusicDetailPreview").Find("CurrentImage").GetComponent<Image>();
        currentName = transform.Find("MusicDetailPreview").Find("CurrentName").GetComponent<TextMeshProUGUI>();
        currentArtist = transform.Find("MusicDetailPreview").Find("CurrentArtist").GetComponent<TextMeshProUGUI>();
        currentBPM = transform.Find("MusicDetailPreview").Find("CurrentBPM").GetComponent<TextMeshProUGUI>();
    }
}
