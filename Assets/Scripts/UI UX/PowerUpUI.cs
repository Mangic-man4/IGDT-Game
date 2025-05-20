using UnityEngine;
using TMPro;

public class PowerUpUI : MonoBehaviour
{
    private TextMeshProUGUI powerUpText;
    private PlayerPowerUps playerPowerUps;
    private RectTransform rectTransform;
    private GameObject uiPanel; 

    void Start()
    {
        // Find child TMP text
        powerUpText = GetComponentInChildren<TextMeshProUGUI>(true);
        rectTransform = GetComponent<RectTransform>();

        // Store reference to the visual UI panel (parent of the TMP)
        if (powerUpText != null)
        {
            Transform panelTransform = transform.Find("Panel");
            if (panelTransform != null)
                uiPanel = panelTransform.gameObject;
        }


        // Attempt to find the PlayerPowerUps script in scene
        playerPowerUps = FindObjectOfType<PlayerPowerUps>();

        if (powerUpText == null)
            Debug.LogWarning("PowerUpUI: No TextMeshProUGUI found!");

        if (playerPowerUps == null)
            Debug.LogWarning("PowerUpUI: No PlayerPowerUps script found!");

        if (uiPanel == null)
            Debug.LogWarning("PowerUpUI: No visual panel found for power-up UI!");
    }

    void LateUpdate()
    {
        // If the player script wasn't found at Start, keep checking
        if (playerPowerUps == null)
        {
            playerPowerUps = FindObjectOfType<PlayerPowerUps>();
            if (playerPowerUps == null)
                return;
        }

        if (powerUpText == null || uiPanel == null)
            return;

        // Build the display string
        string display = "";

        if (playerPowerUps.hasDash)
            display += "Dash Enabled \n";

        if (playerPowerUps.hasDoubleJump)
        {
            if (playerPowerUps.hasInfiniteDoubleJump)
                display += "Double Jump (∞) \n";
            else
                display += "Double Jump (" + Mathf.CeilToInt(playerPowerUps.doubleJumpTimer) + "s) \n";
        }

        if (playerPowerUps.hasSpeed)
        {
            if (playerPowerUps.hasInfiniteSpeed)
                display += "Speed Boost (∞) \n";
            else
                display += "Speed Boost (" + Mathf.CeilToInt(playerPowerUps.speedTimer) + "s) \n";
        }


        if (playerPowerUps.gravityFlipped)
            display += "Gravity Flipped \n";

        if (playerPowerUps.fireballCharges > 0)
            display += "Fireballs: " + playerPowerUps.fireballCharges + " \n";

        display = display.TrimEnd('\n');
        powerUpText.text = display;

        // Resize the panel based on number of lines
        if (rectTransform != null)
        {
            float height = powerUpText.preferredHeight + 12f;

            float width = 200f;
            foreach (string line in display.Split('\n'))
            {
                float approxLength = line.Length * (powerUpText.fontSize * 1.05f);
                width = Mathf.Max(width, approxLength);
            }

            rectTransform.sizeDelta = new Vector2(width, height);
        }

        // Show/hide the visual box (but keep this script running)
        bool shouldBeVisible = !string.IsNullOrEmpty(display);
        if (uiPanel.activeSelf != shouldBeVisible)
            uiPanel.SetActive(shouldBeVisible);
    }
    /*void OnDisable()
    {
        Debug.LogWarning("PowerUpUI was disabled!", this);
    }*/

}
