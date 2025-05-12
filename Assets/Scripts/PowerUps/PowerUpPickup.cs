using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public PowerUpType powerUpType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPowerUps player = other.GetComponent<PlayerPowerUps>();
            if (player != null)
            {
                Debug.Log("Picked up Power Up: " + powerUpType);
                player.CollectPowerUp(powerUpType);
                Destroy(gameObject);
            }
        }
    }
}

