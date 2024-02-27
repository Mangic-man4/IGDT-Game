using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class LevelFinish : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    private float coinsCollected = 0; // Initialize coinsCollected
    private float timeElapsed = 0f;   // Initialize timeElapsed

    private void Start()
    {
        // Get the current scene name
        string sceneName = SceneManager.GetActiveScene().name;

        // Split the scene name by space
        string[] sceneParts = sceneName.Split(' ');

        // The last part should be the difficulty
        string difficulty = sceneParts[sceneParts.Length - 1];

        // Calculate the score using the ScoreManager
        int score = ScoreManager.instance.CalculateScore(difficulty, (int)coinsCollected, timeElapsed);
        scoreText.text = "Score: " + score.ToString();
    }

    // Call this method whenever the player collects a coin
    public void CollectCoin()
    {
        coinsCollected++;
    }

    private void Update()
    {
        // Increment timeElapsed by the time passed since the last frame
        timeElapsed += Time.deltaTime;
    }
}
        
        
        /*
        // Calculate score based on difficulty
        if (ScoreManager.instance != null)
        {
            int score = ScoreManager.instance.CalculateScore(difficulty);
            scoreText.text = "Score: " + score.ToString();
        }
        else
        {
            Debug.LogError("ScoreManager instance not found.");
        }
        */