using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Datas;
using static Define;

public class GameController : MonoBehaviour
{
    GameObject gameNotePref;
    GameObject patternObject;
    Transform[] laneTransforms = new Transform[4];
    SpriteRenderer[] lanePressEffects = new SpriteRenderer[4];

    float _barSampleAmount;
    double diff;
    double diffUpdatesAlways;
    float[] holdingSampleAmout = new float[4];
    int gainedAcc = 0, perfectAcc = 0;
    int passedNote;

    public static bool isPlaying;

    public Queue<GameObject>[] laneNotes = new Queue<GameObject>[4];
    public Queue<GameNote>[] laneNoteDatas = new Queue<GameNote>[4];

    public static Action<Judge, double> JudgeAction = null;
    public static Action ScoreUpdateAction = null;
    void Start()
    {
        gameNotePref = Resources.Load("Prefabs/GameNote") as GameObject;
        patternObject = GameObject.Find("Pattern");
        laneTransforms[0] = GameObject.Find("First").transform;
        laneTransforms[1] = GameObject.Find("Second").transform;
        laneTransforms[2] = GameObject.Find("Third").transform;
        laneTransforms[3] = GameObject.Find("Fourth").transform;
        lanePressEffects[0] = GameObject.Find("FirstLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[1] = GameObject.Find("SecondLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[2] = GameObject.Find("ThirdLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[3] = GameObject.Find("FourthLanePressEffect").GetComponent<SpriteRenderer>();
        for (int i = 0; i < 4; i++)
        {
            laneNotes[i] = new Queue<GameObject>();
            laneNoteDatas[i] = new Queue<GameNote>();
        }
        Array.Fill(holdingSampleAmout, 0);

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
        if (laneNoteDatas[0].Count > 0)
            CheckMissingNote((int)LaneNumber.First);
        if (laneNoteDatas[1].Count > 0)
            CheckMissingNote((int)LaneNumber.Second);
        if (laneNoteDatas[2].Count > 0)
            CheckMissingNote((int)LaneNumber.Third);
        if (laneNoteDatas[3].Count > 0)
            CheckMissingNote((int)LaneNumber.Fourth);
    }

    void CheckMissingNote(int lane)
    {
        lane -= 1;
        diffUpdatesAlways = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern.songOffset);
        if (!laneNoteDatas[lane].Peek().data.isLongNote && (laneNoteDatas[lane].Peek().data.startTiming + diffUpdatesAlways) / Managers.Sound.managerAudioSource.clip.frequency < -0.16667)
        {
            Destroy(laneNotes[lane].Dequeue());
            StartCoroutine(JudgingInput((laneNoteDatas[lane].Dequeue().data.startTiming + diffUpdatesAlways) / Managers.Sound.managerAudioSource.clip.frequency));
        }
        else if (laneNoteDatas[lane].Peek().data.isLongNote)
        {
            if ((laneNoteDatas[lane].Peek().data.endTiming + diffUpdatesAlways) / Managers.Sound.managerAudioSource.clip.frequency < -0.2)
            {
                holdingSampleAmout[lane] = 0;
                Destroy(laneNotes[lane].Dequeue());
                StartCoroutine(JudgingInput((laneNoteDatas[lane].Dequeue().data.endTiming + diffUpdatesAlways) / Managers.Sound.managerAudioSource.clip.frequency));
            }
            else if (((laneNoteDatas[lane].Peek().data.startTiming + diffUpdatesAlways) / Managers.Sound.managerAudioSource.clip.frequency < -0.16667 && holdingSampleAmout[lane] == 0) ||
                (((holdingSampleAmout[lane] - (int)holdingSampleAmout[lane] > 0.5f ? (int)holdingSampleAmout[lane] + 1 : holdingSampleAmout[lane]) + diffUpdatesAlways) / Managers.Sound.managerAudioSource.clip.frequency < -0.16667) && holdingSampleAmout[lane] > 0 && laneNoteDatas[lane].Peek().data.endTiming > holdingSampleAmout[lane])
            {
                if (holdingSampleAmout[lane] == 0)
                {
                    holdingSampleAmout[lane] = laneNoteDatas[lane].Peek().data.startTiming;
                    StartCoroutine(JudgingInput((laneNoteDatas[lane].Peek().data.startTiming + diffUpdatesAlways) / Managers.Sound.managerAudioSource.clip.frequency));
                    holdingSampleAmout[lane] += _barSampleAmount * 0.25f;
                }
                else
                {
                    StartCoroutine(JudgingInput(((holdingSampleAmout[lane] - (int)holdingSampleAmout[lane] > 0.5f ? (int)holdingSampleAmout[lane] + 1 : holdingSampleAmout[lane]) + diffUpdatesAlways) / Managers.Sound.managerAudioSource.clip.frequency));
                    holdingSampleAmout[lane] += _barSampleAmount * 0.125f;
                }
            }
        }
    }

    public void PlayerKeyDown(KeyCode key)
    {
        diff = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern.songOffset);
        switch (key)
        {
            case KeyCode.S:
                NoteProcessKeyDown((int)LaneNumber.First, diff);
                break;
            case KeyCode.D:
                NoteProcessKeyDown((int)LaneNumber.Second, diff);
                break;
            case KeyCode.L:
                NoteProcessKeyDown((int)LaneNumber.Third, diff);
                break;
            case KeyCode.Semicolon:
                NoteProcessKeyDown((int)LaneNumber.Fourth, diff);
                break;
        }
    }

