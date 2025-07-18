using UnityEngine;
using UnityEngine.UI;

public class FPSCounterToggleUI : MonoBehaviour
{
    public Toggle fpsToggle;
    public GameObject fpsDisplay; 

    private void Start()
    {
        bool showFPS = PlayerPrefs.GetInt("ShowFPS", 1) == 1;
        fpsToggle.isOn = showFPS;

        if (fpsDisplay != null)
        {
            fpsDisplay.SetActive(showFPS);
        }

        fpsToggle.onValueChanged.AddListener(SetFPSCounterActive);
    }

    private void SetFPSCounterActive(bool enabled)
    {
        PlayerPrefs.SetInt("ShowFPS", enabled ? 1 : 0);
        PlayerPrefs.Save();

        if (fpsDisplay != null)
        {
            fpsDisplay.SetActive(enabled);
        }

    }
}
