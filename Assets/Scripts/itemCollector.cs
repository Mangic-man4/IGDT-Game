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
            // Check if the door object exists
            if (Door != null)
            {
                // Destroy the door object
                Destroy(Door);
                Debug.Log("Door disappeared!");
            }
            else
            {
                Debug.LogWarning("Door reference not set in the inspector!");

            }
        }
}
