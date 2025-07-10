using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //ALT VERSION

    public int level; // Assign in Inspector (1, 2, 3...)

    [Header("Shared References")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public TextMeshProUGUI difficultyText; // Reference to current difficulty display

    public Vector3 offset = new(150, -50, 0); // Optional offset from mouse

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel == null || tooltipText == null || difficultyText == null) return;

        string difficulty = difficultyText.text; // Read current difficulty from UI
        string sceneName = $"Level {level} {difficulty}";

        // Load player-specific and global hiscore
        string playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        int playerScore = PlayerPrefs.GetInt($"Hiscore_{playerName}_{sceneName}", 0);
        int globalScore = PlayerPrefs.GetInt($"GlobalHiscore_{sceneName}", 0);
        string globalPlayer = PlayerPrefs.GetString($"GlobalHiscoreName_{sceneName}", "None");

        // Format tooltip text
        string content = $"<u>Level {level} - {difficulty}</u>\n"
                       + $"<color=#00ffff>You: {playerScore}</color>\n"
                       + $"<color=#ffd700>Best: {globalScore} ({globalPlayer})</color>";

        tooltipText.text = content;

        tooltipPanel.SetActive(true);
        tooltipPanel.transform.position = Input.mousePosition + offset;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            tooltipPanel.transform.position = Input.mousePosition + offset;
        }
    }
}