    void NoteProcessKeyDown(int lane, double timingDiff)
    {
        lane -= 1;
        lanePressEffects[lane].enabled = true;
        timingDiff = (laneNoteDatas[lane].Peek().data.startTiming + timingDiff) / Managers.Sound.managerAudioSource.clip.frequency;
        if (timingDiff > 0.2)
            return;
        if (!laneNoteDatas[lane].Peek().data.isLongNote)
        {
            laneNoteDatas[lane].Dequeue();
            Destroy(laneNotes[lane].Dequeue());
        }
        else
        {
            holdingSampleAmout[lane] = laneNoteDatas[lane].Peek().data.startTiming;
            holdingSampleAmout[lane] += _barSampleAmount * 0.25f;
        }
        StartCoroutine(JudgingInput(timingDiff));
    }

    public void PlayerKeyPress(KeyCode key)
    {
        int timingPoint = Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern.songOffset;
        switch (key)
        {
            case KeyCode.S:
                LongNoteProcessKeyPress((int)LaneNumber.First, timingPoint);
                break;
            case KeyCode.D:
                LongNoteProcessKeyPress((int)LaneNumber.Second, timingPoint);
                break;
            case KeyCode.L:
                LongNoteProcessKeyPress((int)LaneNumber.Third, timingPoint);
                break;
            case KeyCode.Semicolon:
                LongNoteProcessKeyPress((int)LaneNumber.Fourth, timingPoint);
                break;
        }
    }

    void LongNoteProcessKeyPress(int lane, int timingPoint)
    {
        lane -= 1;
        if (holdingSampleAmout[lane] > 0)
        {
            if (laneNoteDatas[lane].Peek().data.endTiming <= timingPoint)
            {
                holdingSampleAmout[lane] = 0;
                Managers.Game.currentCombo += 1;
                JudgeAction.Invoke(Judge.None, diff);
                laneNoteDatas[lane].Dequeue();
                Destroy(laneNotes[lane].Dequeue());
                StartCoroutine(CalcScore());
            }
            else if (holdingSampleAmout[lane] <= timingPoint)
            {
                holdingSampleAmout[lane] += _barSampleAmount * 0.125f;
                Managers.Game.currentCombo += 1;
                JudgeAction.Invoke(Judge.None, diff);
                StartCoroutine(CalcScore());
            }
        }
    }

