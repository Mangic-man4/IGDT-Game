using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuButton : MonoBehaviour
{
    public void MainMenu()
    {
        // Set pause state to false before transitioning to the Start Screen
        PauseManager.Instance.SetPauseState(false);

        SceneManager.LoadScene("Start Screen");
    }
}
