using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ColorPickerToggle : MonoBehaviour
{
    public FlexibleColorPicker colorPicker;
    public Button closeButton;

    private bool pickerOpen = false;

    private enum PickerMode { None, Safe, Unsafe }
    private PickerMode currentMode = PickerMode.None;

    void Start()
    {
        colorPicker.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        closeButton.onClick.AddListener(CloseColorPicker);
    }

    public void ToggleSafeColorPicker()
    {
        if (pickerOpen && currentMode == PickerMode.Safe)
        {
            CloseColorPicker();
            return;
        }

        colorPicker.color = GhostSettings.safeColor;

        colorPicker.onColorChange.RemoveAllListeners();
        colorPicker.onColorChange.AddListener((newColor) =>
        {
            GhostSettings.safeColor = newColor;
            GhostSettings.SaveColors();
        });

        OpenColorPicker(PickerMode.Safe);
    }


    public void ToggleUnsafeColorPicker()
    {
        if (pickerOpen && currentMode == PickerMode.Unsafe)
        {
            CloseColorPicker();
            return;
        }

        colorPicker.color = GhostSettings.unsafeColor;

        colorPicker.onColorChange.RemoveAllListeners();
        colorPicker.onColorChange.AddListener((newColor) =>
        {
            GhostSettings.unsafeColor = newColor;
            GhostSettings.SaveColors();
        });

        OpenColorPicker(PickerMode.Unsafe);
    }

    private void OpenColorPicker(PickerMode mode)
    {
        colorPicker.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
        pickerOpen = true;
        currentMode = mode;
    }

    public void CloseColorPicker()
    {
        // Save current color back, just in case
        if (pickerOpen)
        {
            if (currentMode == PickerMode.Safe)
            {
                GhostSettings.safeColor = colorPicker.color;
            }
            else if (currentMode == PickerMode.Unsafe)
            {
                GhostSettings.unsafeColor = colorPicker.color;
            }

            GhostSettings.SaveColors();
        }

        colorPicker.onColorChange.RemoveAllListeners();
        colorPicker.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        pickerOpen = false;
        currentMode = PickerMode.None;

        PlayerPrefs.Save();
    }

}


