using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    [Tooltip("Vertical distance the player teleports.")]
    [SerializeField] private float teleportDistance = 3f;

    [Tooltip("Cooldown time between teleports or dashes.")]
    [SerializeField] private float teleportCooldown = 0.5f;

    private float lastTeleportTime;
    private bool isPaused = false;

    private PlayerPowerUps powerUps;

    private void Start()
    {
        powerUps = GetComponent<PlayerPowerUps>();
    }

    private void Update()
    {
        if (isPaused) return;

        bool canTeleport = Time.time > lastTeleportTime + teleportCooldown;

        if (canTeleport && Input.GetKeyDown(KeyCode.F))
        {
            if (powerUps.hasDash)
            {
                powerUps.PerformDash(); // Dash overrides teleport
            }
            else
            {
                PerformTeleport();
            }
        }
    }

    private void PerformTeleport()
    {
        Vector3 newPosition = transform.position;

        // Teleport up if below threshold, otherwise down
        if (VerticalModeManager.IsVertical)
        {
            // Horizontal teleport (left/right)
            newPosition += transform.position.x < 0
                ? Vector3.right * teleportDistance
                : Vector3.left * teleportDistance;
        }
        else
        {
            // Vertical teleport (up/down)
            newPosition += transform.position.y < -3f
                ? Vector3.up * teleportDistance
                : Vector3.down * teleportDistance;
        }

        transform.position = newPosition;
        lastTeleportTime = Time.time;
    }

    public void SetPauseState(bool pause)
    {
        isPaused = pause;
    }
}