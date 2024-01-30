using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject PauseScreen;
    public Text pausedText;
    public AudioSource backgroundMusic;

    private bool isPaused = false;


    public PlayerController playerController;
    public TeleportControl teleportControl;

    void Start()
    {
        // Disable the pause menu UI and resume button at the start
        PauseScreen.SetActive(false);

        // Ensure these references are set in the Inspector or through script
        if (playerController == null)
            Debug.LogError("PlayerController reference is not set!");

        if (teleportControl == null)
            Debug.LogError("TeleportControl reference is not set!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed!");
            TogglePause();
        }
    }

    void TogglePause()
    {
        if (PauseScreen.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        PauseManager.Instance.SetPauseState(true);
        PauseScreen.SetActive(true);
        pausedText.gameObject.SetActive(true);
        PauseBackgroundMusic();

        // Call pause-related methods in other scripts
        if (playerController != null)
            playerController.SetPauseState(true);

        if (teleportControl != null)
            teleportControl.SetPauseState(true);
    }

    public void ResumeGame()
    {
        if (PauseScreen == null)
        {
            Debug.LogError("PauseScreen is null!");
            return;
        }

        Debug.Log("ResumeGame called successfully!");

        PauseManager.Instance.SetPauseState(false);
        PauseScreen.SetActive(false);
        pausedText.gameObject.SetActive(false);
        ResumeBackgroundMusic();

        // Call resume-related methods in other scripts
        if (playerController != null)
            playerController.SetPauseState(false);

        if (teleportControl != null)
            teleportControl.SetPauseState(false);
    }

    void PauseBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Pause();
        }
    }

    void ResumeBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
    }
}


// pause to do: Fix unpause when press space