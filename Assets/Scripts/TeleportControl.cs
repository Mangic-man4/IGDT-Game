using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    public float teleportDistance; //The distance you want the character to teleport
    public bool teleportUp;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) //The character teleports once F is pressed
        {
            if (!teleportUp) //checks, if teleportUp is false
            {
                transform.position = new Vector2(transform.position.x, transform.position.y+teleportDistance);
                teleportUp = true;
            }
            else //if teleportUp is true, then teleports down
            {
                transform.position = new Vector2(transform.position.x, transform.position.y-teleportDistance);
                teleportUp = false;
            }
        }
    }
}
