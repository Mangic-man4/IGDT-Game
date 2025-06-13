using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class PowerUpPickup : MonoBehaviour
{
    [Tooltip("Type of power-up this object provides")]
    public PowerUpType powerUpType;

    // ---------- internal state ----------
    private Vector3 startPos;
    private Quaternion startRot;
    private bool collected;

    // ---------- life-cycle ----------
    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        CheckpointManager.RegisterPickup(this);   // <- global list
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerPowerUps>(out var player))
        {
            player.CollectPowerUp(powerUpType);
            collected = true;
            gameObject.SetActive(false);          // disable instead of Destroy
        }
    }

    /// <summary> Called by the checkpoint system when it wants every
    /// pickup to be available again. </summary>
    public void Respawn()
    {
        if (!collected) return;                   // already active – do nothing

        transform.SetPositionAndRotation(startPos, startRot);
        gameObject.SetActive(true);
        collected = false;
    }

    private void OnDestroy()                      // tidy up when scene unloads
    {
        CheckpointManager.UnregisterPickup(this);
    }
}
