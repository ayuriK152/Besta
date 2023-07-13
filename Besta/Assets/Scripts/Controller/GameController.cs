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

    public Queue<GameObject> firstLaneNotes;
    public Queue<GameObject> secondLaneNotes;
    public Queue<GameObject> thirdLaneNotes;
    public Queue<GameObject> fourthLaneNotes;
    void Start()
    {
        gameNotePref = Resources.Load("Prefabs/GameNote") as GameObject;
        patternObject = GameObject.Find("Pattern");
        laneTransforms = new Transform[4];
        laneTransforms[0] = GameObject.Find("First").transform;
        laneTransforms[1] = GameObject.Find("Second").transform;
        laneTransforms[2] = GameObject.Find("Third").transform;
        laneTransforms[3] = GameObject.Find("Fourth").transform;

        firstLaneNotes = new Queue<GameObject>();
        secondLaneNotes = new Queue<GameObject>();
        thirdLaneNotes = new Queue<GameObject>();
        fourthLaneNotes = new Queue<GameObject>();

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

    public void PlayerKeyDown(KeyCode key)
    {
        Debug.Log(key.ToString());
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
                    firstLaneNotes.Enqueue(currentNote);
                    break;
                case LaneNumber.Second:
                    currentNote = Instantiate(gameNotePref, laneTransforms[1]);
                    secondLaneNotes.Enqueue(currentNote);
                    break;
                case LaneNumber.Third:
                    currentNote = Instantiate(gameNotePref, laneTransforms[2]);
                    thirdLaneNotes.Enqueue(currentNote);
                    break;
                case LaneNumber.Fourth:
                    currentNote = Instantiate(gameNotePref, laneTransforms[3]);
                    fourthLaneNotes.Enqueue(currentNote);
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
        }
    }
}
