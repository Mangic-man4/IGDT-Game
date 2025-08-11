using UnityEditor;
using UnityEngine;

public static class ParticleRescue
{
    [MenuItem("Tools/Particles/Rescue: Stop and Clear all in Scene")]
    public static void StopAndClearAll()
    {
        var systems = Object.FindObjectsOfType<ParticleSystem>(true);
        int n = 0;
        foreach (var ps in systems)
        {
            var main = ps.main;
            main.playOnAwake = false; // don’t auto-run again
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            n++;
        }
        Debug.Log($"[ParticleRescue] Stopped & cleared {n} particle systems.");
    }
}
