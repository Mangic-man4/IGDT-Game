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

    // ---------------- runtime state ----------------
    private int coins = 0;

    // ---------------- references ----------------
    private TextMeshProUGUI coinCount;
    [SerializeField] private AudioSource keyCollectSound;
    [SerializeField] private AudioSource coinCollectSound;

    // button → door/spikes map so we can re-enable them on level reset
    private readonly Dictionary<GameObject, DoorAndSpikes> buttonToDoorAndSpikesMap = new();

    // ---------- coin+button lookup so we can re-enable them ----------
    private readonly List<GameObject> collectedCoins = new();
    private readonly List<GameObject> pressedButtons = new();

    // ────────────────────────────────────────────────────────────────
    #region Unity Life-Cycle
    void Start()
    {
        if (coinCount == null)
        {
            foreach (var tmp in FindObjectsOfType<TextMeshProUGUI>(true))
            {
                if (tmp.name == "Coin Count") 
                { 
                    coinCount = tmp; break; 
                }
            }

            if (coinCount == null) 
                Debug.LogWarning("coinCount TMP not found in scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ─────────── COINS ───────────
        if (collision.CompareTag("Coins"))
        {
            coinCollectSound.Play();
            CollectCoin(collision.gameObject);
            return;
        }

        // ─────────── KEYS / BUTTONS ───────────
        if (collision.CompareTag("Button"))
        {
            keyCollectSound.time = 0.5f;
            keyCollectSound.Play();
            PressButton(collision.gameObject);
        }
    }
    #endregion
    // ────────────────────────────────────────────────────────────────

    #region Coin & Button Helpers
    void CollectCoin(GameObject coin)
    {
        coins++;
        coinCount.text = $"Coins: {coins}";
        collectedCoins.Add(coin);
        coin.SetActive(false);              // disable instead of Destroy
    }

    void PressButton(GameObject button)
    {
        // find the child "Door"
        Transform doorTf = button.transform.Find("Door");
        if (doorTf == null) 
        { 
            Debug.LogError($"No child 'Door' under {button.name}"); 
            return; 
        }

        if (buttonToDoorAndSpikesMap.ContainsKey(button))
            return; // already processed this button in this life

        GameObject door = doorTf.gameObject;

        // find spikes under door
        List<GameObject> spikes = new();
        foreach (Transform t in door.transform)
            if (t.CompareTag("Spikes")) spikes.Add(t.gameObject);

        buttonToDoorAndSpikesMap.Add(button, new DoorAndSpikes { door = door, spikes = spikes });
        pressedButtons.Add(button);

        button.SetActive(false);            // disable button
        OpenDoor(door, spikes);
    }

    void OpenDoor(GameObject door, List<GameObject> spikes)
    {
        door.SetActive(false);              // disable instead of Destroy
        foreach (var s in spikes) s.SetActive(false);
    }
    #endregion
    // ────────────────────────────────────────────────────────────────

    #region Checkpoint / Level-Reset API
    // Snapshot used by Checkpoint
    public struct CollectorSnapshot
    {
        public int coins;
        public List<GameObject> collectedCoins;
        public List<GameObject> pressedButtons;
    }

    public CollectorSnapshot GetSnapshot()
    {
        return new CollectorSnapshot
        {
            coins = coins,
            collectedCoins = collectedCoins != null ? new List<GameObject>(collectedCoins) : new List<GameObject>(),
            pressedButtons = pressedButtons != null ? new List<GameObject>(pressedButtons) : new List<GameObject>()
        };
    }


    public void RestoreSnapshot(CollectorSnapshot snap)
    {
        if (snap.collectedCoins == null || snap.pressedButtons == null)
        {
            Debug.LogWarning("Checkpoint snapshot is empty or invalid — skipping restore.");
            return;
        }

        // 1) reset every coin & button to active
        foreach (var c in collectedCoins) c.SetActive(true);
        foreach (var b in pressedButtons) b.SetActive(true);
        foreach (var kv in buttonToDoorAndSpikesMap)
        {
            kv.Value.door.SetActive(true);
            kv.Value.spikes.ForEach(s => s.SetActive(true));
        }
            
        // 2) apply snapshot
        coins = snap.coins;
        coinCount.text = $"Coins: {coins}";

        buttonToDoorAndSpikesMap.Clear();

        // 3) disable coins & buttons that were already collected/pressed
        collectedCoins.Clear();
        pressedButtons.Clear();

        foreach (var c in snap.collectedCoins) 
        { 
            c.SetActive(false); collectedCoins.Add(c); 
        }
        foreach (var b in snap.pressedButtons) 
        { 
            b.SetActive(false); pressedButtons.Add(b); 
        }

        // re-open doors for pressed buttons
        foreach (var b in snap.pressedButtons)
        {
            if (buttonToDoorAndSpikesMap.TryGetValue(b, out var ds))
            {
                ds.door.SetActive(false);
                ds.spikes.ForEach(s => s.SetActive(false));
            }
        }
    }

    // Fast level reset (no checkpoint reached)
    public void ResetEverything()
    {
        // reactivate every coin & button
        foreach (var c in collectedCoins) c.SetActive(true);
        foreach (var b in pressedButtons) b.SetActive(true);
        foreach (var kv in buttonToDoorAndSpikesMap)
        {
            kv.Value.door.SetActive(true);
            kv.Value.spikes.ForEach(s => s.SetActive(true));
        }
        buttonToDoorAndSpikesMap.Clear();
        collectedCoins.Clear();
        pressedButtons.Clear();
        coins = 0;
        coinCount.text = "Coins: 0";
    }

    public int GetCoinCount() => coins;
    #endregion
}
