using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    public float teleportDistance;
    public bool teleportUp;
    public float cooldown;
    private float lastTeleport;
    private bool isPaused = false;


    void Update()
    {
        if (!isPaused)
        {
            // Teleport logic remains unchanged
            if (Input.GetKeyDown(KeyCode.F) && Time.time > lastTeleport + cooldown)
            {

                HandleTeleportInput();
            }
        }
    }
        void HandleTeleportInput()
        {
            if (!isPaused && Input.GetKeyDown(KeyCode.F) && Time.time > lastTeleport + cooldown)
            {
                PerformTeleport();
            }
        }

        void PerformTeleport()
        {
            if (!teleportUp)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + teleportDistance);
                teleportUp = true;
            }
            else
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - teleportDistance);
                teleportUp = false;
            }
            lastTeleport = Time.time;
        }
    public void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }
}

