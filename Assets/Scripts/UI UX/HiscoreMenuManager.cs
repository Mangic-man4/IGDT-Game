using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HiscoreMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text hiscoreListText;
    public TMP_Dropdown difficultyDropdown;

    private string[] levelNames = { "Level 0", "Level 1", "Level 2", "Level 3" }; // Add your level names
    private string[] difficulties = { "Easy", "Normal", "Hard", "Extreme" };


    private void Start()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        titleText.text = $"Hiscores for {playerName}";

        // Setup dropdown
        difficultyDropdown.ClearOptions();
        difficultyDropdown.AddOptions(new System.Collections.Generic.List<string>(difficulties));
        difficultyDropdown.onValueChanged.AddListener(UpdateHiscoreList);

        // Initial load
        UpdateHiscoreList(difficultyDropdown.value);
    }

    public void UpdateHiscoreList(int difficultyIndex)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        string difficulty = difficulties[difficultyIndex];

        string display = "";
        foreach (string level in levelNames)
        {
            string sceneName = $"{level} {difficulty}";
            string key = $"Hiscore_{playerName}_{sceneName}";
            int score = PlayerPrefs.GetInt(key, 0);

            display += $"{level}: {score}\n";
        }

        hiscoreListText.text = display;
    }
}