    public void PlayerKeyUp(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.S:
                StartCoroutine(CheckLongNoteKeyUp((int)LaneNumber.First));
                break;
            case KeyCode.D:
                StartCoroutine(CheckLongNoteKeyUp((int)LaneNumber.Second));
                break;
            case KeyCode.L:
                StartCoroutine(CheckLongNoteKeyUp((int)LaneNumber.Third));
                break;
            case KeyCode.Semicolon:
                StartCoroutine(CheckLongNoteKeyUp((int)LaneNumber.Fourth));
                break;
        }
    }

    IEnumerator CheckLongNoteKeyUp(int lane)
    {
        lane -= 1;
        lanePressEffects[lane].enabled = false;
        if (holdingSampleAmout[lane] > 0)
        {
            double tempDiff = (double)(laneNoteDatas[lane].Peek().data.endTiming - (Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern.songOffset)) / Managers.Sound.managerAudioSource.clip.frequency;
            if (tempDiff <= 0.16667 && tempDiff >= -0.16667)
            {
                holdingSampleAmout[lane] = 0;
                Managers.Game.currentCombo++;
                JudgeAction.Invoke(Judge.None, diff);
                laneNoteDatas[lane].Dequeue();
                Destroy(laneNotes[lane].Dequeue());
                StartCoroutine(CalcScore());
            }
        }
        yield return null;
    }

    IEnumerator CalcScore()
    {
        passedNote += 1;
        Managers.Game.acurracy = (double)gainedAcc / perfectAcc;
        Managers.Game.progressByCombo = (double)passedNote / Managers.Game.currentLoadedPattern.totalCombo;
        Managers.Game.gainedScore = (int)((1000000 * Managers.Game.progressByCombo) * Managers.Game.acurracy);
        Debug.Log($"{Managers.Game.gainedScore} {Managers.Game.acurracy} {Managers.Game.progressByCombo}");
        ScoreUpdateAction.Invoke();
        yield return null;
    }

    IEnumerator JudgingInput(double diff)
    {
        perfectAcc += 3;
        if (diff <= 0.04167 && diff >= -0.04167)
        {
            Debug.Log("Besta");
            gainedAcc += 3;
            Managers.Game.currentCombo += 1;
            JudgeAction.Invoke(Judge.Besta, diff);
        }
        else if (diff <= 0.1 && diff >= -0.1)
        {
            Debug.Log("Good");
            gainedAcc += 2;
            Managers.Game.currentCombo += 1;
            JudgeAction.Invoke(Judge.Good, diff);
        }
        else if (diff <= 0.16667 && diff >= -0.16667)
        {
            Debug.Log("Bad");
            gainedAcc += 1;
            Managers.Game.currentCombo += 1;
            JudgeAction.Invoke(Judge.Bad, diff);
        }
        else
        {
            Debug.LogWarning("Miss");
            gainedAcc += 0;
            Managers.Game.currentCombo = 0;
            JudgeAction.Invoke(Judge.Miss, diff);
        }
        StartCoroutine(CalcScore());
        yield return null;
    }

    void ScrollPattern()
    {
        patternObject.transform.Translate(new Vector3(0, -Managers.Sound.managerAudioSource.clip.frequency / _barSampleAmount * 4.8f, 0) * 3 * Time.deltaTime);
        if (isPlaying && !Managers.Sound.managerAudioSource.isPlaying)
        {
            if (-patternObject.transform.localPosition.y / (4.8f * 3) >= 2 && (_barSampleAmount * ((-patternObject.transform.localPosition.y - (4.8f * 3 * 2)) / (4.8f * 3)) >= Managers.Game.currentLoadedPattern.songOffset))
            {
                Managers.Sound.managerAudioSource.timeSamples = (int)(_barSampleAmount * ((-patternObject.transform.localPosition.y - (4.8f * 3 * 2)) / (4.8f * 3)) - Managers.Game.currentLoadedPattern.songOffset);
                Managers.Sound.managerAudioSource.Play();
            }
        }
    }

    public void LoadPattern()
    {
        _barSampleAmount = (Managers.Game.currentLoadedPattern.musicSource.frequency / (Managers.Game.currentLoadedPattern.bpm / (float)60)) * 4;
        foreach (Note n in Managers.Game.currentLoadedPattern.noteDatas)
        {
            GameObject currentNote = null;
            switch (n.laneNumber)  // 레인별로 입력을 처리하기 위해 따로 큐에 저장한 후 선입선출 방식으로 처리
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
            float yPos = (n.startTiming / _barSampleAmount * 4.8f * 3) + 4.8f * 3 * 2;
            currentNote.transform.localPosition = new Vector2(0, yPos + 0.125f);
            if (n.isLongNote)
            {
                float legacyPos = yPos;
                yPos = (n.endTiming / _barSampleAmount * 4.8f * 3) + 4.8f * 3 * 2;
                GameNote tempGameNoteData = currentNote.GetComponent<GameNote>();
                tempGameNoteData.longNotePole.SetActive(true);
                tempGameNoteData.longNotePole.SetActive(true);
                tempGameNoteData.endPoint.SetActive(true);
                tempGameNoteData.endPoint.transform.localPosition = new Vector2(0, yPos - legacyPos);
                tempGameNoteData.ResizePole();
            }
            currentNote.GetComponent<GameNote>().data = n;

            switch (n.laneNumber)  
            {
                case LaneNumber.First:
                    laneNotes[0].Enqueue(currentNote);
                    laneNoteDatas[0].Enqueue(currentNote.GetComponent<GameNote>());
                    break;
                case LaneNumber.Second:
                    laneNotes[1].Enqueue(currentNote);
                    laneNoteDatas[1].Enqueue(currentNote.GetComponent<GameNote>());
                    break;
                case LaneNumber.Third:
                    laneNotes[2].Enqueue(currentNote);
                    laneNoteDatas[2].Enqueue(currentNote.GetComponent<GameNote>());
                    break;
                case LaneNumber.Fourth:
                    laneNotes[3].Enqueue(currentNote);
                    laneNoteDatas[3].Enqueue(currentNote.GetComponent<GameNote>());
                    break;
            }
        }
    }
}
