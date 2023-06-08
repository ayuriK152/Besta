using UnityEngine;
using static Define;

public class EditorBar : MonoBehaviour
{
    public static int barCount = 0;
    public int barIndex;
    GameObject _baseLine;
    GameObject[] _linesByBeat;
    bool _isShowing;

    void Start()
    {
        EditorController.BeatChangeAction -= OnBeatChanged;
        EditorController.BeatChangeAction += OnBeatChanged;
        _baseLine = transform.Find("Base").gameObject;
        _linesByBeat = new GameObject[4];
        _linesByBeat[0] = transform.Find("1over2").gameObject;
        _linesByBeat[1] = transform.Find("1over4").gameObject;
        _linesByBeat[2] = transform.Find("1over8").gameObject;
        _linesByBeat[3] = transform.Find("1over16").gameObject;
        _isShowing = true;
    }

    void Update()
    {
        if (transform.position.y >= EditorController.barLowerLimitPos.y && transform.position.y <= EditorController.barUpperLimitPos.y && !_isShowing)
        {
            OnBeatChanged(EditorController.editorBeat);
            _isShowing = true;
        }
        else if ((transform.position.y < EditorController.barLowerLimitPos.y || transform.position.y > EditorController.barUpperLimitPos.y) && _isShowing)
        {
            OnBeatChanged(Beat.None);
            _isShowing = false;
        }
    }

    void OnBeatChanged(Beat beat)
    {
        if ((transform.position.y < EditorController.barLowerLimitPos.y || transform.position.y > EditorController.barUpperLimitPos.y) && beat != Beat.None)
            return;
        switch (beat)
        {
            case Beat.None:
                _baseLine.SetActive(false);
                _linesByBeat[0].SetActive(false);
                _linesByBeat[1].SetActive(false);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                break;
            case Beat.One:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(false);
                _linesByBeat[1].SetActive(false);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                break;
            case Beat.Two:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(false);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                break;
            case Beat.Four:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(true);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                break;
            case Beat.Eight:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(true);
                _linesByBeat[2].SetActive(true);
                _linesByBeat[3].SetActive(false);
                break;
            case Beat.Sixteen:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(true);
                _linesByBeat[2].SetActive(true);
                _linesByBeat[3].SetActive(true);
                break;
        }
    }
}
