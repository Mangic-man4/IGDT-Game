using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    // Singleton instance
    public static ScoreManager instance;

    // Constants for difficulty multipliers
    private const float ApprenticeMultiplier = 0.5f;
    private const float AdeptMultiplier = 0.75f;
    private const float WizardMultiplier = 1.0f;
    private const float ArchmageMultiplier = 1.5f;

    private readonly static HashSet<string> loggedDifficultyErrors = new();

    private int score = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // Detach from parent if needed
            if (transform.parent != null)
                transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
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
        int score = (int)((coinsCollected * 25 + timeScore) * difficultyMultiplier);

        return score;
    }

    public int GetScore()
    {
        return score;
    }

    // Helper method to get difficulty multiplier
    private float GetDifficultyMultiplier(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "apprentice":
                return ApprenticeMultiplier;
            case "adept":
                return AdeptMultiplier;
            case "wizard":
                return WizardMultiplier;
            case "archmage":
                return ArchmageMultiplier;
            default:
                if (!loggedDifficultyErrors.Contains(difficulty))
                {
                    Debug.LogError("Unknown difficulty: " + difficulty);
                    loggedDifficultyErrors.Add(difficulty);
                }
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
    public void SaveScore(int score)
    {
        PlayerPrefs.SetInt("PreviousScore", score);
        //Debug.Log("Score saved: " + score);

    }

    public void ResetScore() 
    { 
        score = 0; 
    }
    // Simple accessors so a checkpoint can snapshot the score
    public void SetScore(int v) => score = v;

}
