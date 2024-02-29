using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private const string PreviousSceneKey = "PreviousSceneIndex";

    public void LoadPreviousScene()
    {
        int previousSceneIndex = PlayerPrefs.GetInt(PreviousSceneKey, -1);
        if (previousSceneIndex != -1)
        {
            SceneManager.LoadScene(previousSceneIndex);
        }
        else
        {
            Debug.LogWarning("Previous scene index not found in PlayerPrefs.");
            // Handle the case where there is no previous scene saved
        }
    }

    public void SaveCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt(PreviousSceneKey, currentSceneIndex);
        PlayerPrefs.Save();
    }
}

