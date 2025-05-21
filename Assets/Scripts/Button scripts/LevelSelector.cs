using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{


    public int level;
    public DifficultySelection difficultySelection; // Reference to the DifficultySelection script
    public TextMeshProUGUI difficultyText; // Add this line to reference the UI Text element

    public void OpenScene()
    {
        if (difficultyText != null)
        {
            // Read the difficulty directly from the Text component
            string difficulty = difficultyText.text;

            // Ensure the string format matches your scene naming convention
            string sceneName = "Level " + level.ToString() + " " + difficulty;
            Debug.Log("Loading scene: " + sceneName);

            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("DifficultyText is not assigned in LevelSelector.");
        }
    }
}

