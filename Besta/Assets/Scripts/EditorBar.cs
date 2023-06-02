using UnityEngine;
using static Define;

public class EditorBar : MonoBehaviour
{
    public static int barCount = 0;
    public int barIndex;
    GameObject[] _linesByBeat;

    void Start()
    {
        EditorController.BeatChangeAction -= OnBeatChanged;
        EditorController.BeatChangeAction += OnBeatChanged;
        _linesByBeat = new GameObject[4];
        _linesByBeat[0] = transform.Find("1over2").gameObject;
        _linesByBeat[1] = transform.Find("1over4").gameObject;
        _linesByBeat[2] = transform.Find("1over8").gameObject;
        _linesByBeat[3] = transform.Find("1over16").gameObject;
    }

    void Update()
    {
        
    }

    void OnBeatChanged(Beat beat)
    {
        switch (beat)
        {
            case Beat.One:
                _linesByBeat[0].SetActive(false);
                _linesByBeat[1].SetActive(false);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                break;
            case Beat.Two:
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(false);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                break;
            case Beat.Four:
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(true);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                break;
            case Beat.Eight:
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(true);
                _linesByBeat[2].SetActive(true);
                _linesByBeat[3].SetActive(false);
                break;
            case Beat.Sixteen:
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(true);
                _linesByBeat[2].SetActive(true);
                _linesByBeat[3].SetActive(true);
                break;
        }
    }
}
