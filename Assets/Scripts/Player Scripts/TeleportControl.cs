using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    public float teleportDistance;
    public float cooldown;
    private float lastTeleport;
    private bool isPaused = false;

    private PlayerPowerUps powerUps;

    void Start()
    {
        powerUps = GetComponent<PlayerPowerUps>();
    }

    void Update()
    {
        if (!isPaused && Input.GetKeyDown(KeyCode.F) && Time.time > lastTeleport + cooldown)
        {
            if (powerUps != null && powerUps.hasDash)
            {
                powerUps.PerformDash(); // Use dash instead of teleport
            }
            else
            {
                PerformTeleport();
            }
        }
    }

    void PerformTeleport()
    {
        Vector3 newPosition = transform.position;
        if (transform.position.y < -3f)
        {
            newPosition += Vector3.up * teleportDistance;
        }
        else
        {
            newPosition += Vector3.down * teleportDistance;
        }

        transform.position = newPosition;
        lastTeleport = Time.time;
    }

    public void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }
}