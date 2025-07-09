using UnityEngine;
using UnityEngine.UI;

public class ApplyFCPColor : MonoBehaviour
{
    [Header("Assign the button image to tint")]
    public Image buttonImage;

    [Header("Color Picker Reference")]
    public FlexibleColorPicker fcp;

    [Header("Button Purpose")]
    public bool isSafeColor = true;
    private void Start()
    {
        ApplyCurrentColor(); // Initial sync
    }

    private void OnEnable()
    {
        ApplyCurrentColor(); // Initial sync
    }

    private void Update()
    {
        // Only update while the picker is active
        if (fcp != null && fcp.gameObject.activeSelf)
        {
            buttonImage.color = fcp.color;
        }
    }

    public void ApplyCurrentColor()
    {
        if (buttonImage == null) return;

        Color savedColor = isSafeColor ? GhostSettings.safeColor : GhostSettings.unsafeColor;
        buttonImage.color = savedColor;
    }
}
