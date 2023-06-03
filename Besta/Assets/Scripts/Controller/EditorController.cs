using System;
using UnityEngine;
using static Datas;
using static Define;

public class EditorController : MonoBehaviour
{
    static EditorController _editorInstance;
    static EditorController EditorInstance { get { return _editorInstance; } }

    public Action<int, int, int, bool> NoteCreateAction;
    public static Action<Beat> BeatChangeAction;

    public static EditorNoteMode editorNoteMode;
    public static Beat editorBeat;

    MusicPattern _musicPattern;
    double _noteTimingValue;
    int barAmount;

    GameObject _noteInstantiatePoint;
    GameObject _barInstatiatePoint;
    GameObject _editorNote;
    GameObject _editorBar;

    void Start()
    {
        editorNoteMode = EditorNoteMode.NormalNote;
        editorBeat = Beat.Eight;
        _musicPattern = new MusicPattern();
        _noteTimingValue = (_musicPattern._musicSource.frequency / 4) / (_musicPattern._bpm / (double)60);
        barAmount = (int)(_musicPattern._songLength / (_noteTimingValue * 16)) + 1;
        NoteCreateAction = null;
        _noteInstantiatePoint = GameObject.Find("Notes");
        _barInstatiatePoint = GameObject.Find("Grid");
        _editorNote = Resources.Load<GameObject>("Prefabs/EditorNote");
        _editorBar = Resources.Load<GameObject>("Prefabs/EditorBar");
        Init();

        Managers.Input.MouseAction -= EditorMouseEvent;
        Managers.Input.MouseAction += EditorMouseEvent;
    }

    void Init()
    {
        for (int i = 0; i < barAmount; i++)
        {
            Instantiate(_editorBar, _barInstatiatePoint.transform);
            _editorBar.transform.localPosition = new Vector3(0, i * 4.8f, 0);
        }
    }

    void Update()
    {

    }

    void EditorMouseEvent(MouseEvent mouseEvent, MousePointer mousePointer)
    {
        if (mouseEvent == MouseEvent.PointerDown && mousePointer == MousePointer.Left)
        {
            Ray2D ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction);
            if (hit.collider == null)
                return;

            if (hit.collider.tag == "EditorCollider")
            {
                Instantiate(_editorNote, hit.collider.transform.position, hit.collider.transform.rotation, _noteInstantiatePoint.transform).name = _editorNote.name;
                Debug.Log("Note Instantiated");
            }
        }

        if (mouseEvent == MouseEvent.PointerDown && mousePointer == MousePointer.Right)
        {
            Ray2D ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider == null)
                return;

            if (hit.collider.tag == "EditorNote")
            {
                Destroy(hit.collider.gameObject);
                Debug.Log("Note Deleted");
            }
        }
    }
}
