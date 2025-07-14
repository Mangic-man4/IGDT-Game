using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalModeManager : MonoBehaviour
{
    public static bool IsVertical = false;

    [Tooltip("Enable vertical mode for this scene (levels that go bottom-to-top).")]
    public bool verticalModeEnabled = false;

    private void Awake()
    {
        IsVertical = verticalModeEnabled;
    }
}

