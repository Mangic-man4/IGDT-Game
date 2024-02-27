using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    // Singleton instance
    public static ScoreManager instance;

    // Constants for difficulty multipliers
    private const float EasyMultiplier = 0.5f;
    private const float NormalMultiplier = 0.75f;
    private const float HardMultiplier = 1.0f;

    private void Awake()
    {
        // Ensure only one instance of ScoreManager exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to calculate the score based on difficulty, coins, and time
    public int CalculateScore(string difficulty, int coinsCollected, float timeElapsed)
    {
        float difficultyMultiplier = GetDifficultyMultiplier(difficulty);

        // Calculate time score
        int timeScore = CalculateTimeScore(timeElapsed);

        // Calculate total score
        int totalScore = (int)((coinsCollected * 25 + timeScore) * difficultyMultiplier);

        return totalScore;
    }

    // Helper method to get difficulty multiplier
    private float GetDifficultyMultiplier(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "easy":
                return EasyMultiplier;
            case "normal":
                return NormalMultiplier;
            case "hard":
                return HardMultiplier;
            default:
                Debug.LogError("Unknown difficulty: " + difficulty);
                return 1.0f;
        }
    }

    // Helper method to calculate time score
    private int CalculateTimeScore(float timeElapsed)
    {
        // Time score starts at 600 and decreases by 2 for each second elapsed, with a minimum score of 0
        int timeScore = Mathf.Max(0, 600 - Mathf.RoundToInt(timeElapsed) * 2);
        return timeScore;
    }
}

/*
{
public DifficultySelection difficultySelection;
public Text coinCount;
public Text timerText;
public int coins = 0;
private float timerValue = 0f;
public static ScoreManager instance;


private void Start()
{
    if (instance == null)
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }

    StartCoroutine(UpdateTimer());
}

private IEnumerator UpdateTimer()
{
    while (true)
    {
        yield return new WaitForSeconds(1f);
        timerValue++;
        UpdateTimerDisplay();
    }
}

private void UpdateTimerDisplay()
{
    string formattedTime = timerValue.ToString("000");
    timerText.text = "Time: " + formattedTime;
}

public void IncrementCoinCount()
{
    coins++;
    coinCount.text = "Coins: " + coins;
}

public int CalculateScore()
{
    // Calculate the time-based score
    int timeScore = Mathf.Max(0, 600 - (int)timerValue * 2);

    // Get the difficulty multiplier from the DifficultySelection script
    float difficultyMultiplier = GetDifficultyMultiplier();

    // Calculate the total score
    int totalScore = (coins * 25 + timeScore) * (int)(difficultyMultiplier * 100);

    return totalScore;
}

private float GetDifficultyMultiplier()
{
    if (difficultySelection != null)
    {
        string difficulty = difficultySelection.difficultyText.text;
        switch (difficulty)
        {
            case "Easy":
                return 0.5f;
            case "Normal":
                return 0.75f;
            case "Hard":
                return 1.0f;
            default:
                Debug.LogError("Unknown difficulty level: " + difficulty);
                return 1.0f;
        }
    }
    else
    {
        Debug.LogError("DifficultySelection is not assigned.");
        return 1.0f;
    }
}
}*/
