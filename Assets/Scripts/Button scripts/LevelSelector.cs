using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{


    public int level;
    public DifficultySelection difficultySelection; // Reference to the DifficultySelection script
    public Text difficultyText; // Add this line to reference the UI Text element

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

/*public class LevelSelector : MonoBehaviour
{
    public int level;
    public Text levelText;
    // Start is called before the first frame update
    void Start()
    {
        levelText.text = level.ToString();
    }

    public void OpenScene()
    {
        SceneManager.LoadScene("Level " + level.ToString());
    }
}*/
/*

public class LevelSelector : MonoBehaviour
{
    public int level;
    public Text levelText;
    public string difficulty = "Normal"; // Default difficulty

    void Start()
    {
        UpdateLevelText();
    }

    public void IncreaseDifficulty()
    {
        if (difficulty == "Easy")
            difficulty = "Normal";
        else if (difficulty == "Normal")
            difficulty = "Hard";
        // Add more difficulty levels if needed

        UpdateLevelText();
    }

    public void DecreaseDifficulty()
    {
        if (difficulty == "Hard")
            difficulty = "Normal";
        else if (difficulty == "Normal")
            difficulty = "Easy";
        // Add more difficulty levels if needed

        UpdateLevelText();
    }

    void UpdateLevelText()
    {
        levelText.text = "Level " + level + " " + difficulty;
    }

    public void OpenScene()
    {
        SceneManager.LoadScene("Level " + level + " " + difficulty);
    }
}
*/


