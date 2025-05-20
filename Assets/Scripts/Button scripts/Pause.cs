using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Pause : MonoBehaviour
{
    public GameObject PauseScreen;
    private TextMeshProUGUI pausedText;
    public AudioSource backgroundMusic;

    private PlayerController playerController;
    private TeleportControl teleportControl;

    void Start()
    {   
        // Disable the pause menu UI and resume button at the start
        PauseScreen.SetActive(false);

        // Attempt to auto-assign pausedText from the children of PauseScreen
        if (pausedText == null && PauseScreen != null)
        {
            foreach (var tmp in PauseScreen.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                Debug.Log("Found TMP child: " + tmp.name); // Log TMP components found
                if (tmp.name == "pausedText")
                {
                    pausedText = tmp;
                    break;
                }
            }
        }

        // Log a warning if pausedText is still not found
        if (pausedText == null)
        {
            Debug.LogWarning("pausedText TMP not found in PauseScreen! Looking for a fallback.");
        }

        // Fallback mechanism to find pausedText if not set by the above method
        if (pausedText == null)
        {
            pausedText = FindObjectOfType<TextMeshProUGUI>(); // Fallback to any TMP text in the scene
            if (pausedText == null)
            {
                Debug.LogError("No TextMeshProUGUI component found in the scene!");
            }
        }

        // Initialize other references (playerController, teleportControl)
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (teleportControl == null)
            teleportControl = FindObjectOfType<TeleportControl>();

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
        if (PauseScreen == null)
        {
            Debug.LogError("PauseScreen is null!");
            return;
        }

        PauseManager.Instance.SetPauseState(true);
        PauseScreen.SetActive(true);

        EnsurePausedTextInitialized();

        if (pausedText != null)
            pausedText.gameObject.SetActive(true);

        PauseBackgroundMusic();

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

        EnsurePausedTextInitialized();

        if (pausedText != null)
            pausedText.gameObject.SetActive(false);
        else
            Debug.LogWarning("pausedText was still null during ResumeGame!");

        ResumeBackgroundMusic();

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
    private void EnsurePausedTextInitialized()
    {
        if (pausedText == null && PauseScreen != null)
        {
            foreach (var tmp in PauseScreen.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (tmp.name == "pausedText")
                {
                    pausedText = tmp;
                    break;
                }
            }

            if (pausedText == null)
                Debug.LogWarning("pausedText still not found when trying to initialize during ResumeGame.");
        }
    }

}




// pause to do: Fix unpause when press space, teleport bug is back...