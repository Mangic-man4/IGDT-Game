using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapInteraction : MonoBehaviour
{
    /*void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerController>(out var playerController) &&
            other.TryGetComponent<PlayerPowerUps>(out var powerUps))
        {
            if (powerUps.protectFromTraps && powerUps.TryUseShield())
            {
                Debug.Log("Trap hit absorbed by shield.");
                return;
            }

            playerController.Die();
            Debug.Log("Player has died! Triggered by TrapInteraction.");
        }
    }*/

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerPowerUps>(out var powerUps) &&
            other.TryGetComponent<PlayerController>(out var playerController))
        {
            if (powerUps.IsShieldActive() && powerUps.IsTrapProtectionEnabled())
            {
                if (powerUps.TryUseShield())
                {
                    Debug.Log("Trap hit absorbed by shield (Stay).");
                    return;
                }
            }

            if (!powerUps.IsInvincible())
            {
                playerController.Die();
                Debug.Log("Player has died! Triggered by TrapInteraction (Stay).");
            }
        }
    }
}



