using UnityEngine;

public class GravityFlipTrigger : MonoBehaviour
{
    private PlayerPowerUps powerUps;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            powerUps = other.GetComponent<PlayerPowerUps>();
            ToggleGravity();
        }
    }

    void ToggleGravity()
    {
        if (powerUps != null)
        {
            powerUps.FlipGravity();

            // Update gravityFlipped flag to reflect the new state
            powerUps.gravityFlipped = !powerUps.gravityFlipped;
            powerUps.previousGravityState = powerUps.gravityFlipped;
        }
    }
}

