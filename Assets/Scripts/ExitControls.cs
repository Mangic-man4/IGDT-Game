using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitControls : MonoBehaviour
{
    public void ExitScene()
    {
        SceneManager.LoadScene("Start Screen");
    }
}
