using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemCollector : MonoBehaviour
{
    private int coins = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coins"))
        {
            Destroy(collision.gameObject);
            coins++;
            Debug.Log("Coins: " + coins);
        }

        if (collision.gameObject.CompareTag("Button"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Button pressed!");

            // Find and destroy all objects with the "Door" tag
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
            foreach (GameObject door in doors)
            {
                Destroy(door);
            }
        }
    }
}
