using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Datas;
using static Define;

public class GameController : MonoBehaviour
{
    static GameController _gameInstance;
    static GameController GameInstance { get { return _gameInstance; } }
    GameObject gameNotePref;
    GameObject patternObject;
    Transform[] laneTransforms;
    SpriteRenderer[] lanePressEffects = new SpriteRenderer[4];

    float _barSampleAmount;
    double diff;
    double diffUpdatesAlways;

    public static bool isPlaying;

    public Queue<GameObject> firstLaneNotes = new Queue<GameObject>();
    public Queue<GameObject> secondLaneNotes = new Queue<GameObject>();
    public Queue<GameObject> thirdLaneNotes = new Queue<GameObject>();
    public Queue<GameObject> fourthLaneNotes = new Queue<GameObject>();

    public Queue<GameNote> firstLaneNoteDatas = new Queue<GameNote>();
    public Queue<GameNote> secondLaneNoteDatas = new Queue<GameNote>();
    public Queue<GameNote> thirdLaneNoteDatas = new Queue<GameNote>();
    public Queue<GameNote> fourthLaneNoteDatas = new Queue<GameNote>();

    public static Action<Judge, double> judgeAction = null;
    void Start()
    {
        gameNotePref = Resources.Load("Prefabs/GameNote") as GameObject;
        patternObject = GameObject.Find("Pattern");
        laneTransforms = new Transform[4];
        laneTransforms[0] = GameObject.Find("First").transform;
        laneTransforms[1] = GameObject.Find("Second").transform;
        laneTransforms[2] = GameObject.Find("Third").transform;
        laneTransforms[3] = GameObject.Find("Fourth").transform;
        lanePressEffects[0] = GameObject.Find("FirstLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[1] = GameObject.Find("SecondLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[2] = GameObject.Find("ThirdLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[3] = GameObject.Find("FourthLanePressEffect").GetComponent<SpriteRenderer>();

        Managers.Input.KeyDownAction -= PlayerKeyDown;
        Managers.Input.KeyDownAction += PlayerKeyDown;
        Managers.Input.KeyPressAction -= PlayerKeyPress;
        Managers.Input.KeyPressAction += PlayerKeyPress;
        Managers.Input.KeyUpAction -= PlayerKeyUp;
        Managers.Input.KeyUpAction += PlayerKeyUp;

        LoadPattern();
    }

    void Update()
    {
        if (isPlaying)
        {
            ScrollPattern();
            CheckMissingNote();
        }
    }

    void CheckMissingNote()
    {
        diffUpdatesAlways = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset);
        if (firstLaneNoteDatas.Count > 0)
        {
            if ((firstLaneNoteDatas.Peek().data._startTiming + diffUpdatesAlways) / 44100 < -0.16667)
            {
                firstLaneNoteDatas.Dequeue();
                JudgingInput(diffUpdatesAlways / 44100);
            }
        }
        if (secondLaneNoteDatas.Count > 0)
        {
            if ((secondLaneNoteDatas.Peek().data._startTiming + diffUpdatesAlways) / 44100 < -0.16667)
            {
                secondLaneNoteDatas.Dequeue();
                JudgingInput(diffUpdatesAlways / 44100);
            }
        }
        if (thirdLaneNoteDatas.Count > 0)
        {
            if ((thirdLaneNoteDatas.Peek().data._startTiming + diffUpdatesAlways) / 44100 < -0.16667)
            {
                thirdLaneNoteDatas.Dequeue();
                JudgingInput(diffUpdatesAlways / 44100);
            }
        }
        if (fourthLaneNoteDatas.Count > 0)
        {
            if ((fourthLaneNoteDatas.Peek().data._startTiming + diffUpdatesAlways) / 44100 < -0.16667)
            {
                fourthLaneNoteDatas.Dequeue();
                JudgingInput(diffUpdatesAlways / 44100);
            }
        }
    }

    public void PlayerKeyDown(KeyCode key)
    {
        diff = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset);
        switch (key)
        {
            case KeyCode.S:
                lanePressEffects[0].enabled = true;
                diff = (firstLaneNoteDatas.Peek().data._startTiming + diff) / 44100;
                if (diff > 0.2)
                    break;
                firstLaneNoteDatas.Dequeue();
                JudgingInput(diff);
                break;
            case KeyCode.D:
                lanePressEffects[1].enabled = true;
                diff = (secondLaneNoteDatas.Peek().data._startTiming + diff) / 44100;
                if (diff > 0.2)
                    break;
                secondLaneNoteDatas.Dequeue();
                JudgingInput(diff);
                break;
            case KeyCode.L:
                lanePressEffects[2].enabled = true;
                diff = (thirdLaneNoteDatas.Peek().data._startTiming + diff) / 44100;
                if (diff > 0.2)
                    break;
                thirdLaneNoteDatas.Dequeue();
                JudgingInput(diff);
                break;
            case KeyCode.Semicolon:
                lanePressEffects[3].enabled = true;
                diff = (fourthLaneNoteDatas.Peek().data._startTiming + diff) / 44100;
                if (diff > 0.2)
                    break;
                fourthLaneNoteDatas.Dequeue();
                JudgingInput(diff);
                break;
        }
    }

