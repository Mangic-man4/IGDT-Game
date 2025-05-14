using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PowerUpPickup : MonoBehaviour
{
    [Tooltip("Type of power-up this object provides")]
    public PowerUpType powerUpType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Safe and allocation-free check for PlayerPowerUps component
        if (other.TryGetComponent<PlayerPowerUps>(out var player))
        {
            Debug.Log($"Picked up Power-Up: {powerUpType}");
            player.CollectPowerUp(powerUpType);
            Destroy(gameObject);
        }
    }
}

