using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapInteraction : MonoBehaviour
{
    public int Respawn;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            ReloadCurrentScene();
            Debug.Log("Player has died!");

        }
    }
    void ReloadCurrentScene()
    {
        // Get the current scene's build index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the scene with the current build index
        SceneManager.LoadScene(currentSceneIndex);
    }
}

