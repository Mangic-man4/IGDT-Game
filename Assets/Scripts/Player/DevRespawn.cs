using UnityEngine;

public class DevRespawn : MonoBehaviour
{
    public float respawnPointX = 140f; //-15 for spawn
    public float respawnPointY = 7.22f; //-15 for spawn

    // Reference to the TeleportControl script
    public TeleportControl teleportControl;

    void Start()
    {
        // Find the TeleportControl script on the same GameObject
        teleportControl = GetComponent<TeleportControl>();
    }

    void Update()
    {
        if (KeyBindings.GetKeyDown(ActionKey.Respawn))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        // Set the respawn point with the specified X and Y coordinates and the current Z position
        Vector3 respawnPosition = new (respawnPointX, respawnPointY, transform.position.z);

        // Move the player to the respawn point
        transform.position = respawnPosition;
    }
}