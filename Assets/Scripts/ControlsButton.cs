using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsButton : MonoBehaviour
{
    public void EnterControls()
    {
        // Set pause state to false before transitioning to the Start Screen
        PauseManager.Instance.SetPauseState(false);

        SceneManager.LoadScene("Controls");
    }
}
