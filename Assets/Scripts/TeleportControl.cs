using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    public float teleportDistance;
    public bool teleportUp;
    public float cooldown;
    private float lastTeleport;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("teleport", false);
        // Teleport logic remains unchanged
        if (Input.GetKeyDown(KeyCode.F) && Time.time > lastTeleport + cooldown)
        {
            animator.SetBool("teleport", true);
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