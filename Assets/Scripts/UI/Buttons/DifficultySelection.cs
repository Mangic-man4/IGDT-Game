using TMPro;
using UnityEngine;

public class DifficultySelection : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;
    public TextMeshProUGUI difficultyText;

    // Store the currently selected difficulty (Apprentice/Adept/Wizard/Archmage)
    public string CurrentDifficulty { get; private set; } = "Adept"; // default

    void Start()
    {
        if (difficultyDropdown != null)
        {
            difficultyDropdown.onValueChanged.AddListener(OnDropdownChanged);

            // Optional: set default index to "Adept" (index 1 if dropdown order = Apprentice, Adept, Wizard, Archmage)
            difficultyDropdown.value = Mathf.Clamp(difficultyDropdown.value, 0, difficultyDropdown.options.Count - 1);
            difficultyDropdown.RefreshShownValue();

            // Initialize from dropdown’s current option
            string initValue = difficultyDropdown.options[difficultyDropdown.value].text;
            SetSelectedDifficulty(initValue);
        }
        else
        {
            // If no dropdown, just refresh label from current stored difficulty
            UpdateDifficultyDisplay();
        }
    }

    private void OnDropdownChanged(int index)
    {
        string selected = difficultyDropdown.options[index].text;
        SetSelectedDifficulty(selected);
    }

    public void SetSelectedDifficulty(string difficulty)
    {
        CurrentDifficulty = difficulty;
        UpdateDifficultyDisplay();
        Debug.Log($"Difficulty set to: {CurrentDifficulty}");
    }

    private void UpdateDifficultyDisplay()
    {
        if (difficultyText != null)
            difficultyText.text = CurrentDifficulty;
        else
            Debug.LogWarning("DifficultyText is not assigned!");
    }
}
