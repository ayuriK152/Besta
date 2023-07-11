using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Datas;
using static Define;

public class GameController : MonoBehaviour
{
    public Queue<GameObject> firstLaneNotes;
    public Queue<GameObject> secondLaneNotes;
    public Queue<GameObject> thirdLaneNotes;
    public Queue<GameObject> fourthLaneNotes;
    void Start()
    {
        Managers.Input.KeyAction -= PlayerKeyDown;
        Managers.Input.KeyAction += PlayerKeyDown;
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
        firstLaneNotes = new Queue<GameObject>();
        secondLaneNotes = new Queue<GameObject>();
        thirdLaneNotes = new Queue<GameObject>();
        fourthLaneNotes = new Queue<GameObject>();

        foreach (Note n in Managers.Game.currentLoadedPattern._noteDatas)
        {
            switch (n._laneNumber)  // ���κ��� �Է��� ó���ϱ� ���� ���� ť�� ������ �� ���Լ��� ������� ó��
            {
                case LaneNumber.First:
                    break;
                case LaneNumber.Second:
                    break;
                case LaneNumber.Third:
                    break;
                case LaneNumber.Fourth:
                    break;
            }
        }
    }
}
