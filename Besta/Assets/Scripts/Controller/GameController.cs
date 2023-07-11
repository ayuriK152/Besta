using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Datas;
using static Define;

public class GameController : MonoBehaviour
{
    GameObject gameNotePref;
    Transform[] laneTransforms;

    public Queue<GameObject> firstLaneNotes;
    public Queue<GameObject> secondLaneNotes;
    public Queue<GameObject> thirdLaneNotes;
    public Queue<GameObject> fourthLaneNotes;
    void Start()
    {
        gameNotePref = Resources.Load("Prefabs/GameNote") as GameObject;
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
        
    }

    public void PlayerKeyDown(KeyCode key)
    {
        Debug.Log(key.ToString());
    }

    public void LoadPattern()
    {
        float barSampleAmount = (Managers.Game.currentLoadedPattern._musicSource.frequency / (Managers.Game.currentLoadedPattern._bpm / (float)60)) * 4;
        foreach (Note n in Managers.Game.currentLoadedPattern._noteDatas)
        {
            GameObject currentNote = null;
            switch (n._laneNumber)  // 레인별로 입력을 처리하기 위해 따로 큐에 저장한 후 선입선출 방식으로 처리
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
            float yPos = n._startTiming / barSampleAmount * 4.8f * 3;
            currentNote.transform.localPosition = new Vector2(0, yPos + 0.125f);
            if (n._isLongNote)
            {
                float legacyPos = yPos;
                yPos = n._endTiming / barSampleAmount * 4.8f * 3;
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
