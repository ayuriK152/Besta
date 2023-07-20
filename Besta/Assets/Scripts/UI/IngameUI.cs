using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class IngameUI : MonoBehaviour
{
    TextMeshProUGUI judgeText;
    TextMeshProUGUI timingDiffText;
    TextMeshProUGUI comboText;
    TextMeshProUGUI scoreText;
    TextMeshProUGUI maxcomboText;
    Coroutine judgeTextCoroutine;
    Coroutine timingDiffTextCoroutine;

    void Start()
    {
        judgeText = GameObject.Find("JudgeText").GetComponent<TextMeshProUGUI>();
        timingDiffText = GameObject.Find("TimingDiffText").GetComponent<TextMeshProUGUI>();
        comboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        maxcomboText = GameObject.Find("MaxComboText").GetComponent<TextMeshProUGUI>();
        GameController.judgeAction -= OnJudgeTriggered;
        GameController.judgeAction += OnJudgeTriggered;
    }

    public void OnStartButtonClick()
    {
        GameController.isPlaying = true;
    }

    public void OnJudgeTriggered(Judge judge, double timingDiff)
    {
        comboText.text = $"{Managers.Game.currentCombo}";
        switch (judge)
        {
            case Judge.Besta:
                UpdateTexts(Judge.Besta, timingDiff, "Besta", new Color(1, 0, 0, 1));
                break;
            case Judge.Good:
                UpdateTexts(Judge.Good, timingDiff, "Good", new Color(0, 1, 0, 1));
                break;
            case Judge.Bad:
                UpdateTexts(Judge.Bad, timingDiff, "Bad", new Color(0, 0, 1, 1));
                break;
            case Judge.Miss:
                UpdateTexts(Judge.Miss, timingDiff, "Miss", new Color(0.8f, 0.8f, 0.8f, 1));
                break;
            case Judge.None:
                UpdateMaxComboText();
                break;
        }
    }

    void UpdateTexts(Judge judge, double timingDiff, string text, Color textColor)
    {
        if (judgeTextCoroutine != null)
            StopCoroutine(judgeTextCoroutine);
        if (judge != Judge.Besta)
        {
            if (timingDiffTextCoroutine != null)
                StopCoroutine(timingDiffTextCoroutine);
            ChangeTimingDiffTextColor(timingDiff);
        }
        judgeText.color = textColor;
        judgeText.text = text;
        UpdateMaxComboText();
        if (judge != Judge.Besta)
        {
            timingDiffText.color = new Color(timingDiffText.color.r, timingDiffText.color.g, timingDiffText.color.b, 1);
            timingDiffTextCoroutine = StartCoroutine(FadeTimingText());
        }
        judgeTextCoroutine = StartCoroutine(FadeJudgeText());
    }

    void UpdateMaxComboText()
    {
        if (Managers.Game.currentCombo > Managers.Game.maxCombo)
            Managers.Game.maxCombo = Managers.Game.currentCombo;
        if (Managers.Game.maxCombo > Convert.ToInt32(maxcomboText.text))
            maxcomboText.text = Convert.ToString(Managers.Game.maxCombo);
    }

    void ChangeTimingDiffTextColor(double timingDiff)
    {
        if (timingDiff > 0)
        {
            timingDiffText.text = $"EARLY +{Math.Truncate(timingDiff * 10000) / 10} ms";
            timingDiffText.color = new Color(0.7f, 0.2f, 0.2f, 0);
        }
        else
        {
            timingDiffText.text = $"LATE {Math.Truncate(timingDiff * 10000) / 10} ms";
            timingDiffText.color = new Color(0.2f, 0.2f, 0.7f, 0);
        }
    }

    IEnumerator FadeJudgeText()
    {
        yield return new WaitForSeconds(0.4f);
        judgeText.color = new Color(0, 0, 0, 0);
    }

    IEnumerator FadeTimingText()
    {
        yield return new WaitForSeconds(1);
        timingDiffText.color = new Color(0, 0, 0, 0);
    }
}
