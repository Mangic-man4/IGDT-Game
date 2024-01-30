using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject PauseScreen;
    public Text pausedText;
    public AudioSource backgroundMusic;


    void Start()
    {
        // Disable the pause menu UI and resume button at the start
        PauseScreen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
        Time.timeScale = 0f; // Pause the game
        PauseScreen.SetActive(true);
        pausedText.gameObject.SetActive(true);
        PauseBackgroundMusic();

    }

    public void ResumeGame()
    {
        if (PauseScreen == null)
        {
            Debug.LogError("PauseScreen is null!");
            return;
        }

        Time.timeScale = 1f; // Resume the game
        PauseScreen.SetActive(false);
        pausedText.gameObject.SetActive(false);
        ResumeBackgroundMusic();

        if (PauseScreen != null)
        {
            Debug.Log("ResumeGame called successfully!");
            Time.timeScale = 1f; // Resume the game
            PauseScreen.SetActive(false);
            pausedText.gameObject.SetActive(false);
            ResumeBackgroundMusic();
        }
        else
        {
            Debug.LogError("PauseScreen is not assigned!");
        }

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

// pause to do: Fix resume button, add pause to 2nd lvl, fix movement/teleport queueing during pause