    public void PlayerKeyPress(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.S:
                break;
            case KeyCode.D:
                break;
            case KeyCode.L:
                break;
            case KeyCode.Semicolon:
                break;
        }
    }

    public void PlayerKeyUp(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.S:
                lanePressEffects[0].enabled = false;
                break;
            case KeyCode.D:
                lanePressEffects[1].enabled = false;
                break;
            case KeyCode.L:
                lanePressEffects[2].enabled = false;
                break;
            case KeyCode.Semicolon:
                lanePressEffects[3].enabled = false;
                break;
        }
    }

    void JudgingInput(double diff)
    {
        if (diff <= 0.04167 && diff >= -0.04167)
        {
            Debug.Log("Besta");
            judgeAction.Invoke(Judge.Besta, diff);
        }
        else if (diff <= 0.1 && diff >= -0.1)
        {
            Debug.Log("Good");
            judgeAction.Invoke(Judge.Good, diff);
        }
        else if (diff <= 0.16667 && diff >= -0.16667)
        {
            Debug.Log("Bad");
            judgeAction.Invoke(Judge.Bad, diff);
        }
        else
        {
            Debug.LogWarning("Miss");
            judgeAction.Invoke(Judge.Miss, diff);
        }
    }

    void ScrollPattern()
    {
        patternObject.transform.Translate(new Vector3(0, -Managers.Sound.managerAudioSource.clip.frequency / _barSampleAmount * 4.8f, 0) * 3 * Time.deltaTime);
        if (isPlaying && !Managers.Sound.managerAudioSource.isPlaying)
        {
            if (-patternObject.transform.localPosition.y / (4.8f * 3) >= 2 && (_barSampleAmount * ((-patternObject.transform.localPosition.y - (4.8f * 3 * 2)) / (4.8f * 3)) >= Managers.Game.currentLoadedPattern._songOffset))
            {
                Managers.Sound.managerAudioSource.timeSamples = (int)(_barSampleAmount * ((-patternObject.transform.localPosition.y - (4.8f * 3 * 2)) / (4.8f * 3)) - Managers.Game.currentLoadedPattern._songOffset);
                Managers.Sound.managerAudioSource.Play();
            }
        }
    }

    public void LoadPattern()
    {
        _barSampleAmount = (Managers.Game.currentLoadedPattern._musicSource.frequency / (Managers.Game.currentLoadedPattern._bpm / (float)60)) * 4;
        foreach (Note n in Managers.Game.currentLoadedPattern._noteDatas)
        {
            GameObject currentNote = null;
            switch (n._laneNumber)  // 레인별로 입력을 처리하기 위해 따로 큐에 저장한 후 선입선출 방식으로 처리
            {
                case LaneNumber.First:
                    currentNote = Instantiate(gameNotePref, laneTransforms[0]);
                    break;
                case LaneNumber.Second:
                    currentNote = Instantiate(gameNotePref, laneTransforms[1]);
                    break;
                case LaneNumber.Third:
                    currentNote = Instantiate(gameNotePref, laneTransforms[2]);
                    break;
                case LaneNumber.Fourth:
                    currentNote = Instantiate(gameNotePref, laneTransforms[3]);
                    break;
            }
            // yPos 값을 계산하는 수식의 마지막 인수는 유저의 개인 설정 스크롤 속도가 되어야 한다. 임시 방편으로 보기 좋기 위해 3으로 설정해 뒀으므로 관련 수정 필요.
            float yPos = (n._startTiming / _barSampleAmount * 4.8f * 3) + 4.8f * 3 * 2;
            currentNote.transform.localPosition = new Vector2(0, yPos + 0.125f);
            if (n._isLongNote)
            {
                float legacyPos = yPos;
                yPos = (n._endTiming / _barSampleAmount * 4.8f * 3) + 4.8f * 3 * 2;
                GameNote tempGameNoteData = currentNote.GetComponent<GameNote>();
                tempGameNoteData.longNotePole.SetActive(true);
                tempGameNoteData.longNotePole.SetActive(true);
                tempGameNoteData.endPoint.SetActive(true);
                tempGameNoteData.endPoint.transform.localPosition = new Vector2(0, yPos - legacyPos);
                tempGameNoteData.ResizePole();
            }
            currentNote.GetComponent<GameNote>().data = n;

            switch (n._laneNumber)  
            {
                case LaneNumber.First:
                    firstLaneNotes.Enqueue(currentNote);
                    firstLaneNoteDatas.Enqueue(currentNote.GetComponent<GameNote>());
                    break;
                case LaneNumber.Second:
                    secondLaneNotes.Enqueue(currentNote);
                    secondLaneNoteDatas.Enqueue(currentNote.GetComponent<GameNote>());
                    break;
                case LaneNumber.Third:
                    thirdLaneNotes.Enqueue(currentNote);
                    thirdLaneNoteDatas.Enqueue(currentNote.GetComponent<GameNote>());
                    break;
                case LaneNumber.Fourth:
                    fourthLaneNotes.Enqueue(currentNote);
                    fourthLaneNoteDatas.Enqueue(currentNote.GetComponent<GameNote>());
                    break;
            }
        }
    }
}
