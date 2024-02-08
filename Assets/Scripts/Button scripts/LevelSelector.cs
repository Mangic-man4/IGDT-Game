using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

public class LevelSelector : MonoBehaviour
{
    public int level;
    public Text difficultyText;

    public void OpenScene()
    {
        string difficulty = difficultyText.text;
        SceneManager.LoadScene("Level " + level.ToString() + " " + difficulty);
    }
}
