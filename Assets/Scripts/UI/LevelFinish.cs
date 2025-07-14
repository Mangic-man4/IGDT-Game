using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            else if (tmp.name == "Timer") timer = tmp.GetComponent<Timer>();
        }

        if (scoreText == null) Debug.LogWarning("Score TMP not found!");
        if (coinCount == null) Debug.LogWarning("Coin Count TMP not found!");
        if (timer == null) timer = FindObjectOfType<Timer>();
        if (scoreManager == null) scoreManager = FindObjectOfType<ScoreManager>();
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
    }

    public void FinalizeScore()
    {
        if (scoreText == null || coinCount == null || timer == null || scoreManager == null)
            return;

        string sceneName = SceneManager.GetActiveScene().name;
        string[] sceneParts = sceneName.Split(' ');
        string difficulty = sceneParts[sceneParts.Length - 1];

        int coinsCollected = ExtractCoinsFromText();
        float timeElapsed = timer.GetTimeElapsed();
        int score = ScoreManager.instance.CalculateScore(difficulty, coinsCollected, timeElapsed);

        PlayerPrefs.SetInt("PreviousScore", score);
        PlayerPrefs.SetString("LastCompletedScene", sceneName);
        SavePlayerHighScore(sceneName, score);
        PlayerPrefs.Save();

        scoreManager.SaveScore(score);
    }


    private void SavePlayerHighScore(string sceneName, int newScore)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        string personalKey = $"Hiscore_{playerName}_{sceneName}";

        int previousBest = PlayerPrefs.GetInt(personalKey, 0);
        if (newScore > previousBest)
        {
            PlayerPrefs.SetInt(personalKey, newScore);
        }

        // --- Global best check ---
        string globalScoreKey = $"GlobalHiscore_{sceneName}";
        string globalNameKey = $"GlobalHiscoreName_{sceneName}";

        int globalBest = PlayerPrefs.GetInt(globalScoreKey, 0);
        if (newScore > globalBest)
        {
            PlayerPrefs.SetInt(globalScoreKey, newScore);
            PlayerPrefs.SetString(globalNameKey, playerName);
        }
    }


    private int ExtractCoinsFromText()
    {
        string[] parts = coinCount.text.Split(':');
        if (parts.Length >= 2 && int.TryParse(parts[1].Trim(), out int coins))
            return coins;

        return 0;
    }
}
