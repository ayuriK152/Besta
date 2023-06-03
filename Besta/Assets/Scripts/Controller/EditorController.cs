using System;
using System.Collections.Generic;
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
    List<GameObject> _instantiatedEditorBars = new List<GameObject>();

    void Start()
    {
        editorNoteMode = EditorNoteMode.LongNote;
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
            _instantiatedEditorBars.Add(Instantiate(_editorBar, _barInstatiatePoint.transform));
            _instantiatedEditorBars[i].transform.localPosition = new Vector3(0, i * 4.8f, 0);
            _instantiatedEditorBars[i].name = _editorBar.name + " 1";
        }
    }

    GameObject initialNote;
    void EditorMouseEvent(MouseEvent mouseEvent, MousePointer mousePointer)
    {
        Ray2D ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (mouseEvent == MouseEvent.PointerDown && mousePointer == MousePointer.Left)      // 왼쪽 마우스 클릭
        {
            if (hit.collider == null)
                return;
            if (hit.collider.tag == "EditorNote")
            {
                Destroy(hit.collider.transform.parent.gameObject);
                Debug.Log("Note Deleted");
                hit = Physics2D.Raycast(ray.origin, ray.direction);
            }
            if (hit.collider.tag == "EditorCollider")
            {
                initialNote = Instantiate(_editorNote, new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, -2), hit.collider.transform.rotation, _noteInstantiatePoint.transform);
                initialNote.name =  _editorNote.name;
                if (editorNoteMode == EditorNoteMode.LongNote)
                {
                    initialNote.GetComponent<EditorNote>().longNotePole.SetActive(true);
                    initialNote.GetComponent<EditorNote>().endPoint.SetActive(true);
                }
                Debug.Log("Note Instantiated");
            }
        }

        if (mouseEvent == MouseEvent.Press && mousePointer == MousePointer.Left)        // 왼쪽 마우스 드래그
        {
            if (hit.collider == null)
                return;
            if (hit.collider.tag == "EditorCollider" && editorNoteMode == EditorNoteMode.LongNote && initialNote != null)
            {
                initialNote.GetComponent<EditorNote>().endPoint.transform.localPosition = new Vector2(0, hit.collider.transform.position.y - initialNote.transform.position.y);
                initialNote.GetComponent<EditorNote>().ResizePole();
                Debug.Log("amogus");
            }
        }

        if (mouseEvent == MouseEvent.PointerUp && mousePointer == MousePointer.Left)
        {
            if (initialNote != null)
            {
                initialNote.GetComponent<EditorNote>().longNotePole.GetComponent<BoxCollider2D>().enabled = true;
                initialNote = null;
            }
        }

        if (mouseEvent == MouseEvent.PointerDown && mousePointer == MousePointer.Right)
        {
            if (hit.collider.tag == "EditorNote")
            {
                Destroy(hit.collider.transform.parent.gameObject);
                Debug.Log("Note Deleted");
            }
        }
    }
}
