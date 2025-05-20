using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Timer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float timerValue = 0f;

    void Start()
    {
        // Auto-find timerText
        if (timerText == null)
        {
            foreach (var tmp in FindObjectsOfType<TextMeshProUGUI>(true))
            {
                if (tmp.name == "Timer") // Match the name of your UI Text element
                {
                    timerText = tmp;
                    break;
                }
            }

            if (timerText == null)
                Debug.LogWarning("timerText TMP not found in scene!");
        }

        // Start the timer coroutine
        StartCoroutine(UpdateTimer());
    }

    IEnumerator UpdateTimer()
    {
        while (true)
        {
            // Wait for one second
            yield return new WaitForSeconds(1f);

            // Increase the timer value
            timerValue++;
            

            // Update the timer display
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {

        // Format the timer value as a string with leading zeros
        string formattedTime = timerValue.ToString("000");
        

        // Update the UI text
        timerText.text = "Time: " + formattedTime;
    }

    // Method to get the elapsed time
    public float GetTimeElapsed()
    {
        return timerValue;
    }
}
