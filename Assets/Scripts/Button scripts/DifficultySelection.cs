using UnityEngine;
using UnityEngine.UI;

public class DifficultySelection : MonoBehaviour
{
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;
    private Button[] allButtons;
    public string CurrentDifficultyStatic { get; private set; } = "Normal"; // Default to "Normal"
    public Text difficultyText; // Reference to the Text UI element
    void Start()
    {
        allButtons = new Button[] { easyButton, normalButton, hardButton };
        SetSelectedDifficulty("Normal"); // Default selection or load from player prefs
    }

    public void SetSelectedDifficulty(string difficulty)
    {
        CurrentDifficultyStatic = difficulty; // Ensure the current difficulty is updated
        UpdateDifficultyDisplay(); // Update the display whenever the difficulty changes

        foreach (Button btn in allButtons)
            {
            ColorBlock cb = btn.colors;
            if (btn.name == difficulty + "Button")
            {
                cb.normalColor = Color.grey;
                cb.highlightedColor = Color.grey;
                btn.interactable = false; // Optional: make it non-interactable
            }
            else
            {
                cb.normalColor = Color.white;
                cb.highlightedColor = Color.white;
                btn.interactable = true; // Make other buttons interactable
            }
            btn.colors = cb;
        }
        Debug.Log("Difficulty set to: " + difficulty); // For debugging
    }
    void UpdateDifficultyDisplay()
    {
        if (difficultyText != null)
        {
            difficultyText.text =  CurrentDifficultyStatic;
        }
        else
        {
            Debug.LogError("DifficultyText is not assigned!");
        }
    }

}


