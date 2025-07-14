using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Timer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float timerValue = 0f;
    private Coroutine timerRoutine;

    void Awake()
    {
        // Destroy this component if another Timer already exists
        Timer other = FindObjectOfType<Timer>();
        if (other != null && other != this)
        {
            Debug.LogWarning("Duplicate Timer found – destroying extra instance on " + gameObject.name);
            Destroy(this);           // or Destroy(gameObject) if the whole GO is redundant
        }
    }


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
        timerRoutine = StartCoroutine(UpdateTimer());
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
        timerText.text = "Time: " + timerValue.ToString("000");

    }

    // Method to get the elapsed time
    public float GetTimeElapsed() => timerValue;


    /// <summary>Sets the timer and guarantees **only one** coroutine is running.</summary>
    public void SetTimeElapsed(float v)
    {
        timerValue = v;
        UpdateTimerDisplay();

        // restart coroutine to avoid duplicates
        if (timerRoutine != null) StopCoroutine(timerRoutine);
        timerRoutine = StartCoroutine(UpdateTimer());
    }
    public void ResetTimer() => SetTimeElapsed(0f); 
    /*{
        timerValue = 0f; UpdateTimerDisplay(); 
    }*/
}
