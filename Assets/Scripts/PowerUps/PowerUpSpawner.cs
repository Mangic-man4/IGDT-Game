using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Tooltip("Prefab of the power-up to spawn")]
    public GameObject powerUpPrefab;

    [Tooltip("Delay before the power-up respawns (in seconds)")]
    public float respawnDelay = 10f;

    [Tooltip("Where to spawn the power-up (default is this object's position)")]
    public Transform spawnPoint;

    private GameObject currentPowerUp;

    private static readonly List<PowerUpSpawner> allSpawners = new();

    private void Awake()
    {
        allSpawners.Add(this);
    }

    private void OnDestroy()
    {
        allSpawners.Remove(this);
    }

    private void Start()
    {
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }

        SpawnPowerUp();
    }

    private void SpawnPowerUp()
    {
        currentPowerUp = Instantiate(powerUpPrefab, spawnPoint.position, spawnPoint.rotation);

        // Hook into the pickup system so we know when it's collected
        if (currentPowerUp.TryGetComponent(out PowerUpPickup pickup))
        {
            pickup.spawnedFromSpawner = true;
            pickup.OnCollected += HandlePickupCollected;
        }
    }

    private void HandlePickupCollected()
    {
        // Unsubscribe so we don't leak memory
        if (currentPowerUp != null && currentPowerUp.TryGetComponent(out PowerUpPickup oldPickup))
        {
            oldPickup.OnCollected -= HandlePickupCollected;
        }

        currentPowerUp = null;
        Invoke(nameof(SpawnPowerUp), respawnDelay);
    }

    public void ForceSpawn()
    {
        // Cancel any delayed respawn
        CancelInvoke(nameof(SpawnPowerUp));

        // If one is already active, do nothing (optional safety)
        if (currentPowerUp != null) return;

        SpawnPowerUp();
    }

    public static void ForceSpawnOnAll()
    {
        foreach (var spawner in allSpawners)
        {
            spawner.ForceSpawn();
        }
    }
}

