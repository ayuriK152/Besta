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
        _linesByBeat = new GameObject[7];
        _linesByBeat[0] = transform.Find("1over2").gameObject;
        _linesByBeat[1] = transform.Find("1over4").gameObject;
        _linesByBeat[2] = transform.Find("1over8").gameObject;
        _linesByBeat[3] = transform.Find("1over16").gameObject;
        _linesByBeat[4] = transform.Find("1over3").gameObject;
        _linesByBeat[5] = transform.Find("1over6").gameObject;
        _linesByBeat[6] = transform.Find("1over9").gameObject;
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
                _linesByBeat[4].SetActive(false);
                _linesByBeat[5].SetActive(false);
                _linesByBeat[6].SetActive(false);
                break;
            case Beat.One:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(false);
                _linesByBeat[1].SetActive(false);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                _linesByBeat[4].SetActive(false);
                _linesByBeat[5].SetActive(false);
                _linesByBeat[6].SetActive(false);
                break;
            case Beat.Two:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(false);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                _linesByBeat[4].SetActive(false);
                _linesByBeat[5].SetActive(false);
                _linesByBeat[6].SetActive(false);
                break;
            case Beat.Three:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(false);
                _linesByBeat[1].SetActive(false);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                _linesByBeat[4].SetActive(true);
                _linesByBeat[5].SetActive(false);
                _linesByBeat[6].SetActive(false);
                break;
            case Beat.Four:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(true);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                _linesByBeat[4].SetActive(false);
                _linesByBeat[5].SetActive(false);
                _linesByBeat[6].SetActive(false);
                break;
            case Beat.Six:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(false);
                _linesByBeat[1].SetActive(false);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                _linesByBeat[4].SetActive(true);
                _linesByBeat[5].SetActive(true);
                _linesByBeat[6].SetActive(false);
                break;
            case Beat.Eight:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(true);
                _linesByBeat[2].SetActive(true);
                _linesByBeat[3].SetActive(false);
                _linesByBeat[4].SetActive(false);
                _linesByBeat[5].SetActive(false);
                _linesByBeat[6].SetActive(false);
                break;
            case Beat.Nine:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(false);
                _linesByBeat[1].SetActive(false);
                _linesByBeat[2].SetActive(false);
                _linesByBeat[3].SetActive(false);
                _linesByBeat[4].SetActive(true);
                _linesByBeat[5].SetActive(true);
                _linesByBeat[6].SetActive(true);
                break;
            case Beat.Sixteen:
                _baseLine.SetActive(true);
                _linesByBeat[0].SetActive(true);
                _linesByBeat[1].SetActive(true);
                _linesByBeat[2].SetActive(true);
                _linesByBeat[3].SetActive(true);
                _linesByBeat[4].SetActive(false);
                _linesByBeat[5].SetActive(false);
                _linesByBeat[6].SetActive(false);
                break;
        }
    }
}
