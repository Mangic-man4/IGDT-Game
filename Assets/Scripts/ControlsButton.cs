using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsButton : MonoBehaviour
{
    public void EnterControls()
    {
        SceneManager.LoadScene("Controls");
    }
}
