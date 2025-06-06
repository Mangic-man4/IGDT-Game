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
        public List<GameObject> spikes;
    }

    private int coins = 0;

    private TextMeshProUGUI coinCount;
    [SerializeField] private AudioSource keyCollectSound;
    [SerializeField] private AudioSource coinCollectSound;

    private Dictionary<GameObject, DoorAndSpikes> buttonToDoorAndSpikesMap = new Dictionary<GameObject, DoorAndSpikes>();

    void Start()
    {
        if (coinCount == null)
        {
            foreach (var tmp in FindObjectsOfType<TextMeshProUGUI>(true))
            {
                if (tmp.name == "Coin Count")
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

            // 🎯 Get the child door directly
            Transform doorTransform = collision.transform.Find("Door"); // assumes door is named "Door"

            if (doorTransform == null)
            {
                Debug.LogError("No child named 'Door' found under button: " + collision.name);
                return;
            }

            GameObject door = doorTransform.gameObject;

            // 🧨 Find spikes under the door (if any)
            List<GameObject> spikes = new List<GameObject>();
            foreach (Transform child in door.transform)
            {
                if (child.CompareTag("Spikes"))
                {
                    spikes.Add(child.gameObject);
                }
            }

            buttonToDoorAndSpikesMap.Add(collision.gameObject, new DoorAndSpikes { door = door, spikes = spikes });

            Destroy(collision.gameObject); // ✅ Remove key/button after use
            Debug.Log("Key collected and door unlocked!");

            OpenDoor(door, spikes);
        }

    }

    void OpenDoor(GameObject door, List<GameObject> spikes)
    {
        if (door != null)
        {
            Destroy(door);
            foreach (GameObject spike in spikes)
            {
                Destroy(spike);
            }
            Debug.Log("Door opened!");
        }
        else
        {
            Debug.LogWarning("Door not found!");
        }
    }
}
