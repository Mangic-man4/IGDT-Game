using UnityEngine;
using UnityEngine.UI;

public class FPSCounterToggleUI : MonoBehaviour
{
    public Toggle fpsToggle;

    private void Start()
    {
        bool showFPS = PlayerPrefs.GetInt("ShowFPS", 1) == 1;
        fpsToggle.isOn = showFPS;

        SetFPSCounterActive(showFPS);

        fpsToggle.onValueChanged.AddListener(SetFPSCounterActive);
    }

    private void SetFPSCounterActive(bool enabled)
    {
        PlayerPrefs.SetInt("ShowFPS", enabled ? 1 : 0);
        PlayerPrefs.Save();

        FPSDisplay.SetVisible(enabled);
    }
}
