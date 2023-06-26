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
    public static Action<bool> PlayValueChangeAction;
    public static Action PatternSettingChangeAction;

    public static EditorNoteMode editorNoteMode;
    public static Beat editorBeat;
    public static float currentPlayValue;       // 노래 파일의 샘플 레이트 기준 재생 시간 측정용
    public static int baseBPM;
    public static int patternOffset;

    [SerializeField]
    public MusicPattern _musicPattern;
    double _noteTimingValue;
    int barAmount;
    float _editorBarMaxPosition;

    GameObject _noteInstantiatePoint;
    GameObject _barInstatiatePoint;
    GameObject _editorNote;
    GameObject _editorBar;
    List<GameObject> _instantiatedEditorBars = new List<GameObject>();

    public static Vector3 barUpperLimitPos;
    public static Vector3 barLowerLimitPos;

    void Start()
    {
        editorNoteMode = EditorNoteMode.NormalNote;
        editorBeat = Beat.Eight;
        _musicPattern = new MusicPattern();
        _noteTimingValue = (_musicPattern._musicSource.frequency / (_musicPattern._bpm / (double)60)) / 4;
        barAmount = (int)(_musicPattern._songLength / (_noteTimingValue * 16)) + 1;
        _editorBarMaxPosition = Managers.Sound.managerAudioSource.clip.samples * 4.8f / ((float)_noteTimingValue * 16);
        currentPlayValue = 0;
        baseBPM = _musicPattern._bpm;
        patternOffset = _musicPattern._songOffset;

        NoteCreateAction = null;
        BeatChangeAction = null;
        barUpperLimitPos = GameObject.Find("UpperLimit").transform.position;
        barLowerLimitPos = GameObject.Find("LowerLimit").transform.position;
        _noteInstantiatePoint = GameObject.Find("Notes");
        _barInstatiatePoint = GameObject.Find("Grid");
        _editorNote = Resources.Load<GameObject>("Prefabs/EditorNote");
        _editorBar = Resources.Load<GameObject>("Prefabs/EditorBar");
        Init();

        Managers.Sound.managerAudioSource.clip = _musicPattern._musicSource;
        Managers.Input.MouseAction -= EditorMouseEvent;
        Managers.Input.MouseAction += EditorMouseEvent;
        Managers.Input.ScrollAction -= EditorMouseScrollEvent;
        Managers.Input.ScrollAction += EditorMouseScrollEvent;
        PlayValueChangeAction -= OnPlayValueChanged;
        PlayValueChangeAction += OnPlayValueChanged;
        PatternSettingChangeAction -= OnSettingValueChanged;
        PatternSettingChangeAction += OnSettingValueChanged;
    }

    public static bool _isGridScrolling = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isGridScrolling = !_isGridScrolling;
            if (!_isGridScrolling)
                Managers.Sound.managerAudioSource.Pause();
        }
        if (_isGridScrolling)
            EditorGridScroll();
    }

    void Init()
    {
        for (int i = 0; i < barAmount; i++)
        {
            _instantiatedEditorBars.Add(Instantiate(_editorBar, _barInstatiatePoint.transform));
            _instantiatedEditorBars[i].transform.localPosition = new Vector3(0, i * 4.8f, 0);
            _instantiatedEditorBars[i].name = _editorBar.name + " " + (i + 1);
            _instantiatedEditorBars[i].GetComponent<EditorBar>().barIndex = i;
        }
    }

    void NoteInstantiateOnMouseButtonDown(Ray2D ray, RaycastHit2D hit)      // 왼쪽 마우스 버튼 down시 노트 입력 시작
    {
        if (hit.collider == null)
            return;

        if (hit.collider.tag == "EditorNote")       // 이미 노트가 있는 부분에 중복 입력을 시도한 경우, 해당 노트 삭제후 입력 시작
        {
            if (!_musicPattern._noteDatas.Remove(hit.collider.transform.parent.GetComponent<EditorNote>().noteData))
            {
                Debug.LogError("Note delete while initiating new note does not functioning!");
                return;
            }
            
            Destroy(hit.collider.transform.parent.gameObject);
            Debug.Log("Note Deleted");
            hit = Physics2D.Raycast(ray.origin, ray.direction);
        }

        if (hit.collider == null)
            return;

        if (hit.collider.tag == "EditorCollider")   // 생성부 클릭시 노트 생성 및 데이터 입력
        {
            Transform editorCollider = hit.collider.transform;
            Transform editorColliderParent = editorCollider.parent;
            currentNote = Instantiate(_editorNote, new Vector3(editorCollider.position.x, editorCollider.position.y, -2), editorCollider.rotation, _noteInstantiatePoint.transform);
            LaneNumber tempNoteLaneNum = LaneNumber.None;
            switch (hit.collider.name)
            {
                case "First":
                    tempNoteLaneNum = LaneNumber.First;
                    break;
                case "Second":
                    tempNoteLaneNum = LaneNumber.Second;
                    break;
                case "Third":
                    tempNoteLaneNum = LaneNumber.Third;
                    break;
                case "Fourth":
                    tempNoteLaneNum = LaneNumber.Fourth;
                    break;
            }

            int currentBarIndex;
            if (editorColliderParent.name != "Base")
                currentBarIndex = editorColliderParent.parent.parent.GetComponent<EditorBar>().barIndex;
            else
                currentBarIndex = editorColliderParent.parent.GetComponent<EditorBar>().barIndex;

            Note tempNoteData = new Note(tempNoteLaneNum, (int)(((currentBarIndex * 16) + (editorColliderParent.transform.localPosition.y / 0.3f)) * _noteTimingValue), 0, false);
            if (editorNoteMode == EditorNoteMode.LongNote)
            {
                tempNoteData._isLongNote = true;
                currentNote.GetComponent<EditorNote>().longNotePole.SetActive(true);
                currentNote.GetComponent<EditorNote>().endPoint.SetActive(true);
            }
            currentNote.GetComponent<EditorNote>().noteData = tempNoteData;
            Debug.Log("Note Instantiated");
        }
    }
    void NoteInstantiateOnMouseButtonPress(RaycastHit2D hit)                // 왼쪽 마우스 번튼 press, 롱노트 모드인 경우
    {
        if (hit.collider == null)
            return;
        if (hit.collider.tag == "EditorCollider" && editorNoteMode == EditorNoteMode.LongNote && currentNote != null)
        {
            Transform editorCollider = hit.collider.transform;
            Transform editroColliderParent = editorCollider.parent;

            int currentBarIndex;
            if (editroColliderParent.name != "Base")
                currentBarIndex = editroColliderParent.parent.parent.GetComponent<EditorBar>().barIndex;
            else
                currentBarIndex = editroColliderParent.parent.GetComponent<EditorBar>().barIndex;

            currentNote.GetComponent<EditorNote>().noteData._endTiming = (int)(((currentBarIndex * 16) + (editroColliderParent.transform.localPosition.y / 0.3f)) * _noteTimingValue);
            currentNote.GetComponent<EditorNote>().endPoint.transform.localPosition = new Vector2(0, editorCollider.position.y - currentNote.transform.position.y);
            currentNote.GetComponent<EditorNote>().ResizePole();
        }
    }
    void NoteInstantiateOnMouseButtonUp()
    {
        if (currentNote != null)
        {
            if (editorNoteMode == EditorNoteMode.LongNote)
            {
                if (currentNote.GetComponent<EditorNote>().noteData._startTiming == currentNote.GetComponent<EditorNote>().noteData._endTiming)
                {
                    currentNote.GetComponent<EditorNote>().noteData._endTiming = 0;
                    currentNote.GetComponent<EditorNote>().noteData._isLongNote = false;
                }
                else
                    currentNote.GetComponent<EditorNote>().longNotePole.GetComponent<BoxCollider2D>().enabled = true;
            }
            _musicPattern._noteDatas.Add(currentNote.GetComponent<EditorNote>().noteData);
            Debug.Log(currentNote.GetComponent<EditorNote>().noteData._laneNumber.ToString() + " " + currentNote.GetComponent<EditorNote>().noteData._startTiming + " " + currentNote.GetComponent<EditorNote>().noteData._endTiming);
            currentNote = null;
        }
    }

    void NoteDeleteOnMouseButtonDown(RaycastHit2D hit)
    {
        if (hit.collider == null)
            return;
        if (hit.collider.tag == "EditorNote")
        {
            if (!_musicPattern._noteDatas.Remove(hit.collider.transform.parent.GetComponent<EditorNote>().noteData))
            {
                Debug.LogError("Note delete does not functioning!");
                return;
            }
            Destroy(hit.collider.transform.parent.gameObject);
            Debug.Log("Note Deleted");
        }
    }

    GameObject currentNote;
    void EditorMouseEvent(MouseEvent mouseEvent, MousePointer mousePointer)         // 에디터상에서의 마우스 입력 처리 메소드
    {
        Ray2D ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (mouseEvent == MouseEvent.PointerDown && mousePointer == MousePointer.Left)      // 왼쪽 마우스 클릭
        {
            NoteInstantiateOnMouseButtonDown(ray, hit);
        }

        if (mouseEvent == MouseEvent.Press && mousePointer == MousePointer.Left)        // 왼쪽 마우스 드래그
        {
            NoteInstantiateOnMouseButtonPress(hit);
        }

        if (mouseEvent == MouseEvent.PointerUp && mousePointer == MousePointer.Left)    // 왼쪽 마우스 때는 순간
        {
            NoteInstantiateOnMouseButtonUp();
        }

        if (mouseEvent == MouseEvent.PointerDown && mousePointer == MousePointer.Right)
        {
            NoteDeleteOnMouseButtonDown(hit);
        }
    }

    void EditorMouseScrollEvent(MouseScroll scrollDir)
    {
        if (scrollDir == MouseScroll.Down && currentPlayValue >= 0)
        {
            if (_isGridScrolling)
            {
                _isGridScrolling = false;
                Managers.Sound.managerAudioSource.Pause();
            }
            currentPlayValue -= 0.005f;
            if (currentPlayValue < 0)
                currentPlayValue = 0;
            OnPlayValueChanged(false);
        }
        if (scrollDir == MouseScroll.Up && currentPlayValue <= 1)
        {
            if (_isGridScrolling)
            {
                _isGridScrolling = false;
                Managers.Sound.managerAudioSource.Pause();
            }
            currentPlayValue += 0.005f;
            if (currentPlayValue > 1)
                currentPlayValue = 1;
            OnPlayValueChanged(false);
        }
    }

    void EditorGridScroll()
    {
        if (!Managers.Sound.managerAudioSource.isPlaying)
        {
            Managers.Sound.managerAudioSource.timeSamples = (int)(currentPlayValue * Managers.Sound.managerAudioSource.clip.samples);
            if (Managers.Sound.managerAudioSource.timeSamples >= Managers.Sound.managerAudioSource.clip.samples)
            {
                Debug.LogWarning("Music time in PCM samples bigger than maximum value!");
                _isGridScrolling = false;
                return;
            }
            Managers.Sound.managerAudioSource.Play();
        }
        currentPlayValue = (float)Managers.Sound.managerAudioSource.timeSamples / Managers.Sound.managerAudioSource.clip.samples;
        OnPlayValueChanged(false);
    }

    public static bool isPlayValueChanged;
    void OnPlayValueChanged(bool callByUI)
    {
        _barInstatiatePoint.transform.localPosition = new Vector3(0, -currentPlayValue * _editorBarMaxPosition, 0);
        if (!callByUI)
            isPlayValueChanged = true;
    }

    void OnSettingValueChanged()
    {
        if (_musicPattern._bpm != baseBPM)
        {
            _musicPattern._bpm = baseBPM;
        }
        if (_musicPattern._songOffset != patternOffset)
        {
            _musicPattern._songOffset = patternOffset;
        }
    }
}
