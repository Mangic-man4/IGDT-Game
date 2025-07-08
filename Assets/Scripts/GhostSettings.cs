using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GhostSettings
{
    public static bool enableGhost = true;
    public static float ghostAlpha = 0.3f;
    public static bool enableTinting = true;
    public static Color ghostColor = Color.white;

    public static Color safeColor = Color.green;
    public static Color unsafeColor = Color.red;

    // Optional: load/save PlayerPrefs here if needed
    public static void LoadSettings()
    {
        enableGhost = PlayerPrefs.GetInt("Ghost_Enable",    1) == 1;
        enableTinting = PlayerPrefs.GetInt("Ghost_Tinting", 1) == 1;
        ghostAlpha = PlayerPrefs.GetFloat("Ghost_Alpha", 0.3f);

        float r = PlayerPrefs.GetFloat("Ghost_Color_R", 1f);
        float g = PlayerPrefs.GetFloat("Ghost_Color_G", 1f);
        float b = PlayerPrefs.GetFloat("Ghost_Color_B", 1f);
        ghostColor = new Color(r, g, b);
    }

    public static void SaveSettings()
    {
        PlayerPrefs.SetInt("Ghost_Enable", enableGhost ? 1 : 0);
        PlayerPrefs.SetInt("Ghost_Tinting", enableTinting ? 1 : 0);
        PlayerPrefs.SetFloat("Ghost_Alpha", ghostAlpha);

        PlayerPrefs.SetFloat("Ghost_Color_R", ghostColor.r);
        PlayerPrefs.SetFloat("Ghost_Color_G", ghostColor.g);
        PlayerPrefs.SetFloat("Ghost_Color_B", ghostColor.b);

        PlayerPrefs.Save();
    }

    public static void LoadColors()
    {
        safeColor = LoadColor("SafeColor", Color.green);
        unsafeColor = LoadColor("UnsafeColor", Color.red);
    }

    public static void SaveColors()
    {
        SaveColor("SafeColor", safeColor);
        SaveColor("UnsafeColor", unsafeColor);
        PlayerPrefs.Save();
    }


    private static void SaveColor(string keyPrefix, Color color)
    {
        PlayerPrefs.SetFloat($"{keyPrefix}_R", color.r);
        PlayerPrefs.SetFloat($"{keyPrefix}_G", color.g);
        PlayerPrefs.SetFloat($"{keyPrefix}_B", color.b);
    }

    private static Color LoadColor(string keyPrefix, Color fallback)
    {
        if (PlayerPrefs.HasKey($"{keyPrefix}_R"))
        {
            float r = PlayerPrefs.GetFloat($"{keyPrefix}_R");
            float g = PlayerPrefs.GetFloat($"{keyPrefix}_G");
            float b = PlayerPrefs.GetFloat($"{keyPrefix}_B");
            return new Color(r, g, b);
        }

        return fallback;
    }
}
