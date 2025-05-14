using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapInteraction : MonoBehaviour
    
{
    void OnTriggerEnter2D(Collider2D other)
    {
        {
            if (other.CompareTag("Player") && other.TryGetComponent<PlayerController>(out var playerController))
            {
                playerController.Die();
                Debug.Log("Player has died! Triggered by TrapInteraction.");
            }
        }
    }
}


