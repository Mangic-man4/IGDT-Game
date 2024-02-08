using UnityEngine;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
    public Text difficultyText;
    private int currentDifficultyIndex = 1; // 0: Easy, 1: Normal, 2: Hard
    private string[] difficultyLevels = { "Easy", "Normal", "Hard" };

    void Start()
    {
        UpdateDifficultyText();
    }

    public void DecreaseDifficulty()
    {
        currentDifficultyIndex--;
        if (currentDifficultyIndex < 0)
        {
            currentDifficultyIndex = difficultyLevels.Length - 1; // Wrap around to the last index
        }
        UpdateDifficultyText();

        // Debug log to monitor DecreaseDifficulty method calls and current difficulty index
        Debug.Log("Difficulty decreased. Current difficulty index: " + currentDifficultyIndex);
    }

    public void IncreaseDifficulty()
    {
        currentDifficultyIndex++;
        if (currentDifficultyIndex >= difficultyLevels.Length)
        {
            currentDifficultyIndex = 0; // Wrap around to the first index
        }
        UpdateDifficultyText();

        // Debug log to monitor IncreaseDifficulty method calls and current difficulty index
        Debug.Log("Difficulty increased. Current difficulty index: " + currentDifficultyIndex);
    }

    private void UpdateDifficultyText()
    {
        difficultyText.text = difficultyLevels[currentDifficultyIndex];
    }
}

//Buttons seem a bit wonky..., new bugs: both buttons decrease index,  

