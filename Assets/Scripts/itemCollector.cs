using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class itemCollector : MonoBehaviour
{
    [System.Serializable]
    public class DoorAndSpikes
    {
        public GameObject door;
        public List<GameObject> spikes; // Assuming spikes are a list of GameObjects
    }

    private int coins = 0;

    [SerializeField] private Text coinCount;
    [SerializeField] private AudioSource keyCollectSound;
    [SerializeField] private AudioSource coinCollectSound;

    // Dictionary to store the relationship between buttons and doors
    private Dictionary<GameObject, DoorAndSpikes> buttonToDoorAndSpikesMap = new Dictionary<GameObject, DoorAndSpikes>();

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

/* Old code
 public class itemCollector : MonoBehaviour
{
    private int coins = 0;

    [SerializeField] private Text coinCount;
    [SerializeField] private AudioSource keyCollectSound;
    [SerializeField] private AudioSource coinCollectSound;

    // Dictionary to store the relationship between buttons and doors
    private Dictionary<GameObject, GameObject> buttonToDoorMap = new Dictionary<GameObject, GameObject>();

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

            //Play collecting sound effect
            keyCollectSound.time = 0.5f;
            keyCollectSound.Play();

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
 */