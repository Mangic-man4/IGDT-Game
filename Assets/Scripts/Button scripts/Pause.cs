using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Pause : MonoBehaviour
{
    private GameObject PauseScreen;
    private TextMeshProUGUI pausedText;
    private AudioSource backgroundMusic;

    private PlayerController playerController;
    private TeleportControl teleportControl;

    void Start()
    {
        // Auto-assign PauseScreen by name or tag
        if (PauseScreen == null)
        {
            PauseScreen = GameObject.Find("PauseScreen");
            if (PauseScreen == null)
                Debug.LogError("PauseScreen not found in scene!");
        }

        // Auto-assign pausedText from PauseScreen's children
        if (pausedText == null && PauseScreen != null)
        {
            pausedText = PauseScreen.GetComponentInChildren<TextMeshProUGUI>(true);
            if (pausedText == null)
                Debug.LogWarning("No TextMeshProUGUI found in PauseScreen!");
        }

        // Auto-assign backgroundMusic from main camera
        if (backgroundMusic == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                if (!cam.TryGetComponent(out backgroundMusic))
                {
                    Debug.LogWarning("Main camera found but no AudioSource on it.");
                }

                if (backgroundMusic == null)
                    Debug.LogWarning("Main camera found but no AudioSource on it.");
            }
            else
            {
                Debug.LogWarning("Main camera not found.");
            }
        }

        // Auto-assign player controller
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (teleportControl == null)
            teleportControl = FindObjectOfType<TeleportControl>();

        if (PauseScreen != null)
            PauseScreen.SetActive(false);
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
        EnsureInitialized();

        if (PauseScreen == null)
        {
            Debug.LogError("PauseScreen is null!");
            return;
        }

        PauseManager.Instance.SetPauseState(true);
        PauseScreen.SetActive(true);

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
        EnsureInitialized();

        if (PauseScreen == null)
        {
            Debug.LogError("PauseScreen is null!");
            return;
        }

        Debug.Log("ResumeGame called successfully!");

        PauseManager.Instance.SetPauseState(false);
        PauseScreen.SetActive(false);

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

    private void EnsureInitialized()
    {
        if (PauseScreen == null)
            PauseScreen = GameObject.Find("PauseScreen");

        if (pausedText == null && PauseScreen != null)
        {
            TextMeshProUGUI[] tmps = PauseScreen.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (tmps.Length > 0)
                pausedText = tmps[0];
        }

        if (backgroundMusic == null && Camera.main != null)
            Camera.main.TryGetComponent(out backgroundMusic);

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (teleportControl == null)
            teleportControl = FindObjectOfType<TeleportControl>();
    }

}




// pause to do: Fix unpause when press space, teleport bug is back...