using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultySelection : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;
    public TextMeshProUGUI difficultyText;

    public string CurrentDifficultyStatic { get; private set; } = "Normal";

    void Start()
    {
        if (difficultyDropdown != null)
        {
            difficultyDropdown.onValueChanged.AddListener(OnDropdownChanged);

            // Optional: Set default index
            difficultyDropdown.value = 1; // 0 = Easy, 1 = Normal, 2 = Hard, 3 = Extreme
            difficultyDropdown.RefreshShownValue();

            SetSelectedDifficulty(difficultyDropdown.options[difficultyDropdown.value].text);
        }
    }

    private void OnDropdownChanged(int index)
    {
        string selectedDifficulty = difficultyDropdown.options[index].text;
        SetSelectedDifficulty(selectedDifficulty);
    }

    public void SetSelectedDifficulty(string difficulty)
    {
        CurrentDifficultyStatic = difficulty;

        UpdateDifficultyDisplay();
        Debug.Log("Difficulty set to: " + difficulty);
    }

    private void UpdateDifficultyDisplay()
    {
        if (difficultyText != null)
        {
            difficultyText.text = CurrentDifficultyStatic;
        }
        else
        {
            Debug.LogWarning("DifficultyText is not assigned!");
        }
    }
}



