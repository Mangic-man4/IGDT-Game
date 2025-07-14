using UnityEngine;
using UnityEngine.UI;

public class TooltipsToggle : MonoBehaviour
{
    public Toggle tooltipToggle;

    void Start()
    {
        if (tooltipToggle != null)
        {
            tooltipToggle.isOn = TooltipSettings.TooltipsEnabled;
            tooltipToggle.onValueChanged.AddListener(SetTooltipsEnabled);
        }
    }

    public void SetTooltipsEnabled(bool enabled)
    {
        TooltipSettings.TooltipsEnabled = enabled;
    }
}
