using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI outline1;
    public TextMeshProUGUI outline2;
    public TextMeshProUGUI outline3;
    public TextMeshProUGUI outline4;
    public TextMeshProUGUI outline5;
    public TextMeshProUGUI outline6;
    public TextMeshProUGUI outline7;
    public TextMeshProUGUI outline8;
    public float updateRate = 0.5f; // How often to update the FPS (in seconds)

    private float timer;
    private int frameCount;

    void Awake()
    {
        // Hide if toggle was off
        bool showFPS = PlayerPrefs.GetInt("ShowFPS", 1) == 1;
        gameObject.SetActive(showFPS);

        Color black = Color.black;
        outline1.color = black;
        outline2.color = black;
        outline3.color = black;
        outline4.color = black;
        outline5.color = black;
        outline6.color = black;
        outline7.color = black;
        outline8.color = black;
    }

   /* void Start()
    {
        // Optional: force FPS text to have an outline in case it's reset or unset
        var outline = fpsText.fontMaterial;
        outline.EnableKeyword("OUTLINE_ON");
        outline.SetColor("_OutlineColor", Color.black);
        outline.SetFloat("_OutlineWidth", 1f);
    }*/
    void Update()
    {
        frameCount++;
        timer += Time.unscaledDeltaTime;

        if (timer >= updateRate)
        {
            float fps = frameCount / timer;
            int roundedFPS = Mathf.RoundToInt(fps);

            string displayText = $"{roundedFPS} FPS";

            fpsText.text = displayText;
            outline1.text = displayText;
            outline2.text = displayText;
            outline3.text = displayText;
            outline4.text = displayText;
            outline5.text = displayText;
            outline6.text = displayText;
            outline7.text = displayText;
            outline8.text = displayText;


            // Set color based on performance
            if (roundedFPS >= 58)
                fpsText.color = new Color(0f, 0.77f, 0f);
            else if (roundedFPS >= 45)
                fpsText.color = new Color(1f, 0.92f, 0.016f); // Yellow-ish
            else if (roundedFPS >= 30)
                fpsText.color = new Color(1f, 0.5f, 0f);       // Orange
            else
                fpsText.color = Color.red;

            frameCount = 0;
            timer = 0f;
        }
    }

}