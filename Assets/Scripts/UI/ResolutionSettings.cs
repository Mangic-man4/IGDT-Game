using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ResolutionSettings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Button applyButton;

    private Resolution[] resolutions;
    private int selectedResolutionIndex;
    private bool isFullscreen;

    void Start()
    {
        // Get system resolutions
        resolutions = Screen.resolutions
            .OrderByDescending(r => r.width * r.height) // Sort by pixel area
            .ThenByDescending(r => r.refreshRateRatio.value)       // Optional: sort by refresh rate too
            .ToArray();

        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if (!options.Contains(option))
            {
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);

        // Load saved settings (if any)
        selectedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        resolutionDropdown.value = selectedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        fullscreenToggle.isOn = isFullscreen;

        applyButton.onClick.AddListener(ApplySettings);
    }

    public void ApplySettings()
    {
        string[] dims = resolutionDropdown.options[resolutionDropdown.value].text.Split('x');
        int width = int.Parse(dims[0].Trim());
        int height = int.Parse(dims[1].Trim());
        bool fullscreen = fullscreenToggle.isOn;

        // Apply resolution with explicit fullscreen mode
        Screen.SetResolution(width, height, fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);

        // Save settings
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"Resolution set to {width}x{height}, Fullscreen: {fullscreen}");
    }

}
