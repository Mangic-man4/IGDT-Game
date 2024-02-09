using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Ignore script. Broken and obsolete!

/*public class DifficultySelector : MonoBehaviour
{
    public Text difficultyText;
    private int currentDifficultyIndex = 1; // Default to Normal
    private string[] difficultyLevels = { "Easy", "Normal", "Hard" };
    private bool isUpdating = false; // Flag to prevent updates while another is in progress

    void Start()
    {
        SetDifficulty(currentDifficultyIndex); // Initialize difficulty on start
    }

    public void DecreaseDifficulty()
    {
        if (isUpdating) return; // Exit if an update is already in progress

        isUpdating = true;
        currentDifficultyIndex--;
        if (currentDifficultyIndex < 0)
        {
            currentDifficultyIndex = difficultyLevels.Length - 1; // Wrap to last index if out of bounds
        }
        SetDifficulty(currentDifficultyIndex);
        isUpdating = false; // Reset flag after update
    }

    public void IncreaseDifficulty()
    {
        if (isUpdating) return; // Exit if an update is already in progress

        isUpdating = true;
        currentDifficultyIndex++;
        if (currentDifficultyIndex >= difficultyLevels.Length)
        {
            currentDifficultyIndex = 0; // Wrap to first index if out of bounds
        }
        SetDifficulty(currentDifficultyIndex);
        isUpdating = false; // Reset flag after update
    }

    private void SetDifficulty(int index)
    {
        // Ensure the index is within valid bounds
        currentDifficultyIndex = Mathf.Clamp(index, 0, difficultyLevels.Length - 1);
        UpdateDifficultyText();
        Debug.Log($"Difficulty set to {difficultyLevels[currentDifficultyIndex]}. Current difficulty index: {currentDifficultyIndex}");
    }

    private void UpdateDifficultyText()
    {
        difficultyText.text = difficultyLevels[currentDifficultyIndex];
    }
}
*/ 
public class DifficultySelector : MonoBehaviour
{
    public Text difficultyText; // Assign in the Inspector
    private int difficultyIndex = 1; // Start at Normal
    private string[] difficulties = { "Easy", "Normal", "Hard" };
    private bool canChangeDifficulty = true; // To prevent rapid, unintended changes
    public float changeCooldown = 0.5f; // Half a second before allowing another change

    void Start()
    {
        UpdateDifficultyDisplay();
    }

    public void OnRightButtonPressed()
    {
        if (canChangeDifficulty)
        {
            ChangeDifficulty(1);
            StartCoroutine(ChangeCooldown());
        }
    }

    public void OnLeftButtonPressed()
    {
        if (canChangeDifficulty)
        {
            ChangeDifficulty(-1);
            StartCoroutine(ChangeCooldown());
        }
    }

    void ChangeDifficulty(int direction)
    {
        difficultyIndex += direction;
        if (difficultyIndex >= difficulties.Length)
        {
            difficultyIndex = 0; // Loop back to the start
        }
        else if (difficultyIndex < 0)
        {
            difficultyIndex = difficulties.Length - 1; // Loop back to the end
        }
        UpdateDifficultyDisplay();
    }

    IEnumerator ChangeCooldown()
    {
        canChangeDifficulty = false;
        yield return new WaitForSeconds(changeCooldown);
        canChangeDifficulty = true;
    }

    void UpdateDifficultyDisplay()
    {
        difficultyText.text = difficulties[difficultyIndex];
        Debug.Log("Difficulty set to: " + difficulties[difficultyIndex]); // Log for debugging
    }
}



