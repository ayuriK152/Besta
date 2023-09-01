using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class IngameUI : MonoBehaviour
{
    TextMeshProUGUI judgeText;
    TextMeshProUGUI timingDiffText;
    TextMeshProUGUI comboText;
    TextMeshProUGUI scoreText;
    TextMeshProUGUI maxcomboText;
    TextMeshProUGUI musicNameText;
    TextMeshProUGUI artistNameText;
    TextMeshProUGUI etcText;
    Image jacketImage;
    Image fadePanelImage;
    Coroutine judgeTextCoroutine;
    Coroutine timingDiffTextCoroutine;

    Animator comboTextAnimator;
    Animator judgeTextAnimator;
    public Animator introMusicInfoAnimator;

    void Start()
    {
        judgeText = GameObject.Find("JudgeText").GetComponent<TextMeshProUGUI>();
        timingDiffText = GameObject.Find("TimingDiffText").GetComponent<TextMeshProUGUI>();
        comboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        scoreText.text = "0";
        maxcomboText = GameObject.Find("MaxComboText").GetComponent<TextMeshProUGUI>();
        jacketImage = GameObject.Find("IngameMusicIntro").transform.Find("Image").GetComponent<Image>();
        jacketImage.sprite = Resources.Load<Sprite>($"Patterns/{Managers.Game.currentLoadedPattern.name}/image");
        musicNameText = GameObject.Find("IngameMusicIntro").transform.Find("Panel").Find("MusicName").GetComponent<TextMeshProUGUI>();
        musicNameText.text = Managers.Game.currentLoadedPattern.name;
        artistNameText = GameObject.Find("IngameMusicIntro").transform.Find("Panel").Find("ArtistName").GetComponent<TextMeshProUGUI>();
        artistNameText.text = Managers.Game.currentLoadedPattern.artist;
        etcText = GameObject.Find("IngameMusicIntro").transform.Find("Panel").Find("Etc").GetComponent<TextMeshProUGUI>();
        etcText.text = $"BPM - {Managers.Game.currentLoadedPattern.bpm} | Design - ayuriK";

        comboTextAnimator = GameObject.Find("ComboText").GetComponent<Animator>();
        judgeTextAnimator = GameObject.Find("JudgeText").GetComponent<Animator>();
        introMusicInfoAnimator = GameObject.Find("IngameMusicIntro").GetComponent<Animator>();
        fadePanelImage = GameObject.Find("FadePanel").GetComponent<Image>();

        GameController.JudgeAction = null;
        GameController.JudgeAction -= OnJudgeTriggered;
        GameController.JudgeAction += OnJudgeTriggered;
        GameController.ScoreUpdateAction = null;
        GameController.ScoreUpdateAction -= OnScoreUpdate;
        GameController.ScoreUpdateAction += OnScoreUpdate;
        StartCoroutine(IngameIntro());
    }

    IEnumerator IngameIntro()
    {
        introMusicInfoAnimator.Play("IngameMusicIntroAnimation");
        while (introMusicInfoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) { yield return null; }
        GameController.isPlaying = true;
        yield return null;
    }

    public void OnJudgeTriggered(Judge judge, double timingDiff)
    {
        if (Managers.Game.currentCombo == 0)
            comboText.text = "";
        else
        {
            comboText.text = $"{Managers.Game.currentCombo}";
            comboTextAnimator.Play("ComboUpdate", -1, 0);
        }
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
        if (judge != Judge.Besta && judge != Judge.Miss && timingDiff != 0)
        {
            if (timingDiffTextCoroutine != null)
                StopCoroutine(timingDiffTextCoroutine);
            ChangeTimingDiffTextColor(timingDiff);
        }
        judgeText.color = textColor;
        judgeText.text = text;
        UpdateMaxComboText();
        if (judge != Judge.Besta && judge != Judge.Miss && timingDiff != 0)
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

    void OnScoreUpdate()
    {
        scoreText.text = Managers.Game.gainedScore.ToString();
    }

    IEnumerator FadeJudgeText()
    {
        judgeTextAnimator.Play("JudgeUpdateAnimation", -1, 0);
        yield return null;
    }

    IEnumerator FadeTimingText()
    {
        yield return new WaitForSeconds(1);
        timingDiffText.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeOutGameScene()
    {
        yield return new WaitForSeconds(1);
        float currentTime = 0;
        while (currentTime < 1)
        {
            currentTime += Time.deltaTime;
            fadePanelImage.color = new Color(0, 0, 0, (Mathf.Lerp(0, 1, currentTime / 1)));
            yield return null;
        }
        Managers.Scene.LoadScene(Scene.GameResult);
    }
}
