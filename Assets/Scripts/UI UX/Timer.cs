using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    private float timerValue = 0f;

    void Start()
    {
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
