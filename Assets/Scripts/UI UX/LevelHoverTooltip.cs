using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int level;

    [Header("Shared References")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public TextMeshProUGUI difficultyText;
    public Vector3 offset = new(150, -50, 0);

    [Header("Level Preview")]
    public Image previewImageObject;

    [System.Serializable]
    public class DifficultyPreview
    {
        public string difficultyName; // e.g., "Easy", "Normal", "Hard", "Extreme"
        public Sprite previewSprite;
    }

    public List<DifficultyPreview> previewSpritesByDifficulty = new();

    private Dictionary<string, Sprite> difficultySpriteMap;

    private void Awake()
    {
        difficultySpriteMap = new Dictionary<string, Sprite>();
        foreach (var item in previewSpritesByDifficulty)
        {
            if (!difficultySpriteMap.ContainsKey(item.difficultyName))
            {
                difficultySpriteMap.Add(item.difficultyName, item.previewSprite);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel == null || tooltipText == null || difficultyText == null) return;

        string difficulty = difficultyText.text.Trim();
        string sceneName = $"Level {level} {difficulty}";

        // Hiscore lookup
        string playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        int playerScore = PlayerPrefs.GetInt($"Hiscore_{playerName}_{sceneName}", 0);
        int globalScore = PlayerPrefs.GetInt($"GlobalHiscore_{sceneName}", 0);
        string globalPlayer = PlayerPrefs.GetString($"GlobalHiscoreName_{sceneName}", "None");

        tooltipText.text =
            $"<u>Level {level} - {difficulty}</u>\n" +
            $"<color=#00ffff>You: {playerScore}</color>\n" +
            $"<color=#ffd700>Best: {globalScore} ({globalPlayer})</color>";

        tooltipPanel.SetActive(true);
        tooltipPanel.transform.position = Input.mousePosition + offset;

        // Set preview image by difficulty
        if (previewImageObject != null)
        {
            if (difficultySpriteMap.TryGetValue(difficulty, out Sprite sprite) && sprite != null)
            {
                previewImageObject.sprite = sprite;
                previewImageObject.gameObject.SetActive(true);
            }
            else
            {
                previewImageObject.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        if (previewImageObject != null)
            previewImageObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            tooltipPanel.transform.position = Input.mousePosition + offset;
        }
    }
}
