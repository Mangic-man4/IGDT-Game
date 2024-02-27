using UnityEngine;

public class DevRespawn : MonoBehaviour
{
    public float respawnPointX = 140f;
    public float respawnPointY = 7.22f;

    // Reference to the TeleportControl script
    public TeleportControl teleportControl;

    void Start()
    {
        // Find the TeleportControl script on the same GameObject
        teleportControl = GetComponent<TeleportControl>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        // Set teleportUp to true/false during respawn
        teleportControl.teleportUp = true;

        // Set the respawn point with the specified X and Y coordinates and the current Z position
        Vector3 respawnPosition = new Vector3(respawnPointX, respawnPointY, transform.position.z);

        // Move the player to the respawn point
        transform.position = respawnPosition;
    }
}