using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MimicFireball : MonoBehaviour
{
    private KeyMimicController owner;

    public void SetOwner(KeyMimicController mimic)
    {
        owner = mimic;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by fireball!");
            other.GetComponent<PlayerController>().Die();

            if (owner != null && SceneManager.GetActiveScene().name.Contains("Easy"))
            {
                owner.ForceDeaggro();
            }

            Destroy(gameObject);
        }

        if (other.CompareTag("ObbyCourse"))
        {
            Destroy(gameObject);
        }
    }
}

