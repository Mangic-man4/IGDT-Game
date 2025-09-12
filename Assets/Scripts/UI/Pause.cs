using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class Pause : MonoBehaviour
{
    private GameObject PauseScreen;
    private TextMeshProUGUI pausedText;
    private AudioSource backgroundMusic;

    private PlayerController playerController;
    private TeleportControl teleportControl;

    private GameObject pauseMainPanel;
    private GameObject settingsPanel;

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

        if (pauseMainPanel == null)
            pauseMainPanel = GameObject.Find("PauseMainPanel");

        if (settingsPanel == null)
            settingsPanel = GameObject.Find("SettingsPanel");

        if (settingsPanel != null)
            settingsPanel.SetActive(false); // hide at start

        AudioListener.pause = false;// Ensure audio is not paused at start
    }



    void Update()
    {
        if (KeyBindings.GetKeyDown(ActionKey.Pause))
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
            
        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (pausedText != null)
            pausedText.gameObject.SetActive(true);

        PauseAllAudio(); // <- instead of PauseBackgroundMusic()
        //PauseBackgroundMusic();

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

        ResumeAllAudio(); // <- instead of ResumeBackgroundMusic()
        //ResumeBackgroundMusic();

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

        Transform mainPanelTransform = PauseScreen.transform.Find("PauseMainPanel");
        if (mainPanelTransform != null)
            pauseMainPanel = mainPanelTransform.gameObject;

        Transform settingsPanelTransform = PauseScreen.transform.Find("SettingsPanel");
        if (settingsPanelTransform != null)
            settingsPanel = settingsPanelTransform.gameObject;


        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (teleportControl == null)
            teleportControl = FindObjectOfType<TeleportControl>();
    }

    public void OpenSettings()
    {
        EnsureInitialized();

        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        EnsureInitialized();

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(true);
    }

    void PauseAllAudio()
    {
        // Pauses all AudioSources where ignoreListenerPause == false (default)
        AudioListener.pause = true;
    }

    void ResumeAllAudio()
    {
        AudioListener.pause = false;
    }

    void Awake()
    {
        // Make sure a new scene never starts muted
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called after each scene loads
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure global audio is unpaused and time is normal
        AudioListener.pause = false;
        Time.timeScale = 1f;

        // Reset your own pause state/UI
        if (PauseManager.Instance != null)
            PauseManager.Instance.SetPauseState(false);

        if (PauseScreen != null) PauseScreen.SetActive(false);
        if (pausedText != null) pausedText.gameObject.SetActive(false);
    }

}