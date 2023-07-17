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
        laneTransforms[0] = GameObject.Find("First").transform;
        laneTransforms[1] = GameObject.Find("Second").transform;
        laneTransforms[2] = GameObject.Find("Third").transform;
        laneTransforms[3] = GameObject.Find("Fourth").transform;
        lanePressEffects[0] = GameObject.Find("FirstLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[1] = GameObject.Find("SecondLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[2] = GameObject.Find("ThirdLanePressEffect").GetComponent<SpriteRenderer>();
        lanePressEffects[3] = GameObject.Find("FourthLanePressEffect").GetComponent<SpriteRenderer>();
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
        if (firstLaneNoteDatas.Count > 0)
        {
            diffUpdatesAlways = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset);
            if (!firstLaneNoteDatas.Peek().data._isLongNote && (firstLaneNoteDatas.Peek().data._startTiming + diffUpdatesAlways) / 44100 < -0.16667)
            {
                Destroy(firstLaneNotes.Dequeue());
                JudgingInput((firstLaneNoteDatas.Dequeue().data._startTiming + diffUpdatesAlways) / 44100);
            }
            else if (firstLaneNoteDatas.Peek().data._isLongNote && (firstLaneNoteDatas.Peek().data._endTiming + diffUpdatesAlways) / 44100 < -0.3)
            {
                Destroy(firstLaneNotes.Dequeue());
                JudgingInput((firstLaneNoteDatas.Dequeue().data._endTiming + diffUpdatesAlways) / 44100);
            }
        }
        if (secondLaneNoteDatas.Count > 0)
        {
            diffUpdatesAlways = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset);
            if (!secondLaneNoteDatas.Peek().data._isLongNote && (secondLaneNoteDatas.Peek().data._startTiming + diffUpdatesAlways) / 44100 < -0.16667)
            {
                Destroy(secondLaneNotes.Dequeue());
                JudgingInput((secondLaneNoteDatas.Dequeue().data._startTiming + diffUpdatesAlways) / 44100);
            }
            else if (secondLaneNoteDatas.Peek().data._isLongNote && (secondLaneNoteDatas.Peek().data._endTiming + diffUpdatesAlways) / 44100 < -0.3)
            {
                Destroy(secondLaneNotes.Dequeue());
                JudgingInput((secondLaneNoteDatas.Dequeue().data._endTiming + diffUpdatesAlways) / 44100);
            }
        }
        if (thirdLaneNoteDatas.Count > 0)
        {
            diffUpdatesAlways = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset);
            if (!thirdLaneNoteDatas.Peek().data._isLongNote && (thirdLaneNoteDatas.Peek().data._startTiming + diffUpdatesAlways) / 44100 < -0.16667)
            {
                Destroy(thirdLaneNotes.Dequeue());
                JudgingInput((thirdLaneNoteDatas.Dequeue().data._startTiming + diffUpdatesAlways) / 44100);
            }
            else if (thirdLaneNoteDatas.Peek().data._isLongNote && (thirdLaneNoteDatas.Peek().data._endTiming + diffUpdatesAlways) / 44100 < -0.3)
            {
                Destroy(thirdLaneNotes.Dequeue());
                JudgingInput((thirdLaneNoteDatas.Dequeue().data._endTiming + diffUpdatesAlways) / 44100);
            }
        }
        if (fourthLaneNoteDatas.Count > 0)
        {
            diffUpdatesAlways = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset);
            if (!fourthLaneNoteDatas.Peek().data._isLongNote && (fourthLaneNoteDatas.Peek().data._startTiming + diffUpdatesAlways) / 44100 < -0.16667)
            {
                Destroy(fourthLaneNotes.Dequeue());
                JudgingInput((fourthLaneNoteDatas.Dequeue().data._startTiming + diffUpdatesAlways) / 44100);
            }
            else if (fourthLaneNoteDatas.Peek().data._isLongNote && (fourthLaneNoteDatas.Peek().data._endTiming + diffUpdatesAlways) / 44100 < -0.3)
            {
                Destroy(fourthLaneNotes.Dequeue());
                JudgingInput((fourthLaneNoteDatas.Dequeue().data._endTiming + diffUpdatesAlways) / 44100);
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
                if (!firstLaneNoteDatas.Peek().data._isLongNote)
                {
                    firstLaneNoteDatas.Dequeue();
                    Destroy(firstLaneNotes.Dequeue());
                }
                else
                {
                    holdingSampleAmout[0] = fourthLaneNoteDatas.Peek().data._startTiming;
                    holdingSampleAmout[0] += _barSampleAmount * 0.25f;
                }
                JudgingInput(diff);
                break;
            case KeyCode.D:
                lanePressEffects[1].enabled = true;
                diff = (secondLaneNoteDatas.Peek().data._startTiming + diff) / 44100;
                if (diff > 0.2)
                    break;
                if (!secondLaneNoteDatas.Peek().data._isLongNote)
                {
                    secondLaneNoteDatas.Dequeue();
                    Destroy(secondLaneNotes.Dequeue());
                }
                else
                {
                    holdingSampleAmout[1] = fourthLaneNoteDatas.Peek().data._startTiming;
                    holdingSampleAmout[1] += _barSampleAmount * 0.25f;
                }
                JudgingInput(diff);
                break;
            case KeyCode.L:
                lanePressEffects[2].enabled = true;
                diff = (thirdLaneNoteDatas.Peek().data._startTiming + diff) / 44100;
                if (diff > 0.2)
                    break;
                if (!thirdLaneNoteDatas.Peek().data._isLongNote)
                {
                    thirdLaneNoteDatas.Dequeue();
                    Destroy(thirdLaneNotes.Dequeue());
                }
                else
                {
                    holdingSampleAmout[2] = fourthLaneNoteDatas.Peek().data._startTiming;
                    holdingSampleAmout[2] += _barSampleAmount * 0.25f;
                }
                JudgingInput(diff);
                break;
            case KeyCode.Semicolon:
                lanePressEffects[3].enabled = true;
                diff = (fourthLaneNoteDatas.Peek().data._startTiming + diff) / 44100;
                if (diff > 0.2)
                    break;
                if (!fourthLaneNoteDatas.Peek().data._isLongNote)
                {
                    fourthLaneNoteDatas.Dequeue();
                    Destroy(fourthLaneNotes.Dequeue());
                }
                else
                {
                    holdingSampleAmout[3] = fourthLaneNoteDatas.Peek().data._startTiming;
                    holdingSampleAmout[3] += _barSampleAmount * 0.25f;
                }
                JudgingInput(diff);
                break;
        }
    }

    public void PlayerKeyPress(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.S:
                if (holdingSampleAmout[0] > 0)
                {
                    if ((holdingSampleAmout[0] - (int)holdingSampleAmout[0] > 0.5f ? (int)holdingSampleAmout[0] + 1 : holdingSampleAmout[0]) <= Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)
                    {
                        holdingSampleAmout[0] += _barSampleAmount * 0.25f;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                    }
                    else if (firstLaneNoteDatas.Peek().data._endTiming <= Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)
                    {
                        holdingSampleAmout[0] = 0;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                        firstLaneNoteDatas.Dequeue();
                        Destroy(firstLaneNotes.Dequeue());
                    }
                }
                break;
            case KeyCode.D:
                if (holdingSampleAmout[1] > 0)
                {
                    if ((holdingSampleAmout[1] - (int)holdingSampleAmout[1] > 0.5f ? (int)holdingSampleAmout[1] + 1 : holdingSampleAmout[1]) <= Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)
                    {
                        holdingSampleAmout[1] += _barSampleAmount * 0.25f;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                    }
                    else if (secondLaneNoteDatas.Peek().data._endTiming <= Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)
                    {
                        holdingSampleAmout[1] = 0;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                        secondLaneNoteDatas.Dequeue();
                        Destroy(secondLaneNotes.Dequeue());
                    }
                }
                break;
            case KeyCode.L:
                if (holdingSampleAmout[2] > 0)
                {
                    if ((holdingSampleAmout[2] - (int)holdingSampleAmout[2] > 0.5f ? (int)holdingSampleAmout[2] + 1 : holdingSampleAmout[2]) <= Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)
                    {
                        holdingSampleAmout[2] += _barSampleAmount * 0.25f;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                    }
                    else if (thirdLaneNoteDatas.Peek().data._endTiming <= Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)
                    {
                        holdingSampleAmout[2] = 0;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                        thirdLaneNoteDatas.Dequeue();
                        Destroy(thirdLaneNotes.Dequeue());
                    }
                }
                break;
            case KeyCode.Semicolon:
                if (holdingSampleAmout[3] > 0)
                {
                    if ((holdingSampleAmout[3] - (int)holdingSampleAmout[3] > 0.5f ? (int)holdingSampleAmout[3] + 1 : holdingSampleAmout[3]) <= Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)
                    {
                        holdingSampleAmout[3] += _barSampleAmount * 0.25f;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                    }
                    else if (fourthLaneNoteDatas.Peek().data._endTiming <= Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)
                    {
                        holdingSampleAmout[3] = 0;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                        fourthLaneNoteDatas.Dequeue();
                        Destroy(fourthLaneNotes.Dequeue());
                    }
                }
                break;
        }
    }

    public void PlayerKeyUp(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.S:
                lanePressEffects[0].enabled = false;
                if (holdingSampleAmout[0] > 0)
                {
                    double tempDiff = (double)(firstLaneNoteDatas.Peek().data._endTiming - (Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)) / 44100;
                    if (tempDiff <= 0.2 && tempDiff >= -0.2)
                    {
                        holdingSampleAmout[0] = 0;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                        firstLaneNoteDatas.Dequeue();
                        Destroy(firstLaneNotes.Dequeue());
                    }
                }
                break;
            case KeyCode.D:
                lanePressEffects[1].enabled = false;
                if (holdingSampleAmout[1] > 0)
                {
                    double tempDiff = (double)(secondLaneNoteDatas.Peek().data._endTiming - (Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)) / 44100;
                    if (tempDiff <= 0.2 && tempDiff >= -0.2)
                    {
                        holdingSampleAmout[1] = 0;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                        secondLaneNoteDatas.Dequeue();
                        Destroy(secondLaneNotes.Dequeue());
                    }
                }
                break;
            case KeyCode.L:
                lanePressEffects[2].enabled = false;
                if (holdingSampleAmout[2] > 0)
                {
                    double tempDiff = (double)(thirdLaneNoteDatas.Peek().data._endTiming - (Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)) / 44100;
                    if (tempDiff <= 0.2 && tempDiff >= -0.2)
                    {
                        holdingSampleAmout[2] = 0;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                        thirdLaneNoteDatas.Dequeue();
                        Destroy(thirdLaneNotes.Dequeue());
                    }
                }
                break;
            case KeyCode.Semicolon:
                lanePressEffects[3].enabled = false;
                if (holdingSampleAmout[3] > 0)
                {
                    double tempDiff = (double)(fourthLaneNoteDatas.Peek().data._endTiming - (Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset)) / 44100;
                    if (tempDiff <= 0.2 && tempDiff >= -0.2)
                    {
                        holdingSampleAmout[3] = 0;
                        Managers.Game.currentCombo++;
                        judgeAction.Invoke(Judge.None, diff);
                        fourthLaneNoteDatas.Dequeue();
                        Destroy(fourthLaneNotes.Dequeue());
                    }
                }
                break;
        }
    }

    void JudgingInput(double diff)
    {
        if (diff <= 0.04167 && diff >= -0.04167)
        {
            Debug.Log("Besta");
            Managers.Game.currentCombo++;
            judgeAction.Invoke(Judge.Besta, diff);
        }
        else if (diff <= 0.1 && diff >= -0.1)
        {
            Debug.Log("Good");
            Managers.Game.currentCombo++;
            judgeAction.Invoke(Judge.Good, diff);
        }
        else if (diff <= 0.16667 && diff >= -0.16667)
        {
            Debug.Log("Bad");
            Managers.Game.currentCombo++;
            judgeAction.Invoke(Judge.Bad, diff);
        }
        else
        {
            Debug.LogWarning("Miss");
            Managers.Game.currentCombo = 0;
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
            switch (n._laneNumber)  // ���κ��� �Է��� ó���ϱ� ���� ���� ť�� ������ �� ���Լ��� ������� ó��
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
            // yPos ���� ����ϴ� ������ ������ �μ��� ������ ���� ���� ��ũ�� �ӵ��� �Ǿ�� �Ѵ�. �ӽ� �������� ���� ���� ���� 3���� ������ �����Ƿ� ���� ���� �ʿ�.
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
