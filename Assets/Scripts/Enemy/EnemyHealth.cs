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

    // Determines hit points based on scene difficulty keyword.
    private int GetHitsByDifficulty(string sceneName) => sceneName switch
    {
        string name when name.Contains("Easy") => 1,
        string name when name.Contains("Hard") => 3,
        string name when name.Contains("Extreme") => 4,
        _ => defaultHits
    };

    // Applies damage and destroys the enemy if health reaches 0.
    public void TakeDamage(int amount)
    {
        currentHits -= amount;
        if (currentHits <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void Kill() //Kill without dealing damage.
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit enemy");

            Kill();
        }
    }



    private int maxHits; // Moved to bottom to reduce inspector confusion
}

