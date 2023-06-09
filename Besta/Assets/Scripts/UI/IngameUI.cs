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
    Coroutine judgeTextCoroutine;
    Coroutine timingDiffTextCoroutine;

    void Start()
    {
        judgeText = GameObject.Find("JudgeText").GetComponent<TextMeshProUGUI>();
        timingDiffText = GameObject.Find("TimingDiffText").GetComponent<TextMeshProUGUI>();
        comboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
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
                if (judgeTextCoroutine != null)
                    StopCoroutine(judgeTextCoroutine);
                judgeText.color = new Color(1, 0, 0, 1);
                judgeText.text = "Besta";
                judgeTextCoroutine = StartCoroutine(FadeJudgeText());
                break;
            case Judge.Good:
                if (judgeTextCoroutine != null)
                    StopCoroutine(judgeTextCoroutine);
                if (timingDiffTextCoroutine != null)
                    StopCoroutine(timingDiffTextCoroutine);
                ChangeTimingDiffTextColor(timingDiff);
                judgeText.color = new Color(0, 1, 0, 1);
                judgeText.text = "Good";
                timingDiffText.color = new Color(timingDiffText.color.r, timingDiffText.color.g, timingDiffText.color.b, 1);
                judgeTextCoroutine = StartCoroutine(FadeJudgeText());
                timingDiffTextCoroutine = StartCoroutine(FadeTimingText());
                break;
            case Judge.Bad:
                if (judgeTextCoroutine != null)
                    StopCoroutine(judgeTextCoroutine);
                if (timingDiffTextCoroutine != null)
                    StopCoroutine(timingDiffTextCoroutine);
                ChangeTimingDiffTextColor(timingDiff);
                judgeText.color = new Color(0, 0, 1, 1);
                judgeText.text = "Bad";
                timingDiffText.color = new Color(timingDiffText.color.r, timingDiffText.color.g, timingDiffText.color.b, 1);
                judgeTextCoroutine = StartCoroutine(FadeJudgeText());
                timingDiffTextCoroutine = StartCoroutine(FadeTimingText());
                break;
            case Judge.Miss:
                if (judgeTextCoroutine != null)
                    StopCoroutine(judgeTextCoroutine);
                if (timingDiffTextCoroutine != null)
                    StopCoroutine(timingDiffTextCoroutine);
                ChangeTimingDiffTextColor(timingDiff);
                judgeText.color = new Color(0.8f, 0.8f, 0.8f, 1);
                judgeText.text = "Miss";
                timingDiffText.color = new Color(timingDiffText.color.r, timingDiffText.color.g, timingDiffText.color.b, 1);
                judgeTextCoroutine = StartCoroutine(FadeJudgeText());
                timingDiffTextCoroutine = StartCoroutine(FadeTimingText());
                break;
        }
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
