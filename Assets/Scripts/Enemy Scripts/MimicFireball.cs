using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicFireball : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by fireball!");
            other.GetComponent<PlayerController>().Respawn();
            Destroy(gameObject);
        }

        if (other.CompareTag("ObbyCourse"))
        {
            Destroy(gameObject);
        }
    }
}

