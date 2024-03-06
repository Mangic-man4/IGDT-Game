using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    public float teleportDistance;
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
        if (transform.position.y < -3f)
        {
            newPosition += Vector3.up * teleportDistance; // Teleport the player upward if y position is less than -3
        }
        else
        {
            newPosition += Vector3.down * teleportDistance; // Teleport the player downward if y position is greater than -3
        }

        transform.position = newPosition; // Perform the teleportation
        lastTeleport = Time.time; // Update last teleport time
    }

    public void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }
}