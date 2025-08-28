using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MimicFireball : MonoBehaviour
{
    private KeyMimicController owner;

    public void SetOwner(KeyMimicController mimic)
    {
        owner = mimic;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") &&
            other.TryGetComponent<PlayerPowerUps>(out var powerUps) &&
            other.TryGetComponent<PlayerController>(out var playerController))
        {
            if (powerUps.IsShieldActive() && powerUps.IsEnemyProtectionEnabled())
            {
                if (powerUps.TryUseShield())
                {
                    Debug.Log("Fireball hit absorbed by shield.");
                    Destroy(gameObject);
                    return;
                }
            }

            if (!powerUps.IsInvincible())
            {
                playerController.Die();
                Debug.Log("Player hit by fireball!");
            }

            if (owner != null && SceneManager.GetActiveScene().name.Contains("Apprentice"))
            {
                owner.ForceDeaggro();
            }

            Destroy(gameObject);
        }

        if (other.CompareTag("ObbyCourse"))
        {
            Destroy(gameObject);
        }
    }

}

