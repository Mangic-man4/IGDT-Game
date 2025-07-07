using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ExitToSettingsButton : MonoBehaviour
{
    private ControlsMenu controlsMenu;
    private MessageBoxUI messageBox;


    void Start()
    {
        controlsMenu = FindObjectOfType<ControlsMenu>();
        messageBox = FindObjectOfType<MessageBoxUI>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Close the message box if open, but do not return next time
            if (messageBox != null && messageBox.IsShowing)
            {
                messageBox.HideMessage();
                return;
            }

            // Prevent scene exit if rebinding is active
            if (controlsMenu != null && controlsMenu.IsWaitingForKey)
                return;

            // Now safe to exit
            ExitScene();
        }
    }



    public void ExitScene()
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.SetPauseState(false);
        }

        SceneManager.LoadScene("Settings");
    }
}

