using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemCollector : MonoBehaviour
{
    private int coins = 0;

    // Dictionary to store the relationship between buttons and doors
    private Dictionary<GameObject, GameObject> buttonToDoorMap = new Dictionary<GameObject, GameObject>();

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
            // Assume each button has a corresponding door as a child
            GameObject door = collision.transform.Find("Door").gameObject;

            // Store the relationship in the dictionary
            buttonToDoorMap.Add(collision.gameObject, door);

            Destroy(collision.gameObject);
            Debug.Log("Button pressed!");

            // Open the specific door associated with this button
            OpenDoor(door);
        }
    }

    void OpenDoor(GameObject door)
    {
        // Check if the door is not null
        if (door != null)
        {
            // Destroy the door
            Destroy(door);
            Debug.Log("Door opened!");
        }
        else
        {
            Debug.LogWarning("Door not found!");
        }
    }
}
