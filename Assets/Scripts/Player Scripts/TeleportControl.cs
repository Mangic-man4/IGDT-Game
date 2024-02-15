using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    public float teleportDistance;
    public bool teleportUp = true;
    public float cooldown;
    private float lastTeleport;
    private bool isPaused = false;

    void Update()
    {
        if (!isPaused)
        {
            if (Input.GetKeyDown(KeyCode.F) && Time.time > lastTeleport + cooldown)
            {
                HandleTeleportInput();
            }
        }
    }

    void HandleTeleportInput()
    {
        if (!isPaused && Time.time > lastTeleport + cooldown)
        {
            PerformTeleport();
        }
    }

    void PerformTeleport()
    {
        Vector3 newPosition = transform.position; // Get the current position
        if (teleportUp)
        {
            newPosition += Vector3.up * teleportDistance; // Teleport the player upward
        }
        else
        {
            newPosition += Vector3.down * teleportDistance; // Teleport the player downward
        }

        transform.position = newPosition; // Perform the teleportation

        teleportUp = !teleportUp; // Toggle teleport direction
        lastTeleport = Time.time; // Update last teleport time
    }

    public void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }
    public void ResetTeleportDirection()
    {
        teleportUp = true; // Reset teleportUp to true
    }
}