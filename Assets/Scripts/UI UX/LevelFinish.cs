using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LevelFinish : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI coinCount;
    private Timer timer;
    private ScoreManager scoreManager;

    private void Start()
    {
        // Auto-assign UI elements by name
        foreach (var tmp in FindObjectsOfType<TextMeshProUGUI>(true))
        {
            if (tmp.name == "Score") scoreText = tmp;
            else if (tmp.name == "Coin Count") coinCount = tmp;
            else if (tmp.name == "Timer") timer = tmp.GetComponent<Timer>(); // if Timer is on the same object
        }

        // Fallbacks (optional)
        if (scoreText == null) Debug.LogWarning("Score TMP not found!");
        if (coinCount == null) Debug.LogWarning("Coin Count TMP not found!");

        if (timer == null)
        {
            timer = FindObjectOfType<Timer>();
            if (timer == null) Debug.LogWarning("Timer component not found!");
        }

        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager == null) Debug.LogWarning("ScoreManager not found!");
        }
    }

    private void Update()
    {
        if (scoreText == null || coinCount == null || timer == null || scoreManager == null)
            return;

        string sceneName = SceneManager.GetActiveScene().name;
        string[] sceneParts = sceneName.Split(' ');
        string difficulty = sceneParts[sceneParts.Length - 1];

        int coinsCollected = ExtractCoinsFromText();
        float timeElapsed = timer.GetTimeElapsed();
        int score = ScoreManager.instance.CalculateScore(difficulty, coinsCollected, timeElapsed);

        scoreText.text = "Score: " + score;
        scoreManager.SaveScore(score);
    }

    private int ExtractCoinsFromText()
    {
        string[] parts = coinCount.text.Split(':');
        if (parts.Length >= 2 && int.TryParse(parts[1].Trim(), out int coins))
            return coins;

        return 0;
    }
}


