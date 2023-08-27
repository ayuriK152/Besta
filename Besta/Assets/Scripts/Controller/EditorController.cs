using SFB;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Datas;
using static Define;

public class EditorController : MonoBehaviour
{

    public Action<int, int, int, bool> NoteCreateAction;
    public static Action<Beat> BeatChangeAction;
    public static Action<bool> PlayValueChangeAction;
    public static Action PatternSettingChangeAction;
    public static Action PatternSaveAction;
    public static Action<string> PatternLoadAction;
    public static Action PatternCreateAction;
    public static Action PatternTitleAction;
    public static Action PatternMaxComboUpdateAction;

    public static EditorNoteMode editorNoteMode;
    public static Beat editorBeat;
    public static float currentPlayValue;       // 노래 파일의 샘플 레이트 기준 재생 시간 측정용
    public static int baseBPM;
    public static int patternOffset;
    public static int totalCombo = 0;
    public static string patternTitle;
    public static string patternArtist;
    public static string patternDesigner;

    MusicPattern _musicPattern;
    double _noteTimingValue;
    double _timingValuePerBar;
    int _barAmount;
    float _editorBarMaxPosition;

    GameObject _noteInstantiatePoint;
    GameObject _barInstatiatePoint;
    GameObject _editorNote;
    GameObject _editorBar;
    GameObject _currentNote;
    List<GameObject> _instantiatedEditorBars = new List<GameObject>();
    List<GameObject> _instantiatedEditorNotes = new List<GameObject>();
    EditorUI uiScript;

    public static Vector3 barUpperLimitPos;
    public static Vector3 barLowerLimitPos;
    public static bool isPlayValueChanged;
    public static bool isGridScrolling;
    int beatDivideNum;

