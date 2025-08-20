using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CheckpointManager
{
    private static readonly List<PowerUpPickup> pickups = new();

    public static void RegisterPickup(PowerUpPickup p)
    {
        if (p.spawnedFromSpawner) return; // Skip spawner pickups completely
        if (!pickups.Contains(p)) pickups.Add(p);
    }


    public static void UnregisterPickup(PowerUpPickup p)
    {
        pickups.Remove(p);
    }

    /// <summary>Respawns every collected pickup in the scene.</summary>
    public static void RespawnAllPickups()
    {
        foreach (PowerUpPickup p in pickups)
        {
            if (p.spawnedFromSpawner) continue; // Ignore spawner pickups
            p.Respawn();
        }
    }


    public static void ResetAllEnvironmentObjects()
    {
        foreach (var platform in Object.FindObjectsOfType<CrumblePlatform>())
        {
            platform.ResetPlatform();
        }

        foreach (var block in Object.FindObjectsOfType<PushableBlock>())
        {
            block.ResetBlock();
        }

        foreach (var movingPlatform in Object.FindObjectsOfType<MovingPlatform>())
        {
            movingPlatform.ResetMovingPlatform();
        }
    }

    public static void ResetEnemies()
    {
        foreach (var mimic in Object.FindObjectsOfType<MimicController>())
        {
            mimic.ResetMimicPos();
        }

        foreach (var keyMimic in Object.FindObjectsOfType<KeyMimicController>())
        {
            keyMimic.ResetMimicPos();
        }

        foreach (var turret in Object.FindObjectsOfType<LaserTurret>(true))
        {
            turret.ResetTurret();
        }
    }

    public static void RespawnEnemies()
    {
        foreach (var mimic in Object.FindObjectsOfType<MimicController>(true))
        {
            mimic.RespawnEnemies();
        }

        foreach (var keyMimic in Object.FindObjectsOfType<KeyMimicController>(true))
        {
            keyMimic.RespawnEnemies();
        }

        foreach (var turret in Object.FindObjectsOfType<LaserTurret>(true))
        {
            turret.RespawnTurret();
        }
    }

}

