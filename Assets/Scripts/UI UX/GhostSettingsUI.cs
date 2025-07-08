using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostSettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject teleportGhost;
    [SerializeField] private LineRenderer teleportLine;

    public Slider ghostOpacitySlider;
    public Toggle ghostTintToggle;
    public Toggle ghostEnableToggle;

    [SerializeField] private FlexibleColorPicker safeColorPicker;
    [SerializeField] private FlexibleColorPicker unsafeColorPicker;

    public bool suppressGhostToggleUpdate = false;

    void Start()
    {
        // Set initial UI values
        if (ghostOpacitySlider != null)
        {
            ghostOpacitySlider.value = GhostSettings.ghostAlpha;
            ghostOpacitySlider.onValueChanged.AddListener(SetGhostOpacity);
        }

        if (ghostTintToggle != null)
        {
            ghostTintToggle.isOn = GhostSettings.enableTinting;
            ghostTintToggle.onValueChanged.AddListener(SetTinting);
        }

        if (ghostEnableToggle != null)
        {
            ghostEnableToggle.isOn = GhostSettings.enableGhost;
            ghostEnableToggle.onValueChanged.AddListener(SetGhostEnabled);
        }

        // Initialize pickers with current saved colors
        safeColorPicker.color = GhostSettings.safeColor;
        unsafeColorPicker.color = GhostSettings.unsafeColor;

        // Listen for changes
        safeColorPicker.onColorChange.AddListener(OnSafeColorChanged);
        unsafeColorPicker.onColorChange.AddListener(OnUnsafeColorChanged);
    }

    public void SetGhostOpacity(float value)
    {
        GhostSettings.ghostAlpha = value;
        GhostSettings.SaveSettings();
    }

    public void SetTinting(bool enabled)
    {
        GhostSettings.enableTinting = enabled;
        GhostSettings.SaveSettings();
        UpdateLineRendererColors();
    }

    public void SetGhostEnabled(bool enabled)
    {
        if (suppressGhostToggleUpdate)
            return;

        GhostSettings.enableGhost = enabled;
        GhostSettings.SaveSettings();

        // Update visuals
        if (teleportGhost != null)
            teleportGhost.SetActive(enabled);

        UpdateLineRendererColors();
    }




    public void OnGhostToggleChanged(bool isOn)
    {
        SetGhostEnabled(isOn);
    }

    public void OnSafeColorChanged(Color newColor)
    {
        GhostSettings.safeColor = newColor;
        GhostSettings.SaveColors();
        UpdateLineRendererColors();
    }

    public void OnUnsafeColorChanged(Color newColor)
    {
        GhostSettings.unsafeColor = newColor;
        GhostSettings.SaveColors();
        UpdateLineRendererColors();
    }

    void OnEnable()
    {
        GhostSettings.LoadSettings();
        GhostSettings.LoadColors();
    }

    void UpdateLineRendererColors()
    {
        if (teleportLine == null || teleportGhost == null)
            return;

        SpriteRenderer ghostSprite = teleportGhost.GetComponentInChildren<SpriteRenderer>();
        if (ghostSprite == null)
            return;

        Color color = ghostSprite.color;
        color.a = 1f;

        teleportLine.startColor = color;
        teleportLine.endColor = color;
    }

}

