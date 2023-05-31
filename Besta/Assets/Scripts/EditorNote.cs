using System;
using UnityEngine;
using static Datas;

public class EditorNote : MonoBehaviour
{
    public Action noteDeleteAction;
    public Note noteData;
    GameObject _startPoint;
    GameObject _longNotePole;
    GameObject _endPoint;

    void Start()
    {
        noteDeleteAction = null;
        _startPoint = transform.Find("Start").gameObject;
        _longNotePole = transform.Find("LongNotePole").gameObject;
        _endPoint = transform.Find("End").gameObject;
    }

    void ResizePole()
    {
        float positionDifference = _endPoint.transform.localPosition.y - _startPoint.transform.localPosition.y;
        _longNotePole.transform.localPosition = new Vector3(0, positionDifference / 2 + _startPoint.transform.position.y, 1);
        _longNotePole.transform.localScale = new Vector3(1, positionDifference / 0.25f, 1);
    }
}
