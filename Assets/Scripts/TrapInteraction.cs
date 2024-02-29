using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapInteraction : MonoBehaviour
    
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if the player has a player controller script attached
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Respawn the player using the player controller's respawn method
                playerController.Respawn();
            }
            else
            {
                // If no player controller script found, reload the scene
                ReloadCurrentScene();
            }
            Debug.Log("Player has died! Reloading scene...");
        }
    }

    public void ReloadCurrentScene()
    {
        // Get the current scene's build index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the scene with the current build index
        SceneManager.LoadScene(currentSceneIndex);

        // Set Time.timeScale to 1f after scene reloads
        Time.timeScale = 1f;
    }
}


