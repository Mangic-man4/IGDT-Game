using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ItemCollector : MonoBehaviour
{
    [System.Serializable]
    public class DoorAndSpikes
    {
        public GameObject door;
        public List<GameObject> spikes; // Assuming spikes are a list of GameObjects
    }

    private int coins = 0;

    private TextMeshProUGUI coinCount;
    [SerializeField] private AudioSource keyCollectSound;
    [SerializeField] private AudioSource coinCollectSound;

    // Dictionary to store the relationship between buttons and doors
    private Dictionary<GameObject, DoorAndSpikes> buttonToDoorAndSpikesMap = new Dictionary<GameObject, DoorAndSpikes>();

    void Start()
    {
        // Try to auto-find coinCount
        if (coinCount == null)
        {
            foreach (var tmp in FindObjectsOfType<TextMeshProUGUI>(true))
            {
                if (tmp.name == "Coin Count") // Make sure this matches the name in hierarchy
                {
                    coinCount = tmp;
                    break;
                }
            }

            if (coinCount == null)
                Debug.LogWarning("coinCount TMP not found in scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coins"))
        {
            coinCollectSound.Play();
            Destroy(collision.gameObject);
            coins++;
            coinCount.text = "Coins: " + coins;

        }

        if (collision.gameObject.CompareTag("Button"))
        {
            keyCollectSound.time = 0.5f;
            keyCollectSound.Play();

            GameObject door = collision.transform.Find("Door").gameObject;
            List<GameObject> spikes = new List<GameObject>();
            // Here you need to find the associated spikes. This depends on your scene structure.
            // For example, if spikes are always children of the door, you can find them like this:
            foreach (Transform child in door.transform)
            {
                if (child.CompareTag("Spikes")) // Assuming spikes have a tag "Spike"
                {
                    spikes.Add(child.gameObject);
                }
            }

            // Store the relationship in the dictionary
            buttonToDoorAndSpikesMap.Add(collision.gameObject, new DoorAndSpikes { door = door, spikes = spikes });

            Destroy(collision.gameObject);
            Debug.Log("Button pressed!");

            // Open the specific door associated with this button
            OpenDoor(door, spikes); // Adjust the OpenDoor call to pass spikes as well

            Destroy(collision.gameObject); // Destroy the button after processing
        }

    }
    void OpenDoor(GameObject door, List<GameObject> spikes)
    {
        // Check if the door is not null
        if (door != null)
        {
            // Destroy the door
            Destroy(door);
            foreach (GameObject spike in spikes)
            {
                Destroy(spike); // Destroy each spike
            }
            Debug.Log("Door opened!");
        }
        else
        {
            Debug.LogWarning("Door not found!");
        }
    }
}