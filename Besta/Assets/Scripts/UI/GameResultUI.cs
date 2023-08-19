using System;
using TMPro;
using UnityEngine;
using static Define;

public class GameResultUI : MonoBehaviour
{
    TextMeshProUGUI scoreText;
    TextMeshProUGUI comboText;
    TextMeshProUGUI accuracyText;
    TextMeshProUGUI bestaText;
    TextMeshProUGUI goodText;
    TextMeshProUGUI badText;
    TextMeshProUGUI missText;

    void Start()
    {
        scoreText = GameObject.Find("Score").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        comboText = GameObject.Find("Combo").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        accuracyText = GameObject.Find("Accuracy").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        bestaText = GameObject.Find("BestaCount").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        goodText = GameObject.Find("GoodCount").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        badText = GameObject.Find("BadCount").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        missText = GameObject.Find("MissCount").transform.Find("Value").GetComponent<TextMeshProUGUI>();

        scoreText.text = Managers.Game.gainedScore.ToString();
        comboText.text = Managers.Game.currentCombo.ToString();
        accuracyText.text = $"{(Math.Truncate(Managers.Game.acurracy * 10000) * 0.01)}%";
        bestaText.text = Managers.Game.judgeCount[(int)Judge.Besta].ToString();
        goodText.text = Managers.Game.judgeCount[(int)Judge.Good].ToString();
        badText.text = Managers.Game.judgeCount[(int)Judge.Bad].ToString();
        missText.text = Managers.Game.judgeCount[(int)Judge.Miss].ToString();
    }

    void Update()
    {
        
    }
}
