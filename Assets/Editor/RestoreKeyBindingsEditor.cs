using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ResetKeyBindingsEditor
{
    [MenuItem("Tools/Reset Keybinds to Defaults")]
    public static void ResetKeybindingsToDefault()
    {
        // ActionKey bindings
        PlayerPrefs.SetInt("Teleport", (int)KeyCode.F);
        PlayerPrefs.SetInt("Jump", (int)KeyCode.Space);
        PlayerPrefs.SetInt("Respawn", (int)KeyCode.R);
        PlayerPrefs.SetInt("Pause", (int)KeyCode.Escape);
        PlayerPrefs.SetInt("FireballAttack", (int)KeyCode.E);
        PlayerPrefs.SetInt("ToggleGhost", (int)KeyCode.G);

        // ActionAxis.Horizontal bindings
        PlayerPrefs.SetInt("Horizontal_Pos", (int)KeyCode.D);
        PlayerPrefs.SetInt("Horizontal_Neg", (int)KeyCode.A);

        PlayerPrefs.Save();
        Debug.Log("Keybindings reset to default values.");
    }
}
