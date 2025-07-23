using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class PowerUpPickup : MonoBehaviour
{
    public event System.Action OnCollected;

    [Tooltip("Type of power-up this object provides")]
    public PowerUpType powerUpType;

    [Tooltip("Duration in seconds for Speed and DoubleJump powerups")]
    public float duration = 30f;

    [Tooltip("Fireball charge amount if this is a Fireball powerup")]
    public int fireballAmount = 10;

    [Tooltip("Picking up the powerup disables the object")]
    public bool disableOnPickup;

    [HideInInspector] public bool spawnedFromSpawner = false;

    [Header("Item pickup VFX settings")]
    [SerializeField] private GameObject pickupVFX; // Drag your particle prefab in Inspector
    [SerializeField] private Color vfxColor = Color.white   ;
    //[SerializeField] private Gradient vfxGradient;


    // ---------- internal state ----------
    private Vector3 startPos;
    private Quaternion startRot;
    private bool collected;

    // ---------- life-cycle ----------
    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        // Only register if not spawned from a spawner
        if (!spawnedFromSpawner)
        {
            CheckpointManager.RegisterPickup(this); // Global list
        }
    }
        
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerPowerUps>(out var player))
        {
            player.CollectPowerUp(powerUpType, duration, fireballAmount);

            PickupVFXSpawner.Spawn(transform.position, vfxColor, pickupVFX);

            collected = true;
            gameObject.SetActive(!disableOnPickup);

            OnCollected?.Invoke(); // Notify spawner or any other listener

        }
    }

    // Called by the checkpoint system when it wants every pickup to be available again.
    public void Respawn()
    {
        if (!collected) return; // already active; do nothing

        transform.SetPositionAndRotation(startPos, startRot);
        gameObject.SetActive(true);
        collected = false;
    }

    private void OnDestroy() // tidy up when scene unloads
    {
        if (!spawnedFromSpawner)
        {
            CheckpointManager.UnregisterPickup(this);
        }
    }
}
