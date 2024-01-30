using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    public float teleportDistance;
    public bool teleportUp;
    public float cooldown;
    private float lastTeleport;
<<<<<<< Updated upstream
=======
    private bool isPaused = false;
>>>>>>> Stashed changes

    void Update()
    {
        // Teleport logic remains unchanged
        if (Input.GetKeyDown(KeyCode.F) && Time.time > lastTeleport + cooldown)
        {
<<<<<<< Updated upstream
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
    }
}
=======
            HandleTeleportInput();
        }
    }

    void HandleTeleportInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time > lastTeleport + cooldown)
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
    
>>>>>>> Stashed changes
