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
            /*
            // Wait for 0.01 seconds (equivalent to 1 millisecond)
            yield return new WaitForSeconds(0.01f);

            // Increase the timer value
            timerValue += 0.01f;

            */
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
        /*
        // Format the timer value as a string with minutes, seconds, and milliseconds
        string formattedTime = string.Format("{0:00}:{1:00}:{2:000}", Mathf.Floor(timerValue / 60), Mathf.Floor(timerValue % 60), (timerValue % 1) * 1000);

        */
        // Format the timer value as a string with leading zeros
        string formattedTime = timerValue.ToString("000");
        

        // Update the UI text
        timerText.text = "Time: " + formattedTime;
    }
}
/*Commented out is for seconds only. Change the timer text to Time: 000*/
