using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitControls : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitScene();
        }
    }

    public void ExitScene()
    {
        // Set pause state to false before transitioning to the Start Screen
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.SetPauseState(false);
        }

        SceneManager.LoadScene("Start Screen");
    }
}