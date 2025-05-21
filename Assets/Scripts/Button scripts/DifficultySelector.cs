using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class DifficultySelector : MonoBehaviour
{
    public TextMeshProUGUI difficultyText; // Assign in the Inspector
    private int difficultyIndex = 1; // Start at Normal
    private string[] difficulties = { "Easy", "Normal", "Hard", "Extreme" };
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



