using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DripDropOld : MonoBehaviour
{
    [Header("Color Settings")]
    public Color dripColor = Color.blue;

    private ParticleSystem splashSystem;

    private void Start()
    {
        var ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = dripColor;

        // Force it to play in case Play on Awake is off
        ps.Play();

        splashSystem = GetComponentInChildren<ParticleSystem>(true);
        if (splashSystem != null)
        {
            var splashMain = splashSystem.main;
            splashMain.startColor = dripColor;
        }
    }
}
