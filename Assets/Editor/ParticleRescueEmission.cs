using UnityEditor;
using UnityEngine;

public static class ParticleRescueEmission
{
    [MenuItem("Tools/Particles/Rescue: Disable Emission in Scene")]
    public static void DisableEmissionAll()
    {
        var systems = Object.FindObjectsOfType<ParticleSystem>(true);
        int n = 0;
        foreach (var ps in systems)
        {
            var emission = ps.emission;
            emission.enabled = false;
            n++;
        }
        Debug.Log($"[ParticleRescue] Disabled emission on {n} particle systems.");
    }

    [MenuItem("Tools/Particles/Rescue: Enable Emission in Scene")]
    public static void EnableEmissionAll()
    {
        var systems = Object.FindObjectsOfType<ParticleSystem>(true);
        int n = 0;
        foreach (var ps in systems)
        {
            var emission = ps.emission;
            emission.enabled = true;
            n++;
        }
        Debug.Log($"[ParticleRescue] Enabled emission on {n} particle systems.");
    }
}
