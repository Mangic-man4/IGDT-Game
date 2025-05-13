using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    public int maxHits = 2; // Default to Normal
    private int currentHits;

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Contains("Easy"))
        {
            maxHits = 1;
        }
        else if (sceneName.Contains("Hard"))
        {
            maxHits = 3;
        }
        else if (sceneName.Contains("Extreme"))
        {
            maxHits = 4;
        }
        else
        {
            maxHits = 2; // Fallback to Normal
        }

        currentHits = maxHits;
    }

    public void TakeDamage(int amount)
    {
        currentHits -= amount;
        if (currentHits <= 0)
        {
            Destroy(gameObject);
        }
    }
}

