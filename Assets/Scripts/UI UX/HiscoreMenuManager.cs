using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HiscoreMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text hiscoreListText;
    public TMP_Dropdown difficultyDropdown;

    private readonly string[] levelNames = { "Level 0", "Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6", "Level 7" }; // Add your level names
    private readonly string[] difficulties = { "Easy", "Normal", "Hard", "Extreme" };


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

            string personalKey = $"Hiscore_{playerName}_{sceneName}";
            int personalScore = PlayerPrefs.GetInt(personalKey, 0);

            string globalScoreKey = $"GlobalHiscore_{sceneName}";
            string globalNameKey = $"GlobalHiscoreName_{sceneName}";
            int globalScore = PlayerPrefs.GetInt(globalScoreKey, 0);
            string globalName = PlayerPrefs.GetString(globalNameKey, "None");

            display += $"{level}:\n";
            display += $"- You: {personalScore}\n";
            display += $"- Best: {globalScore} ({globalName})\n\n";
        }

        hiscoreListText.text = display;
    }

}
