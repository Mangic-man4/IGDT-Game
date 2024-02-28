using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelFinish : MonoBehaviour
{/*
    [SerializeField] private Text scoreText;
    [SerializeField] private Text coinCountText;
    [SerializeField] private Timer timerScript;

    private void Start()
    {
        // Get the number of coins collected from the coinCountText
        int coinsCollected = ExtractCoinsFromText();

        // Get the time elapsed from the timerScript
        float timeElapsed = timerScript.GetTimeElapsed();

        // Calculate the score using the provided formula
        int score = CalculateScore(coinsCollected, timeElapsed);

        // Update the score text to display the calculated score
        scoreText.text = "Score: " + score.ToString();
    }

    private int ExtractCoinsFromText()
    {
        // Split the coinCountText by ':' to get the part after ':', which should be the number of coins
        string[] parts = coinCountText.text.Split(':');
        if (parts.Length >= 2)
        {
            // Try parsing the number of coins
            if (int.TryParse(parts[1], out int coins))
            {
                return coins;
            }
        }
        // If parsing fails, return 0
        return 0;
    }

    private int CalculateScore(int coinsCollected, float timeElapsed)
    {
        // Define difficulty multiplier based on scene name
        float difficultyMultiplier = GetDifficultyMultiplierFromSceneName();

        // Calculate score using the formula: (Coins + Time) * Difficulty
        int score = (coinsCollected * 25 + Mathf.FloorToInt(600 - timeElapsed * 2)) * Mathf.FloorToInt(difficultyMultiplier);

        return score;
    }

    private float GetDifficultyMultiplierFromSceneName()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName.Contains("Easy"))
        {
            return 0.5f;
        }
        else if (sceneName.Contains("Normal"))
        {
            return 0.75f;
        }
        else if (sceneName.Contains("Hard"))
        {
            return 1.0f;
        }
        else
        {
            Debug.LogWarning("Difficulty not found in scene name. Defaulting to Normal difficulty.");
            return 0.75f;
        }
    }
}
*/



    [SerializeField] private Text scoreText;
    [SerializeField] private Text coinCount;
    [SerializeField] private Timer timer;


    private void Start()
    {
        // Get the current scene name
        string sceneName = SceneManager.GetActiveScene().name;

        // Split the scene name by space
        string[] sceneParts = sceneName.Split(' ');

        // The last part should be the difficulty
        string difficulty = sceneParts[sceneParts.Length - 1];

        // Extract the number of coins collected from the coinCount text
        int coinsCollected = ExtractCoinsFromText();

        // Get the time elapsed from the Timer script
        float timeElapsed = timer.GetTimeElapsed();

        // Calculate the score using the ScoreManager
        int score = ScoreManager.instance.CalculateScore(difficulty, coinsCollected, timeElapsed);
        scoreText.text = "Score: " + score.ToString();
    }

    private int ExtractCoinsFromText()
    {
        // Split the coinCount text by ':' to get the part after ':', which should be the number of coins
        string[] parts = coinCount.text.Split(':');
        if (parts.Length >= 2)
        {
            // Try parsing the number of coins
            if (int.TryParse(parts[1].Trim(), out int coins))
            {
                return coins;
            }
        }
        // If parsing fails, return 0
        return 0;
    }
    /*
    private void Update()
    {
        // Increment timeElapsed by the time passed since the last frame
        timeElapsed += Time.deltaTime;
        //Debug.Log("Time Elapsed: " + timeElapsed);

        }
    */
}
