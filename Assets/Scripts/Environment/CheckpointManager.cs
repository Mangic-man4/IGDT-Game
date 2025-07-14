using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CheckpointManager
{
    private static readonly List<PowerUpPickup> pickups = new();

    public static void RegisterPickup(PowerUpPickup p)
    {
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
            p.Respawn();
    }
}

