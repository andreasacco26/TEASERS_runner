using System;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class ScoreManager: MonoBehaviour {
    [Header("Score")]
    public TMP_Text scoreText;
    public float scoreMultiplier = 5.0f;

    [Header("Game Over")]
    public TMP_Text gameOverText;
    public CanvasGroup gameOverCanvas;
    public List<string> gameOverInsults;

    public bool IsRecording { get; private set; }

    public static ScoreManager shared { get; private set; }

    void Awake() {
        if (shared != null && shared != this) {
            Destroy(this);
        } else {
            shared = this;
        }
    }

    public void StartRecording() {
        IsRecording = true;
        scoreText.DOFade(1f, 1f);
    }
    public void StopRecording() {
        IsRecording = false;
        scoreText.DOFade(0f, 1f);
        score = 0.0f;
        innerScore100 = 0.0f;
        innerScore1000 = 0.0f;
        innerScore10000 = 0.0f;
    }

    private readonly string scoreTextFormat = "Score: {0:.}";
    private readonly Color yellowColor = new(249.0f / 255, 194.0f / 255, 43.0f / 255);
    private readonly Color orangeColor = new(251.0f / 255, 107.0f / 255, 29.0f / 255);
    private readonly Color redColor = new(234.0f / 255, 79.0f / 255, 54.0f / 255);
    private float score = 0.0f;
    private float innerScore100 = 0.0f;
    private float innerScore1000 = 0.0f;
    private float innerScore10000 = 0.0f;

    void Start() {
        IsRecording = false;
    }

    void Update() {
        if (IsRecording) {
            score += Time.deltaTime * scoreMultiplier;
            innerScore100 += Time.deltaTime * scoreMultiplier;
            innerScore1000 += Time.deltaTime * scoreMultiplier;
            innerScore10000 += Time.deltaTime * scoreMultiplier;
            scoreText.SetText(String.Format(scoreTextFormat, score));

            if (innerScore10000 > 10000) {
                innerScore10000 = 0.0f;
                innerScore1000 = 0.0f;
                innerScore100 = 0.0f;
                GetSequence(redColor, 1.6f, 0.3f);
            } else if (innerScore1000 > 1000) {
                innerScore1000 = 0.0f;
                innerScore100 = 0.0f;
                GetSequence(orangeColor, 1.4f, 0.3f);
            } else if (innerScore100 > 100) {
                innerScore100 = 0.0f;
                GetSequence(yellowColor, 1.2f, 0.3f);
            }
        }
    }

    Sequence GetSequence(Color color, float maxScale, float duration) {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(scoreText.transform.DOScale(maxScale, duration));
        sequence.Append(scoreText.transform.DOScale(1, duration));
        sequence.Insert(0, DOTween.Sequence()
            .Append(scoreText.DOColor(color, duration))
            .Append(scoreText.DOColor(Color.white, duration)));
        return sequence;
    }

    string GetRandomInsult() {
        if (gameOverInsults.Count > 0) {
            return gameOverInsults[UnityEngine.Random.Range(0, gameOverInsults.Count)];
        }
        return "Game Over";
    }

    public void GameOver() {
        StopRecording();
        gameOverText.SetText(GetRandomInsult());
        gameOverCanvas.DOFade(1f, 1f);
    }

    void ResetGame() {
        gameOverText.gameObject.SetActive(false);
        gameOverText.alpha = 0.0f;
    }
}
