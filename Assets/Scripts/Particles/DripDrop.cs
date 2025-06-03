using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DripDrop : MonoBehaviour
{
    [Header("Color Settings")]

    [SerializeField] private float splashDelay = 1.18f; // when the splash should trigger
    [SerializeField] private float destroyDelay = 1.55f; // when the whole object should be destroyed

    private ParticleSystem dripSystem;
    private ParticleSystem splashSystem;

    private void Start()
    {
        dripSystem = GetComponent<ParticleSystem>();
        splashSystem = GetComponentInChildren<ParticleSystem>(true);

        // Apply color
        if (dripSystem != null)
        {
            var main = dripSystem.main;
            dripSystem.Play();
        }

        if (splashSystem != null)
        {
            var splashMain = splashSystem.main;
        }

        // Trigger splash after delay
        Invoke(nameof(PlaySplash), splashDelay);

        // Destroy entire object after total delay
        Destroy(gameObject, destroyDelay);
    }

    private void PlaySplash()
    {
        if (splashSystem != null)
        {
            splashSystem.transform.position = transform.position;
            splashSystem.Play();
        }

        if (dripSystem != null)
        {
            dripSystem.Stop();
        }
    }
}