    void Start()
    {
        editorNoteMode = EditorNoteMode.NormalNote;
        editorBeat = Beat.Eight;
        beatDivideNum = 8;
        _musicPattern = null;
        isPlayValueChanged = false;
        isGridScrolling = false;

        // 바인딩
        barUpperLimitPos = GameObject.Find("UpperLimit").transform.position;
        barLowerLimitPos = GameObject.Find("LowerLimit").transform.position;
        _noteInstantiatePoint = GameObject.Find("Notes");
        _barInstatiatePoint = GameObject.Find("Grid");
        _editorNote = Resources.Load<GameObject>("Prefabs/EditorNote");
        _editorBar = Resources.Load<GameObject>("Prefabs/EditorBar");
        uiScript = Managers.UI.currentSceneUI as EditorUI;

        // 대리자 초기화
        ActionInit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isGridScrolling && Managers.Sound.managerAudioSource.clip == null)
            {
                Debug.LogWarning("No music selected");
                return;
            }
            isGridScrolling = !isGridScrolling;
            if (!isGridScrolling)
                Managers.Sound.managerAudioSource.Pause();
        }
        if (isGridScrolling)
            EditorGridScroll();
    }

    void Init()
    {
        // 채보 판정 타이밍과 같은 수치 설정
        Managers.Sound.managerAudioSource.clip = _musicPattern.musicSource;
        Managers.Sound.managerAudioSource.timeSamples = 0;
        _timingValuePerBar = (_musicPattern.musicSource.frequency / (_musicPattern.bpm / (double)60)) * 4;
        _noteTimingValue = _timingValuePerBar / beatDivideNum;
        _barAmount = (int)(_musicPattern.songLength / _timingValuePerBar) + 1;
        _editorBarMaxPosition = Managers.Sound.managerAudioSource.clip.samples * 4.8f / ((float)_timingValuePerBar);
        currentPlayValue = 0;
        baseBPM = _musicPattern.bpm;
        patternOffset = _musicPattern.songOffset;

        BeatChangeAction = null;
        for (int i = 0; i < _barAmount; i++)
        {
            _instantiatedEditorBars.Add(Instantiate(_editorBar, _barInstatiatePoint.transform));
            _instantiatedEditorBars[i].transform.localPosition = new Vector3(0, i * 4.8f, 0);
            _instantiatedEditorBars[i].name = _editorBar.name + " " + (i + 1);
            _instantiatedEditorBars[i].GetComponent<EditorBar>().barIndex = i;
        }
    }

    void ActionInit()
    {
        NoteCreateAction = null;
        BeatChangeAction = null;
        BeatChangeAction -= OnBeatChange;
        BeatChangeAction += OnBeatChange;
        Managers.Input.MouseAction -= EditorMouseEvent;
        Managers.Input.MouseAction += EditorMouseEvent;
        Managers.Input.ScrollAction -= EditorMouseScrollEvent;
        Managers.Input.ScrollAction += EditorMouseScrollEvent;
        PlayValueChangeAction -= OnPlayValueChanged;
        PlayValueChangeAction += OnPlayValueChanged;
        PatternSettingChangeAction -= OnSettingValueChanged;
        PatternSettingChangeAction += OnSettingValueChanged;
        PatternSaveAction -= SaveMusicPatternData;
        PatternSaveAction += SaveMusicPatternData;
        PatternLoadAction -= LoadMusicPatternData;
        PatternLoadAction += LoadMusicPatternData;
        PatternCreateAction -= CreateMusicPattern;
        PatternCreateAction += CreateMusicPattern;
    }

    void NoteInstantiateOnMouseButtonDown(Ray2D ray, RaycastHit2D hit)      // 왼쪽 마우스 버튼 down시 노트 입력 시작
    {
        if (hit.collider == null)
            return;

        if (hit.collider.tag == "EditorNote")       // 이미 노트가 있는 부분에 중복 입력을 시도한 경우, 해당 노트 삭제후 입력 시작
        {
            Note targetNoteData = hit.collider.transform.parent.GetComponent<EditorNote>().noteData;
            if (!_musicPattern.noteDatas.Remove(targetNoteData))
            {
                Debug.LogError("Note delete while initiating new note does not functioning!");
                return;
            }

            _instantiatedEditorNotes.Remove(hit.collider.transform.parent.gameObject);
            Destroy(hit.collider.transform.parent.gameObject);
            Debug.Log($"Note Deleted. Combo Update: {UpdateComboCount(targetNoteData, false)}, Total: {_musicPattern.totalCombo}");
            hit = Physics2D.Raycast(ray.origin, ray.direction);
        }

        if (hit.collider == null)
            return;

        if (hit.collider.tag == "EditorCollider")   // 생성부 클릭시 노트 생성 및 데이터 입력
        {
            Transform editorCollider = hit.collider.transform;
            Transform editorColliderParent = editorCollider.parent;
            _currentNote = Instantiate(_editorNote, new Vector3(editorCollider.position.x, editorCollider.position.y, -2), editorCollider.rotation, _noteInstantiatePoint.transform);
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
            double tempTiming = ((currentBarIndex * beatDivideNum) + (editorColliderParent.transform.localPosition.y / (4.8f / beatDivideNum))) * _noteTimingValue;
            Note tempNoteData = new Note(tempNoteLaneNum, (tempTiming - (int)tempTiming) >= 0.5f ? (int)tempTiming + 1 : (int)tempTiming, 0, false);
            if (editorNoteMode == EditorNoteMode.LongNote)
            {
                tempNoteData.endTiming = tempNoteData.startTiming;
                tempNoteData.isLongNote = true;
                _currentNote.GetComponent<EditorNote>().longNotePole.SetActive(true);
                _currentNote.GetComponent<EditorNote>().endPoint.SetActive(true);
            }
            _currentNote.GetComponent<EditorNote>().noteData = tempNoteData;
            _instantiatedEditorNotes.Add(_currentNote);
            Debug.Log("Note Instantiated");
        }
    }
    void NoteInstantiateOnMouseButtonPress(RaycastHit2D hit)                // 왼쪽 마우스 번튼 press, 롱노트 모드인 경우
    {
        if (hit.collider == null)
            return;
        if (hit.collider.tag == "EditorCollider" && editorNoteMode == EditorNoteMode.LongNote && _currentNote != null)
        {
            Transform editorCollider = hit.collider.transform;
            Transform editroColliderParent = editorCollider.parent;

            int currentBarIndex;
            if (editroColliderParent.name != "Base")
                currentBarIndex = editroColliderParent.parent.parent.GetComponent<EditorBar>().barIndex;
            else
                currentBarIndex = editroColliderParent.parent.GetComponent<EditorBar>().barIndex;

            double tempTiming = ((currentBarIndex * beatDivideNum) + (editroColliderParent.transform.localPosition.y / (4.8f / beatDivideNum))) * _noteTimingValue;
            _currentNote.GetComponent<EditorNote>().noteData.endTiming = (tempTiming - (int)tempTiming) >= 0.5f ? (int)tempTiming + 1 : (int)tempTiming;
            _currentNote.GetComponent<EditorNote>().endPoint.transform.localPosition = new Vector2(0, editorCollider.position.y - _currentNote.transform.position.y);
            _currentNote.GetComponent<EditorNote>().ResizePole();
        }
    }
    void NoteInstantiateOnMouseButtonUp()
    {
        if (_currentNote != null)
        {
            EditorNote currentNoteData = _currentNote.GetComponent<EditorNote>();
            if (editorNoteMode == EditorNoteMode.LongNote)
            {
                if (currentNoteData.noteData.startTiming == currentNoteData.noteData.endTiming)
                {
                    currentNoteData.noteData.endTiming = 0;
                    currentNoteData.noteData.isLongNote = false;
                }
                else
                    currentNoteData.longNotePole.GetComponent<BoxCollider2D>().enabled = true;
            }
            _musicPattern.noteDatas.Add(currentNoteData.noteData);
            Debug.Log($"Note instatiated. Combo Update: {UpdateComboCount(currentNoteData.noteData, true)}, Total: {_musicPattern.totalCombo}");
            _currentNote = null;
        }
    }

    void NoteDeleteOnMouseButtonDown(RaycastHit2D hit)
    {
        if (hit.collider == null)
            return;
        if (hit.collider.tag == "EditorNote")
        {
            Note targetNoteData = hit.collider.transform.parent.GetComponent<EditorNote>().noteData;
            if (!_musicPattern.noteDatas.Remove(targetNoteData))
            {
                Debug.LogError("Note delete does not functioning!");
                return;
            }
            _instantiatedEditorNotes.Remove(hit.collider.transform.parent.gameObject);
            Destroy(hit.collider.transform.parent.gameObject);
            Debug.Log($"Note Deleted. Combo Update: {UpdateComboCount(targetNoteData, false)}, Total: {_musicPattern.totalCombo}");
        }
    }

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
        if (_musicPattern == null)
            return;
        if (scrollDir == MouseScroll.Down && currentPlayValue >= 0)
        {
            if (isGridScrolling)
            {
                isGridScrolling = false;
                Managers.Sound.managerAudioSource.Pause();
            }
            currentPlayValue -= 1 / (float)(_barAmount * 4);
            if (currentPlayValue < 0)
                currentPlayValue = 0;
            OnPlayValueChanged(false);
        }
        if (scrollDir == MouseScroll.Up && currentPlayValue <= 1)
        {
            if (isGridScrolling)
            {
                isGridScrolling = false;
                Managers.Sound.managerAudioSource.Pause();
            }
            currentPlayValue += 1 / (float)(_barAmount * 4);
            if (currentPlayValue > 1)
                currentPlayValue = 1;
            OnPlayValueChanged(false);
        }
    }

    void EditorGridScroll()
    {
        currentPlayValue = (float)Managers.Sound.managerAudioSource.timeSamples / Managers.Sound.managerAudioSource.clip.samples;
        _barInstatiatePoint.transform.Translate(new Vector3(0, -(Managers.Sound.managerAudioSource.clip.frequency / (float)_timingValuePerBar) * 4.8f, 0) * Time.deltaTime);
        OnPlayValueChanged(false);
        if (!Managers.Sound.managerAudioSource.isPlaying)
        {
            if (currentPlayValue == 1)
            {
                isGridScrolling = false;
                isPlayValueChanged = false;
                Managers.Sound.managerAudioSource.timeSamples = Managers.Sound.managerAudioSource.clip.samples - 1;
                return;
            }
            else
            {
                if (Managers.Sound.managerAudioSource.timeSamples >= Managers.Sound.managerAudioSource.clip.samples)
                {
                    Debug.LogWarning("Music time in PCM samples bigger than maximum value!");
                    isGridScrolling = false;
                    return;
                }
                Managers.Sound.managerAudioSource.Play();
            }
        }
    }

    void OnPlayValueChanged(bool callByUI)
    {
        if (!isGridScrolling)
            _barInstatiatePoint.transform.localPosition = new Vector3(0, -currentPlayValue * _editorBarMaxPosition - (patternOffset / ((float)_timingValuePerBar)) * 4.8f, 0);
        if (!callByUI)
            isPlayValueChanged = true;
        else
        {
            float tempSampleFloat = currentPlayValue * Managers.Sound.managerAudioSource.clip.samples;
            int tempSample = tempSampleFloat - (int)tempSampleFloat > 0.5f ? (int)tempSampleFloat + 1 : (int)tempSampleFloat;
            if (currentPlayValue != 1)
                Managers.Sound.managerAudioSource.timeSamples = tempSample;
            else
                Managers.Sound.managerAudioSource.timeSamples = tempSample - 1;
        }
        uiScript.TimeProgressUIUpdate();
    }

    void OnSettingValueChanged()
    {
        if (_musicPattern == null)
        {
            Debug.LogWarning("BPM or Offset can't be change with out create or load any pattern!");
            baseBPM = 0;
            patternOffset = 0;
            EditorUI uiScript = GameObject.Find("BaseCanvas").GetComponent<EditorUI>();
            uiScript.OnOffsetChangeByController();
            uiScript.OnBaseBPMChangeByController();
            return;
        }
        // BPM 변경시
        if (_musicPattern.bpm != baseBPM)
        {
            int legacyBpm = _musicPattern.bpm;
            _musicPattern.bpm = baseBPM;
            double updateRatio = (double)legacyBpm / _musicPattern.bpm;
            _timingValuePerBar = (_musicPattern.musicSource.frequency / (_musicPattern.bpm / (double)60)) * 4;
            _noteTimingValue = _timingValuePerBar / beatDivideNum;
            _editorBarMaxPosition = Managers.Sound.managerAudioSource.clip.samples * 4.8f / ((float)_timingValuePerBar);
            if (_barAmount != (int)(_musicPattern.songLength / _timingValuePerBar) + 1)
                OnBarAmountChanged();

            for (int i = 0; i < _instantiatedEditorNotes.Count; i++)
            {
                double tempStartTiming = _instantiatedEditorNotes[i].GetComponent<EditorNote>().noteData.startTiming * updateRatio;
                _instantiatedEditorNotes[i].GetComponent<EditorNote>().noteData.startTiming = tempStartTiming - (int)tempStartTiming >= 0.5f ? (int)tempStartTiming + 1 : (int)tempStartTiming;
                if (_instantiatedEditorNotes[i].GetComponent<EditorNote>().noteData.startTiming >= _musicPattern.songLength)
                {
                    Destroy(_instantiatedEditorNotes[i]);
                    _instantiatedEditorNotes[i] = null;
                    continue;
                }
                if (_instantiatedEditorNotes[i].GetComponent<EditorNote>().noteData.isLongNote)
                {
                    double tempEndTiming = _instantiatedEditorNotes[i].GetComponent<EditorNote>().noteData.endTiming * updateRatio;
                    _instantiatedEditorNotes[i].GetComponent<EditorNote>().noteData.endTiming = tempEndTiming - (int)tempEndTiming >= 0.5f ? (int)tempEndTiming + 1 : (int)tempEndTiming;
                    if (_instantiatedEditorNotes[i].GetComponent<EditorNote>().noteData.endTiming >= _musicPattern.songLength)
                    {
                        Destroy(_instantiatedEditorNotes[i]);
                        _instantiatedEditorNotes[i] = null;
                    }
                }
            }

            for (int i = 0; i < _instantiatedEditorNotes.Count; i++)
            {
                if (_instantiatedEditorNotes[i] == null)
                {
                    _instantiatedEditorNotes.RemoveAt(i);
                    i--;
                }
            }
            OnPlayValueChanged(false);
        }

        // 오프셋 변경시
        if (_musicPattern.songOffset != patternOffset)
        {
            _musicPattern.songOffset = patternOffset;
            _musicPattern.songLength = _musicPattern.musicSource.samples + _musicPattern.songOffset;
            if (_barAmount != (int)(_musicPattern.songLength / _timingValuePerBar) + 1)
                OnBarAmountChanged();
            OnPlayValueChanged(false);
        }

        // 패턴 이름 변경시
        if (_musicPattern.name != patternTitle)
        {
            if (Managers.Data.ChangePatternDirectoryName(_musicPattern.name, patternTitle))
            {
                _musicPattern.name = patternTitle;
                SaveMusicPatternData();
            }
            else
            {
                Debug.LogError("Pattern title update fail!");
                EditorUI uiScript = GameObject.Find("BaseCanvas").GetComponent<EditorUI>();
                uiScript.OnTitleUpdateByController();
            }
        }

        // 노래 아티스트 변경시
        if (_musicPattern.artist != patternArtist)
        {
            _musicPattern.artist = patternArtist;
        }

        // 패턴 디자이너 변경시
        if (_musicPattern.designer != patternDesigner)
        {
            _musicPattern.designer = patternDesigner;
        }
    }

    void OnBarAmountChanged()
    {
        if (_barAmount < ((int)(_musicPattern.songLength / _timingValuePerBar) + 1))   // BPM or 오프셋 조절에 따라 채보 길이가 늘어나는 경우
        {
            Debug.Log($"Bar amount increased! Before: {_barAmount}, After: {(int)(_musicPattern.songLength / _timingValuePerBar) + 1}");
            for (int i = _barAmount; i < (int)(_musicPattern.songLength / _timingValuePerBar) + 1; i++)
            {
                _instantiatedEditorBars.Add(Instantiate(_editorBar, _barInstatiatePoint.transform));
                _instantiatedEditorBars[i].transform.localPosition = new Vector3(0, i * 4.8f, 0);
                _instantiatedEditorBars[i].name = _editorBar.name + " " + (i + 1);
                _instantiatedEditorBars[i].GetComponent<EditorBar>().barIndex = i;
            }
            _barAmount = _instantiatedEditorBars.Count;
        }

        else if (_barAmount > ((int)(_musicPattern.songLength / (_noteTimingValue * 16)) + 1))  // BPM or 오프셋 조절에 따라 채보 길이가 줄어드는 경우
        {
            Debug.Log($"Bar amount decreased! Before: {_barAmount}, After: {(int)(_musicPattern.songLength / _timingValuePerBar) + 1}");
            for (int i = _barAmount - 1; i >= (int)(_musicPattern.songLength / _timingValuePerBar) + 1; i--)
            {
                GameObject temp = _instantiatedEditorBars[i];
                Destroy(temp);
                _instantiatedEditorBars.RemoveAt(i);
            }
            _barAmount = _instantiatedEditorBars.Count;
        }
    }

    void SaveMusicPatternData()
    {
        if (_musicPattern == null)
        {
            Debug.LogWarning("You tried to save pattern with out create or load any pattern!");
            return;
        }
        _musicPattern.noteDatas.Sort();
        Managers.Data.SavePatternAsJson(_musicPattern, _musicPattern.name);
    }

    void CreateMusicPattern()
    {
        if (_musicPattern != null )
        {
            // 이미 작업중이던 채보가 존재하는 경우 해당 채보를 저장할 것인지 물어본 후 저장 여부를 결정하는 기능 추가 요망
        }
#if UNITY_EDITOR
        string musicFilePath = EditorUtility.OpenFilePanel("Choose music file", "", "mp3");
#elif UNITY_STANDALONE_WIN
        string musicFilePath = StandaloneFileBrowser.OpenFilePanel("Choose music file", "", "mp3", false)[0];
#endif
        string[] directorys = musicFilePath.Split("/");
        Managers.Data.CopyMusicFilesWithCheckDirectory(musicFilePath);
        _musicPattern = new MusicPattern(directorys[directorys.Length - 1].Split(".")[0]);
        Init();
        patternTitle = _musicPattern.name;
        uiScript.OnOffsetChangeByController();
        uiScript.OnBaseBPMChangeByController();
        uiScript.OnTitleUpdateByController();
        uiScript.OriginTimeUIUpdate();
        Managers.Data.SavePatternAsJson(_musicPattern, _musicPattern.name);
    }

    void LoadMusicPatternData(string path)
    {
        MusicPattern tempPatternData = Managers.Data.LoadJsonData<MusicPattern>(path);
        if (tempPatternData == null)
        {
            Debug.LogError("Pattern load failed!");
            return;
        }
        _musicPattern = tempPatternData;
        _musicPattern.ReloadMusic();
        _musicPattern.totalCombo = 0;
        patternTitle = _musicPattern.name;
        patternArtist = _musicPattern.artist;
        patternDesigner = _musicPattern.designer;

        // 기존 작업물 내용 삭제 과정
        for (int i = _barAmount - 1; i >= 0; i--)
        {
            GameObject temp = _instantiatedEditorBars[i];
            Destroy(temp);
            _instantiatedEditorBars.RemoveAt(i);
        }
        foreach (GameObject n in _instantiatedEditorNotes)
        {
            Destroy(n);
        }
        _instantiatedEditorNotes.Clear();
        _barAmount = 0;
        Init();

        // 노트 데이터 로드 및 생성
        foreach (Note data in _musicPattern.noteDatas)
        {
            UpdateComboCount(data, true);

            float xPos = 0, yPos = 0;
            switch (data.laneNumber)
            {
                case LaneNumber.First:
                    xPos = -1.875f;
                    break;
                case LaneNumber.Second:
                    xPos = -0.625f;
                    break;
                case LaneNumber.Third:
                    xPos = 0.625f;
                    break;
                case LaneNumber.Fourth:
                    xPos = 1.875f;
                    break;
            }
            yPos = data.startTiming / ((float)_timingValuePerBar) * 4.8f;
            _currentNote = Instantiate(_editorNote, _noteInstantiatePoint.transform);
            _currentNote.transform.localPosition = new Vector2(xPos, yPos + 0.125f);
            if (data.isLongNote)
            {
                float legacyPos = yPos;
                yPos = data.endTiming / ((float)_timingValuePerBar) * 4.8f;
                EditorNote tempEditorNoteData = _currentNote.GetComponent<EditorNote>();
                tempEditorNoteData.longNotePole.SetActive(true);
                tempEditorNoteData.longNotePole.SetActive(true);
                tempEditorNoteData.endPoint.SetActive(true);
                tempEditorNoteData.endPoint.transform.localPosition = new Vector2(0, yPos - legacyPos);
                tempEditorNoteData.ResizePole();
                tempEditorNoteData.longNotePole.GetComponent<BoxCollider2D>().enabled = true;
            }
            _currentNote.GetComponent<EditorNote>().noteData = data;
            _instantiatedEditorNotes.Add(_currentNote);
            _currentNote = null;
        }

        Debug.Log($"{_musicPattern.totalCombo}");
        uiScript.OnOffsetChangeByController();
        uiScript.OnBaseBPMChangeByController();
        uiScript.OnTitleUpdateByController();
        uiScript.OnArtistUpdateByController();
        uiScript.OnDesignerUpdateByController();
        uiScript.OriginTimeUIUpdate();
        OnPlayValueChanged(false);
    }

    int UpdateComboCount(Note n, bool isIncrease)
    {
        int updateAmount = 0;
        if (!n.isLongNote)
            updateAmount = 1;

        else if (n.isLongNote)
            updateAmount = 2;
        if (!isIncrease)
            updateAmount = -updateAmount;

        _musicPattern.totalCombo += updateAmount;
        totalCombo = _musicPattern.totalCombo;
        PatternMaxComboUpdateAction.Invoke();
        return updateAmount;
    }

    void OnBeatChange(Beat beat)
    {
        switch (beat)
        {
            case Beat.One:
                beatDivideNum = 1;
                break;
            case Beat.Two:
                beatDivideNum = 2;
                break;
            case Beat.Three:
                beatDivideNum = 3;
                break;
            case Beat.Four:
                beatDivideNum = 4;
                break;
            case Beat.Six:
                beatDivideNum = 6;
                break;
            case Beat.Eight:
                beatDivideNum = 8;
                break;
            case Beat.Nine:
                beatDivideNum = 12;
                break;
            case Beat.Sixteen:
                beatDivideNum = 16;
                break;
        }
        _noteTimingValue = _timingValuePerBar / beatDivideNum;
    }
}
