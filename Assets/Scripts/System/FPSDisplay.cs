using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI[] outlines;
    public float updateRate = 0.5f; // How often to update the FPS (in seconds)

    private float timer;
    private int frameCount;

    public static GameObject InstanceObject { get; private set; }

    void OnEnable()
    {
        if (InstanceObject == null)
        {
            InstanceObject = gameObject;
        }
    }

    void Awake()
    {
        InstanceObject = gameObject;

        // Hide if toggle was off
        bool showFPS = PlayerPrefs.GetInt("ShowFPS", 1) == 1;
        SetChildrenActive(showFPS);

        foreach (var outline in outlines)
        {
            if (outline != null)
                outline.color = Color.black;
        }
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
            string displayText = Mathf.RoundToInt(fps) + " FPS";

            fpsText.text = displayText;
            foreach (var outline in outlines)
            {
                outline.text = displayText;
            }

            if (fps >= 58)
                fpsText.color = new Color(0f, 0.77f, 0f);
            else if (fps >= 45)
                fpsText.color = new Color(1f, 0.92f, 0.016f);
            else if (fps >= 30)
                fpsText.color = new Color(1f, 0.5f, 0f);
            else
                fpsText.color = Color.red;

            frameCount = 0;
            timer = 0f;
        }
    }


    private void SetChildrenActive(bool active)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }

    public static void SetVisible(bool visible)
    {
        if (InstanceObject == null)
        {
            Debug.LogWarning("FPSDisplay.InstanceObject is null!");
            return;
        }

        foreach (Transform child in InstanceObject.transform)
        {
            child.gameObject.SetActive(visible);
        }
    }
}