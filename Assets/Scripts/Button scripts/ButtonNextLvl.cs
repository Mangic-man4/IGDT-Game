using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonNextLvl : MonoBehaviour
{
    public void NextLvl()
    {
        // Get the completed level's name
        string completedLevelName = PlayerPrefs.GetString("CompletedLevel", "");

        // Check if the completed level name is not empty
        if (!string.IsNullOrEmpty(completedLevelName))
        {
            // Parse the completed level's name to extract the level number and difficulty
            string[] levelParts = completedLevelName.Split(' ');
            if (levelParts.Length >= 3)
            {
                string levelNumberStr = levelParts[1];
                string difficulty = levelParts[2];

                // Increment the level number
                int levelNumber;
                if (int.TryParse(levelNumberStr, out levelNumber))
                {
                    levelNumber++;
                }
                else
                {
                    Debug.LogError("Invalid level number format: " + levelNumberStr);
                    return;
                }

                // Construct the next level's name
                string nextLevelName = "Level " + levelNumber.ToString() + " " + difficulty;

                // Check if the next level exists in the build settings
                if (SceneExists(nextLevelName))
                {
                    // Load the next level
                    SceneManager.LoadScene(nextLevelName);
                }
                else
                {
                    Debug.LogWarning("Next level '" + nextLevelName + "' not found. Loading default level.");
                    // If the next level doesn't exist, load the default level (e.g., Level (Number) Normal)
                    SceneManager.LoadScene("Level " + levelNumber.ToString() + " Normal");
                }
            }
            else
            {
                Debug.LogError("Invalid level name format: " + completedLevelName);
            }
        }
        else
        {
            Debug.LogWarning("Completed level name not found.");
        }
    }

    // Helper method to check if a scene exists in the build settings
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}



