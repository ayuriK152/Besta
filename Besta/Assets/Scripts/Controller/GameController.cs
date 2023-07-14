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

    float _barSampleAmount;

    public static bool isPlaying;

    public Queue<GameObject> firstLaneNotes = new Queue<GameObject>();
    public Queue<GameObject> secondLaneNotes = new Queue<GameObject>();
    public Queue<GameObject> thirdLaneNotes = new Queue<GameObject>();
    public Queue<GameObject> fourthLaneNotes = new Queue<GameObject>();

    public Queue<GameNote> firstLaneNoteDatas = new Queue<GameNote>();
    public Queue<GameNote> secondLaneNoteDatas = new Queue<GameNote>();
    public Queue<GameNote> thirdLaneNoteDatas = new Queue<GameNote>();
    public Queue<GameNote> fourthLaneNoteDatas = new Queue<GameNote>();
    void Start()
    {
        gameNotePref = Resources.Load("Prefabs/GameNote") as GameObject;
        patternObject = GameObject.Find("Pattern");
        laneTransforms = new Transform[4];
        laneTransforms[0] = GameObject.Find("First").transform;
        laneTransforms[1] = GameObject.Find("Second").transform;
        laneTransforms[2] = GameObject.Find("Third").transform;
        laneTransforms[3] = GameObject.Find("Fourth").transform;

        Managers.Input.KeyAction -= PlayerKeyDown;
        Managers.Input.KeyAction += PlayerKeyDown;
        LoadPattern();
    }

    void Update()
    {
        if (isPlaying)
        {
            ScrollPattern();
        }
    }

    double diff;
    public void PlayerKeyDown(KeyCode key)
    {
        diff = -(Managers.Sound.managerAudioSource.timeSamples + Managers.Game.currentLoadedPattern._songOffset);
        switch (key)
        {
            case KeyCode.S:
                diff += firstLaneNoteDatas.Peek().data._startTiming;
                if (true)
                {
                    Debug.Log($"{diff / 44100}");
                    firstLaneNoteDatas.Dequeue();
                    if (diff / 44100 <= 0.04167 && diff / 44100 >= -0.04167)
                    {
                        Debug.Log("Besta");
                    }
                    else if (diff / 44100 <= 0.1 && diff / 44100 >= -0.1)
                    {
                        Debug.Log("Good");
                    }
                    else if (diff / 44100 <= 0.16667 && diff / 44100 >= -0.16667)
                    {
                        Debug.Log("Bad");
                    }
                    else
                    {
                        Debug.LogWarning("Miss");
                    }
                }
                break;
            case KeyCode.D:
                diff += secondLaneNoteDatas.Peek().data._startTiming;
                if (true)
                {
                    Debug.Log($"{diff / 44100}");
                    secondLaneNoteDatas.Dequeue();
                    if (diff / 44100 <= 0.04167 && diff / 44100 >= -0.04167)
                    {
                        Debug.Log("Besta");
                    }
                    else if (diff / 44100 <= 0.1 && diff / 44100 >= -0.1)
                    {
                        Debug.Log("Good");
                    }
                    else if (diff / 44100 <= 0.16667 && diff / 44100 >= -0.16667)
                    {
                        Debug.Log("Bad");
                    }
                    else
                    {
                        Debug.LogWarning("Miss");
                    }
                }
                break;
            case KeyCode.L:
                diff += thirdLaneNoteDatas.Peek().data._startTiming;
                if (true)
                {
                    Debug.Log($"{diff / 44100}");
                    thirdLaneNoteDatas.Dequeue();
                    if (diff / 44100 <= 0.04167 && diff / 44100 >= -0.04167)
                    {
                        Debug.Log("Besta");
                    }
                    else if (diff / 44100 <= 0.1 && diff / 44100 >= -0.1)
                    {
                        Debug.Log("Good");
                    }
                    else if (diff / 44100 <= 0.16667 && diff / 44100 >= -0.16667)
                    {
                        Debug.Log("Bad");
                    }
                    else
                    {
                        Debug.LogWarning("Miss");
                    }
                }
                break;
            case KeyCode.Semicolon:
                diff += fourthLaneNoteDatas.Peek().data._startTiming;
                if (true)
                {
                    Debug.Log($"{diff / 44100}");
                    fourthLaneNoteDatas.Dequeue();
                    if (diff / 44100 <= 0.04167 && diff / 44100 >= -0.04167)
                    {
                        Debug.Log("Besta");
                    }
                    else if (diff / 44100 <= 0.1 && diff / 44100 >= -0.1)
                    {
                        Debug.Log("Good");
                    }
                    else if (diff / 44100 <= 0.16667 && diff / 44100 >= -0.16667)
                    {
                        Debug.Log("Bad");
                    }
                    else
                    {
                        Debug.LogWarning("Miss");
                    }
                }
                break;
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
