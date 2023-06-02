using System;
using UnityEditor;
using UnityEngine;
using static Datas;
using static Define;

public class EditorController : MonoBehaviour
{
    static EditorController _editorInstance;
    static EditorController EditorInstance { get { return _editorInstance; } }

    public Action<int, int, int, bool> NoteCreateAction;
    public Action<Beat> BeatChangeAction;
    public EditorNoteMode editorNoteMode;
    MusicPattern _musicPattern;
    double _noteTimingValue;

    GameObject _noteInstantiatePoint;
    GameObject _editorNote;

    void Start()
    {
        editorNoteMode = EditorNoteMode.NormalNote;
        _musicPattern = new MusicPattern();
        _noteTimingValue = 48000 / (_musicPattern._bpm / (double)60);
        NoteCreateAction = null;
        _noteInstantiatePoint = GameObject.Find("Notes");
        _editorNote = Resources.Load<GameObject>("Prefabs/EditorNote");

        Managers.Input.MouseAction -= EditorMouseEvent;
        Managers.Input.MouseAction += EditorMouseEvent;
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

            if (hit.collider.tag == "EditorNote")
            {
                Destroy(hit.collider.gameObject);
                Debug.Log("Note Deleted");
            }
        }
    }
}
