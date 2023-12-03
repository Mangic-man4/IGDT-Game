using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapInteraction : MonoBehaviour
{
    public int Respawn;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene(Respawn);
            Debug.Log("Player has died!");

        }
    }

}

