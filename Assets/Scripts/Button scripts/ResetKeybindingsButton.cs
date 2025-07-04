using UnityEngine;

public class ResetKeybindingsButton : MonoBehaviour
{
    [SerializeField] private ControlsMenu controlsMenu; // Drag your ControlsMenu GameObject here

    public void ResetKeybindingsToDefault()
    {
        // Reset ActionKey bindings
        PlayerPrefs.SetInt("Teleport", (int)KeyCode.F);
        PlayerPrefs.SetInt("Jump", (int)KeyCode.Space);
        PlayerPrefs.SetInt("Respawn", (int)KeyCode.R);
        PlayerPrefs.SetInt("Pause", (int)KeyCode.Escape);
        PlayerPrefs.SetInt("FireballAttack", (int)KeyCode.E);
        PlayerPrefs.SetInt("ToggleGhost", (int)KeyCode.G);

        // Reset ActionAxis bindings
        PlayerPrefs.SetInt("Horizontal_Pos", (int)KeyCode.D);
        PlayerPrefs.SetInt("Horizontal_Neg", (int)KeyCode.A);

        PlayerPrefs.Save();

        // Reload keybindings from PlayerPrefs
        KeyBindings.Instance?.LoadBindings();

        // Refresh the UI to reflect updated bindings
        if (controlsMenu != null)
        {
            controlsMenu.RefreshUI();
        }

        Debug.Log("Keybindings reset to default values.");
    }
}
