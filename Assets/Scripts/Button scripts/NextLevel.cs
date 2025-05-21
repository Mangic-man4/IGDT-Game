using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private SceneController sceneController;
    public string currentLevelName;

    private void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
        if (sceneController == null)
        {
            Debug.LogError("SceneController not found in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            // Finalize the score before changing scenes
            var levelFinish = FindObjectOfType<LevelFinish>();
            if (levelFinish != null)
            {
                levelFinish.FinalizeScore();
            }
            else
            {
                Debug.LogWarning("LevelFinish script not found before scene change.");
            }

            // Set pause state to false before transitioning to the Start Screen
            PauseManager.Instance.SetPauseState(false);

            sceneController.SaveCurrentScene();
            sceneController.SaveCompletedLevel(currentLevelName);

            SceneManager.LoadScene("Level Complete");
        }
    }

}
