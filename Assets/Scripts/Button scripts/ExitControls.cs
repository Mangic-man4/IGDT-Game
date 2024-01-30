using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitControls : MonoBehaviour
{
    public void ExitScene()
    {
        // Set pause state to false before transitioning to the Start Screen
        PauseManager.Instance.SetPauseState(false);

        SceneManager.LoadScene("Start Screen");
    }
}
