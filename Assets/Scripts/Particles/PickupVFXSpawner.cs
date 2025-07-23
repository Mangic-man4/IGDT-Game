using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupVFXSpawner : MonoBehaviour
{
    public static void Spawn(Vector3 position, Color color, GameObject vfxPrefab)
    {
        if (vfxPrefab == null) return;

        GameObject fx = Instantiate(vfxPrefab, position, Quaternion.identity);

        if (fx.TryGetComponent<ParticleSystem>(out var ps))
        {
            var main = ps.main;
            main.startColor = color;
        }
    }
}

