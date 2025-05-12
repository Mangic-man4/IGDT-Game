using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    private void Start()
    {
        // Retrieve the score from Player Prefs
        int previousScore = PlayerPrefs.GetInt("PreviousScore", 0);

        // Update the score text with the previous score
        UpdateScoreText(previousScore);
    }

    // Method to update the score text with the given score value
    private void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    /*
        private void Start()
        {
            // Retrieve the score from the ScoreManager
            int score = ScoreManager.instance.GetScore();

            // Display the score in the score text
            scoreText.text = "Score: " + score.ToString();
        }*/
}
