using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonNextLvl : MonoBehaviour
{
    public void NextLvl()
    {
        // Set pause state to false before transitioning to the Start Screen
        PauseManager.Instance.SetPauseState(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}