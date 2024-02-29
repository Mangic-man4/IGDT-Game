using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private SceneController sceneController;

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
            // Set pause state to false before transitioning to the Start Screen
            PauseManager.Instance.SetPauseState(false);

            sceneController.SaveCurrentScene();

            SceneManager.LoadScene("Level Complete");
        }
    }
}
/*
 public class NextLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            // Set pause state to false before transitioning to the Start Screen
            PauseManager.Instance.SetPauseState(false);

            SceneManager.LoadScene("Level Complete");
        }
    }
}
*/