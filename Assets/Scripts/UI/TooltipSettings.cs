using UnityEngine;

public static class TooltipSettings
{
    private const string TooltipEnabledKey = "TooltipsEnabled";

    public static bool TooltipsEnabled
    {
        get => PlayerPrefs.GetInt(TooltipEnabledKey, 1) == 1; // Default to enabled
        set
        {
            PlayerPrefs.SetInt(TooltipEnabledKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
    