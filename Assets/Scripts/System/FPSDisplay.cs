using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public float updateRate = 0.5f; // How often to update the FPS (in seconds)

    private float timer;
    private int frameCount;

    private void Update()
    {
        frameCount++;
        timer += Time.unscaledDeltaTime;

        if (timer >= updateRate)
        {
            float fps = frameCount / timer;
            fpsText.text = $"{Mathf.RoundToInt(fps)} FPS";
            frameCount = 0;
            timer = 0f;
        }
        //Debug.Log("FPS: " + (1f / Time.unscaledDeltaTime));

    }
}