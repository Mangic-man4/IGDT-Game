using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EnemyHealth : MonoBehaviour
{
    [Tooltip("Default max hits before the enemy dies (will be overridden by scene difficulty)")]
    [SerializeField] private int defaultHits = 2;

    private int currentHits;

    private void Start()
    {
        maxHits = GetHitsByDifficulty(SceneManager.GetActiveScene().name);
        currentHits = maxHits;
    }

    /// <summary>
    /// Determines hit points based on scene difficulty keyword.
    /// </summary>
    private int GetHitsByDifficulty(string sceneName) => sceneName switch
    {
        string name when name.Contains("Easy") => 1,
        string name when name.Contains("Hard") => 3,
        string name when name.Contains("Extreme") => 4,
        _ => defaultHits
    };

    /// <summary>
    /// Applies damage and destroys the enemy if health reaches 0.
    /// </summary>
    public void TakeDamage(int amount)
    {
        currentHits -= amount;
        if (currentHits <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void Kill() //Kill without dealing damage.
    {
        Destroy(gameObject);
    }


    private int maxHits; // Moved to bottom to reduce inspector confusion
}

