using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private TextMeshProUGUI newRecordText;

    private void Start()
    {
        int currentScore = PlayerPrefs.GetInt("PreviousScore", 0);
        string sceneName = PlayerPrefs.GetString("LastCompletedScene", "Unknown");
        string playerName = PlayerPrefs.GetString("PlayerName", "Guest");

        string key = $"Hiscore_{playerName}_{sceneName}";
        int best = PlayerPrefs.GetInt(key, 0);

        scoreText.text = "Score: " + currentScore;
        highscoreText.text = "Best: " + best;

        // Show "New Record!" only if this exact score set the new highscore
        if (currentScore > 0 && currentScore == best)
        {
            highscoreText.text += "\n(New Record!)";

            // If using a separate text field:
            if (newRecordText != null)
            {
                newRecordText.gameObject.SetActive(true);
                newRecordText.text = "New Record!";
            }
        }
        else
        {
            if (newRecordText != null)
                newRecordText.gameObject.SetActive(false);
        }
    }
}

