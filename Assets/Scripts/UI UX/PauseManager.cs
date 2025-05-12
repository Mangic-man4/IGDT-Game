using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private static PauseManager instance;

    public static PauseManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("PauseManager").AddComponent<PauseManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    private bool isPaused = false;

    public bool IsPaused
    {
        get { return isPaused; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SetPauseState(bool pauseState)
    {
        Debug.Log($"Pause state set to: {pauseState}");
        isPaused = pauseState;
        Time.timeScale = isPaused ? 0f : 1f;
    }
}
