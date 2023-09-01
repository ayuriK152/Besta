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
    GameObject judgeLineObject;
    Transform[] laneTransforms = new Transform[4];
    SpriteRenderer[] lanePressEffects = new SpriteRenderer[4];
    AudioSource[] lanePressSounds = new AudioSource[4];

    float _barSampleAmount;
    float[] holdingSampleAmout = new float[4];
    Judge[] longnoteStartJudge = new Judge[4];
    bool[] holdingCheck = new bool[4];
    int gainedAcc = 0, perfectAcc = 0;
    int passedNote = 0;
    int userOffset;
    float patternObjectPosOffset;
    float scrollSpeed;

    public static bool isPlaying;

    public Queue<GameObject>[] laneNotes = new Queue<GameObject>[4];
    public Queue<GameNote>[] laneNoteDatas = new Queue<GameNote>[4];

    public static Action<Judge, double> JudgeAction = null;
    public static Action ScoreUpdateAction = null;
    void Start()
    {
        gameNotePref = Resources.Load("Prefabs/GameNote") as GameObject;
        patternObject = GameObject.Find("Pattern");
        judgeLineObject = GameObject.Find("JudgeLine");
        judgeLineObject.transform.localPosition = new Vector2(0, -3.5f + PlayerPrefs.GetInt("JudgeLineHeight") / 20.0f);
        laneTransforms[0] = GameObject.Find("First").transform;
        laneTransforms[1] = GameObject.Find("Second").transform;
        laneTransforms[2] = GameObject.Find("Third").transform;
        laneTransforms[3] = GameObject.Find("Fourth").transform;
        lanePressEffects[0] = GameObject.Find("FirstLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[1] = GameObject.Find("SecondLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[2] = GameObject.Find("ThirdLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[3] = GameObject.Find("FourthLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressSounds[0] = GameObject.Find("FirstLanePressEffect").GetComponent<AudioSource>();
        lanePressSounds[1] = GameObject.Find("SecondLanePressEffect").GetComponent<AudioSource>();
        lanePressSounds[2] = GameObject.Find("ThirdLanePressEffect").GetComponent<AudioSource>();
        lanePressSounds[3] = GameObject.Find("FourthLanePressEffect").GetComponent<AudioSource>();
        lanePressSounds[0].volume = PlayerPrefs.GetInt("EffectSoundValue") / 100.0f;
        lanePressSounds[1].volume = PlayerPrefs.GetInt("EffectSoundValue") / 100.0f;
        lanePressSounds[2].volume = PlayerPrefs.GetInt("EffectSoundValue") / 100.0f;
        lanePressSounds[3].volume = PlayerPrefs.GetInt("EffectSoundValue") / 100.0f;

        for (int i = 0; i < 4; i++)
        {
            laneNotes[i] = new Queue<GameObject>();
            laneNoteDatas[i] = new Queue<GameNote>();
        }
        Array.Fill(holdingSampleAmout, 0);

        userOffset = PlayerPrefs.GetInt("UserOffset");
        scrollSpeed = PlayerPrefs.GetInt("SlideSpeed") / 10.0f;
        patternObjectPosOffset = 4.8f * (PlayerPrefs.GetInt("NotePosition") / 20.0f);
        patternObject.transform.localPosition = new Vector2(0, patternObjectPosOffset * scrollSpeed);

        Managers.Input.KeyDownAction = null;
        Managers.Input.KeyDownAction -= PlayerKeyDown;
        Managers.Input.KeyDownAction += PlayerKeyDown;
        Managers.Input.KeyPressAction = null;
        Managers.Input.KeyPressAction -= PlayerKeyPress;
        Managers.Input.KeyPressAction += PlayerKeyPress;
        Managers.Input.KeyUpAction = null;
        Managers.Input.KeyUpAction -= PlayerKeyUp;
        Managers.Input.KeyUpAction += PlayerKeyUp;
        Managers.Game.CheckGameEndAction = null;
        Managers.Game.CheckGameEndAction -= EndUpGame;
        Managers.Game.CheckGameEndAction += EndUpGame;

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

    void EndUpGame(bool isFullCombo)
    {
        Debug.Log($"Game ends! Full combo {isFullCombo}");
        StartCoroutine((Managers.UI.currentSceneUI as IngameUI).FadeOutGameScene());
    }

    void CheckMissingNote()
    {
        if (laneNoteDatas[0].Count > 0)
            StartCoroutine(CheckMissingNote((int)LaneNumber.First));
        if (laneNoteDatas[1].Count > 0)
            StartCoroutine(CheckMissingNote((int)LaneNumber.Second));
        if (laneNoteDatas[2].Count > 0)
            StartCoroutine(CheckMissingNote((int)LaneNumber.Third));
        if (laneNoteDatas[3].Count > 0)
            StartCoroutine(CheckMissingNote((int)LaneNumber.Fourth));
    }

    IEnumerator CheckMissingNote(int lane)
    {
        lane -= 1;
        double diffUpdatesAlways = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern.songOffset);
        double calculatedStartTimingDiff = (laneNoteDatas[lane].Peek().data.startTiming + diffUpdatesAlways) / Managers.Sound.managerAudioSource.clip.frequency;
        if (!laneNoteDatas[lane].Peek().data.isLongNote && calculatedStartTimingDiff < -(0.16667 + userOffset / 1000.0))
        {
            Destroy(laneNotes[lane].Dequeue());
            laneNoteDatas[lane].Dequeue();
            StartCoroutine(JudgingInput(calculatedStartTimingDiff + userOffset / 1000.0, lane));
        }
        else if (laneNoteDatas[lane].Peek().data.isLongNote)
        {
            double calculatedEndTimingDiff = (laneNoteDatas[lane].Peek().data.endTiming + diffUpdatesAlways) / Managers.Sound.managerAudioSource.clip.frequency;
            if (calculatedEndTimingDiff < -(0.16667 + userOffset / 1000.0))
            {
                Destroy(laneNotes[lane].Dequeue());
                laneNoteDatas[lane].Dequeue();
                holdingSampleAmout[lane] = 0;
                StartCoroutine(JudgingInput(calculatedEndTimingDiff + userOffset / 1000.0, lane));
            }
            else if (calculatedStartTimingDiff < -(0.16667 + userOffset / 1000.0) && holdingSampleAmout[lane] == 0)
            {
                passedNote += 1;
                perfectAcc += 3;
                Destroy(laneNotes[lane].Dequeue());
                laneNoteDatas[lane].Dequeue();
                holdingSampleAmout[lane] = 0;
                StartCoroutine(JudgingInput(calculatedStartTimingDiff + userOffset / 1000.0, lane));
            }
        }

        yield return null;
    }

    public void PlayerKeyDown(KeyCode key)
    {
        int timing = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern.songOffset);
        switch (key)
        {
            case KeyCode.S:
                StartCoroutine(NoteProcessKeyDown((int)LaneNumber.First - 1, timing));
                break;
            case KeyCode.D:
                StartCoroutine(NoteProcessKeyDown((int)LaneNumber.Second - 1, timing));
                break;
            case KeyCode.L:
                StartCoroutine(NoteProcessKeyDown((int)LaneNumber.Third - 1, timing));
                break;
            case KeyCode.Semicolon:
                StartCoroutine(NoteProcessKeyDown((int)LaneNumber.Fourth - 1, timing));
                break;
        }
    }

    IEnumerator NoteProcessKeyDown(int lane, double timing)
    {
        lanePressSounds[lane].Play();
        holdingCheck[lane] = true;
        lanePressEffects[lane].enabled = true;
        if (laneNoteDatas[lane].Count == 0)
            yield break;
        timing = (laneNoteDatas[lane].Peek().data.startTiming + timing) / Managers.Sound.managerAudioSource.clip.frequency;
        if (timing <= 0.2)
        {
            if (!laneNoteDatas[lane].Peek().data.isLongNote)
            {
                laneNoteDatas[lane].Dequeue();
                Destroy(laneNotes[lane].Dequeue());
                StartCoroutine(JudgingInput(timing + userOffset / 1000.0, lane));
            }
            else
            {
                holdingSampleAmout[lane] = laneNoteDatas[lane].Peek().data.startTiming;
                holdingSampleAmout[lane] += _barSampleAmount * 0.125f;
                StartCoroutine(JudgingLongNoteInput(timing + userOffset / 1000.0, lane));
            }
        }
        yield return null;
    }

    public void PlayerKeyPress(KeyCode key)
    {
        int timing = Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern.songOffset;
        switch (key)
        {
            case KeyCode.S:
                StartCoroutine(LongnoteProcess((int)LaneNumber.First - 1, timing));
                break;
            case KeyCode.D:
                StartCoroutine(LongnoteProcess((int)LaneNumber.Second - 1, timing));
                break;
            case KeyCode.L:
                StartCoroutine(LongnoteProcess((int)LaneNumber.Third - 1, timing));
                break;
            case KeyCode.Semicolon:
                StartCoroutine(LongnoteProcess((int)LaneNumber.Fourth - 1, timing));
                break;
        }
    }

    IEnumerator LongnoteProcess(int lane, int timing)
    {
        if (holdingSampleAmout[lane] <= timing && holdingSampleAmout[lane] > 0 &&  holdingSampleAmout[lane] - laneNoteDatas[lane].Peek().data.endTiming < 1)
        {
            Managers.Game.currentCombo += 1;
            holdingSampleAmout[lane] += _barSampleAmount * 0.125f;
            JudgeAction.Invoke(longnoteStartJudge[lane], 0);
        }
        yield return null;
    }

    public void PlayerKeyUp(KeyCode key)
    {
        int timing = Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern.songOffset;
        switch (key)
        {
            case KeyCode.S:
                StartCoroutine(CheckLongNoteKeyUp((int)LaneNumber.First - 1, timing));
                break;
            case KeyCode.D:
                StartCoroutine(CheckLongNoteKeyUp((int)LaneNumber.Second - 1, timing));
                break;
            case KeyCode.L:
                StartCoroutine(CheckLongNoteKeyUp((int)LaneNumber.Third - 1, timing));
                break;
            case KeyCode.Semicolon:
                StartCoroutine(CheckLongNoteKeyUp((int)LaneNumber.Fourth - 1, timing));
                break;
        }
    }

    IEnumerator CheckLongNoteKeyUp(int lane, int timing)
    {
        holdingCheck[lane] = false;
        lanePressEffects[lane].enabled = false;
        if (laneNoteDatas[lane].Count == 0)
            yield break;
        if (holdingSampleAmout[lane] > 0)
        {
            double tempDiff = (double)(laneNoteDatas[lane].Peek().data.endTiming - timing) / Managers.Sound.managerAudioSource.clip.frequency;
            holdingSampleAmout[lane] = 0;
            laneNoteDatas[lane].Dequeue();
            Destroy(laneNotes[lane].Dequeue());
            StartCoroutine(JudgingLongNoteInput(tempDiff, lane));
        }
        yield return null;
    }

    IEnumerator CalcScore()
    {
        passedNote += 1;
        Managers.Game.acurracy = (double)gainedAcc / perfectAcc;
        Managers.Game.progressByCombo = (double)passedNote / Managers.Game.currentLoadedPattern.totalCombo;
        Managers.Game.gainedScore = (int)(1000000 * Managers.Game.progressByCombo * Managers.Game.acurracy);
        Debug.Log($"{Managers.Game.gainedScore} {Managers.Game.acurracy} {Managers.Game.progressByCombo} {passedNote}");
        ScoreUpdateAction.Invoke();
        yield return null;
    }

    IEnumerator JudgingInput(double diff, int lane)
    {
        if (laneNoteDatas[lane].Count == 0)
            Managers.Game.DataQueueEmptyAction.Invoke();
        perfectAcc += 3;
        if (diff <= 0.04167 && diff >= -0.04167)
        {
            gainedAcc += 3;
            Managers.Game.currentCombo += 1;
            JudgeAction.Invoke(Judge.Besta, diff);
            Managers.Game.judgeCount[(int)Judge.Besta]++;
        }
        else if (diff <= 0.1 && diff >= -0.1)
        {
            gainedAcc += 2;
            Managers.Game.currentCombo += 1;
            JudgeAction.Invoke(Judge.Good, diff);
            Managers.Game.judgeCount[(int)Judge.Good]++;
        }
        else if (diff <= 0.16667 && diff >= -0.16667)
        {
            gainedAcc += 1;
            Managers.Game.currentCombo += 1;
            JudgeAction.Invoke(Judge.Bad, diff);
            Managers.Game.judgeCount[(int)Judge.Bad]++;
        }
        else
        {
            Managers.Game.currentCombo = 0;
            Managers.Game.isFullCombo = false;
            JudgeAction.Invoke(Judge.Miss, diff);
            Managers.Game.judgeCount[(int)Judge.Miss]++;
        }
        StartCoroutine(CalcScore());
        yield return null;
    }
    IEnumerator JudgingLongNoteInput(double diff, int lane)
    {
        if (laneNoteDatas[lane].Count == 0)
            Managers.Game.DataQueueEmptyAction.Invoke();
        perfectAcc += 3;
        if (diff <= 0.1 && diff >= -0.1)
        {
            gainedAcc += 3;
            Managers.Game.currentCombo += 1;
            JudgeAction.Invoke(Judge.Besta, diff);
            Managers.Game.judgeCount[(int)Judge.Besta]++;
            longnoteStartJudge[lane] = Judge.Besta;
        }
        else if (diff <= 0.16667 && diff >= -0.16667)
        {
            gainedAcc += 1;
            Managers.Game.currentCombo += 1;
            JudgeAction.Invoke(Judge.Bad, diff);
            Managers.Game.judgeCount[(int)Judge.Bad]++;
            longnoteStartJudge[lane] = Judge.Bad;
        }
        else
        {
            gainedAcc += 0;
            Managers.Game.currentCombo = 0;
            JudgeAction.Invoke(Judge.Miss, diff);
            Managers.Game.judgeCount[(int)Judge.Miss]++;
            longnoteStartJudge[lane] = Judge.Miss;
        }
        StartCoroutine(CalcScore());
        yield return null;
    }

    void ScrollPattern()
    {
        patternObject.transform.Translate(new Vector3(0, -(Managers.Sound.managerAudioSource.clip.frequency / _barSampleAmount * 4.8f), 0) * scrollSpeed * Time.deltaTime);
        if (isPlaying && !Managers.Sound.managerAudioSource.isPlaying)
        {
            if (-patternObject.transform.localPosition.y + patternObjectPosOffset / (4.8f * scrollSpeed) >= 2 && (_barSampleAmount * ((-patternObject.transform.localPosition.y + patternObjectPosOffset - (4.8f * scrollSpeed * 2)) / (4.8f * scrollSpeed)) >= Managers.Game.currentLoadedPattern.songOffset))
            {
                Managers.Sound.managerAudioSource.timeSamples = (int)(_barSampleAmount * ((-patternObject.transform.localPosition.y + patternObjectPosOffset - (4.8f * scrollSpeed * 2)) / (4.8f * scrollSpeed)) - Managers.Game.currentLoadedPattern.songOffset);
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
            float yPos = (n.startTiming / _barSampleAmount * 4.8f * scrollSpeed) + 4.8f * scrollSpeed * 2;
            currentNote.transform.localPosition = new Vector2(0, yPos + 0.125f);
            if (n.isLongNote)
            {
                float legacyPos = yPos;
                yPos = (n.endTiming / _barSampleAmount * 4.8f * scrollSpeed) + 4.8f * scrollSpeed * 2;
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
