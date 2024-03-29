using UnityEngine;
using static Datas;

public class EditorNote : MonoBehaviour
{
    public Note noteData;
    GameObject _startPoint;
    GameObject _longNotePole;
    public GameObject longNotePole { get { return _longNotePole; } }
    GameObject _endPoint;
    public GameObject endPoint { get { return _endPoint; } }

    AudioSource hitSound;
    bool isTriggered;

    void Awake()
    {
        _startPoint = transform.Find("Start").gameObject;
        _longNotePole = transform.Find("LongNotePole").gameObject;
        _endPoint = transform.Find("End").gameObject;

        hitSound = GetComponent<AudioSource>();
        isTriggered = false;
    }

    public void ResizePole()
    {
        float positionDifference = _endPoint.transform.localPosition.y - _startPoint.transform.localPosition.y;
        _longNotePole.transform.localPosition = new Vector3(_startPoint.transform.localPosition.x, positionDifference / 2 + _startPoint.transform.localPosition.y, 1);
        _longNotePole.transform.localScale = new Vector3(1, positionDifference / 0.25f, 1);
    }

    void Update()
    {
        if (Managers.Sound.managerAudioSource.timeSamples + EditorController.patternOffset < noteData.startTiming)
            isTriggered = false;
        if (!isTriggered && Managers.Sound.managerAudioSource.timeSamples + EditorController.patternOffset >= noteData.startTiming)
        {
            hitSound.volume = EditorController.effectSoundValue;
            hitSound.Play();
            isTriggered = true;
        }
    }
}
