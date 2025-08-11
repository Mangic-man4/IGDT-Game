using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionVFXScaler : MonoBehaviour
{
    [Tooltip("Visual radius of this effect when transform scale = 1.")]
    public float baseVisualRadius = 1.0f;

    // Optional extra multiplier if you want the visual slightly smaller/larger
    [Tooltip("Fine control after matching radius (usually leave at 1).")]
    public float visualTweak = 1.0f;

    /// <summary>
    /// Scales this VFX so its visible radius matches the provided world-space radius.
    /// </summary>
    public void SetRadius(float targetRadius)
    {
        if (baseVisualRadius <= 0f) baseVisualRadius = 1f;

        // scale = desired / baseline
        float scale = (targetRadius * baseVisualRadius) * visualTweak;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
