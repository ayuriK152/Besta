using System;
using static Datas;

public class IngameManager
{
    public MusicPattern currentLoadedPattern;

    public int currentCombo;
    public int maxCombo;
    public int gainedScore;
    public int[] judgeCount = new int[4];
    int _queueEmptyCount;

    public double acurracy;
    public double progressByCombo;

    public bool isFullCombo;

    public Action DataQueueEmptyAction = null;
    public Action<bool> CheckGameEndAction = null;

    public void Init()
    {
        currentCombo = 0;
        maxCombo = 0;
        gainedScore = 0;
        for (int i = 0; i < judgeCount.Length; i++)
            judgeCount[i] = 0;
        _queueEmptyCount = 0;

        isFullCombo = true;

        DataQueueEmptyAction -= CheckGameEnds;
        DataQueueEmptyAction += CheckGameEnds;
    }

    void CheckGameEnds()
    {
        _queueEmptyCount++;
        if (_queueEmptyCount == 4)
        {
            CheckGameEndAction.Invoke(isFullCombo);
        }
    }
}
