using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer sR;
    [SerializeField] private Sprite activated;

    private bool isChecked = false;

    //  Checkpoint behaviour flags 
    [Header("Checkpoint Settings")]
    [Tooltip("Re-enable powerups on respawn")]
    public bool restorePowerUps = false;

    [Tooltip("Save & restore the player’s item pickup snapshot (aka respawn with the items that you had when you activated the checkpoint, and don't respawn them)")]
    public bool restoreSavedPickupState = false;

    [Tooltip("Restore things like pushable blocks, crumbling platforms and moving platfroms")]
    public bool restoreEnvironmentalObjects = false;

    [Tooltip("WARNING: STILL BROKEN! Save & restore enemy and turret snapshots. WARNING: CAN LEAD TO SOFTLOCKS!")]
    public bool restoreEnemySnapshot = false;

    [Tooltip("Reset the position (and states) of enemies that are still alive (only mimics atm, because laser turrets don't benefit from this)")]
    public bool resetEnemies = false;

    [Tooltip("Reset the states of all enemies (respawn dead enemies)")]
    public bool respawnEnemies = false;

    private ItemCollector.CollectorSnapshot savedCollector;
    private float savedTimer;
    private int savedScore;


    //  Manual powerups to give on respawn 
    [Header("Manual Powerups to Grant on Respawn")]
    public bool giveDash;
    public bool giveGravityFlip;
    public bool giveInfiniteSpeed;
    public bool giveInfiniteDoubleJump;
    public bool giveTeleport;
    public bool giveSpeed;
    public bool giveDoubleJump;
    [SerializeField] private float speedDuration = 15f;
    [SerializeField] private float jumpDuration = 15f;
    [Tooltip("0 = none")]
    public int giveFireballCharges = 0;
    [Tooltip("0 = none")]
    public int giveShieldStacks = 0;


    // Stored state (only used if restoreSavedPickupState is true)
    private PlayerPowerUps.CheckpointPowerUpState savedState;

    [HideInInspector] public MimicType mimicType;

    // Enemy Snapshots
    [System.Serializable]
    public struct MimicSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector2 velocity;
        public MimicType type;
        public string tag;
    }

    [System.Serializable]
    public struct TurretSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    // Prefabs for respawning
    [SerializeField] private GameObject keyMimicPrefab;
    [SerializeField] private GameObject coinMimicPrefab;
    [SerializeField] private GameObject turretPrefab;

    // Saved states
    private readonly List<MimicSnapshot> savedMimics = new();
    private readonly List<TurretSnapshot> savedTurrets = new();

    private readonly Rigidbody2D rb;

    private void Start()
    {
        sR = GetComponent<SpriteRenderer>();
    }

    public bool IsChecked
    {
        get { return isChecked; }
        set { isChecked = value; }
    }

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        TryTriggerCheckpoint(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryTriggerCheckpoint(other);
    }

    private void TryTriggerCheckpoint(Collider2D other)
    {
        if (!other.CompareTag("Player") || isChecked || triggered) return;

        triggered = true; // prevent double triggers in same frame
        Debug.Log($"Checkpoint {name} triggered!");

        PlayerController player = other.GetComponent<PlayerController>();
        player.SetActiveCheckpoint(this);

        // Save power-ups
        PlayerPowerUps powerUps = player.GetComponent<PlayerPowerUps>();
        savedState = powerUps.GetPowerUpState();

        // Save coins/buttons
        var collector = FindObjectOfType<ItemCollector>();
        if (collector != null)
            savedCollector = collector.GetSnapshot();

        // Save timer
        var timer = FindObjectOfType<Timer>();
        if (timer != null)
            savedTimer = timer.GetTimeElapsed();

        // Save score
        var scoreMgr = FindObjectOfType<ScoreManager>();
        if (scoreMgr != null)
            savedScore = scoreMgr.GetScore();


        // Save only active mimics
        savedMimics.Clear();

        foreach (var mimic in FindObjectsOfType<MimicController>())
        {
            if (mimic.gameObject.activeInHierarchy)
                SaveMimicSnapshot(mimic.transform, MimicType.Coin);
        }

        foreach (var keyMimic in FindObjectsOfType<KeyMimicController>())
        {
            if (keyMimic.gameObject.activeInHierarchy)
                SaveMimicSnapshot(keyMimic.transform, MimicType.Key);
        }

        savedTurrets.Clear();
        foreach (var turret in FindObjectsOfType<LaserTurret>())
        {
            savedTurrets.Add(new TurretSnapshot
            {
                position = turret.transform.position,
                rotation = turret.transform.rotation
            });
        }

        isChecked = true;
        sR.sprite = activated;
        GetComponent<SpriteRenderer>().color = Color.green;
        Debug.Log($"Checkpoint {name} snapshot saved!");
    }

    private void SaveMimicSnapshot(Transform t, MimicType type)
    {
        savedMimics.Add(new MimicSnapshot
        {
            position = t.position,
            rotation = t.rotation,
            velocity = rb != null ? rb.velocity : Vector2.zero,
            type = type,
            tag = t.tag // <- here
        });

        Debug.Log($"Saved {type} mimic at {t.position} with tag {t.tag}");
    }

    public void RestoreCheckpointState(GameObject playerObj)
    {
        var powerUps = playerObj.GetComponent<PlayerPowerUps>();

        if (restorePowerUps)
            CheckpointManager.RespawnAllPickups();

        if (restoreSavedPickupState)
        {
            powerUps.SetPowerUpState(savedState);

            if (savedCollector.collectedCoins == null || savedCollector.pressedButtons == null)
            {
                Debug.LogWarning("Checkpoint snapshot is empty or invalid — skipping restore.");
                return;
            }

            var collector = FindObjectOfType<ItemCollector>();
            if (collector != null) collector.RestoreSnapshot(savedCollector);

            var timer = FindObjectOfType<Timer>();
            if (timer != null) timer.SetTimeElapsed(savedTimer);

            var scoreMgr = FindObjectOfType<ScoreManager>();
            if (scoreMgr != null) scoreMgr.SetScore(savedScore);
        }

        if (restoreEnvironmentalObjects)
        {
            CheckpointManager.ResetAllEnvironmentObjects();
        }

        if (resetEnemies)
        {
            CheckpointManager.ResetEnemies();
        }

        if (respawnEnemies)
        {
            CheckpointManager.RespawnEnemies();
        }

        if (restoreEnemySnapshot)
        {
            // Handle Coin Mimics
            foreach (var mimic in FindObjectsOfType<MimicController>())
            {
                var snapshot = savedMimics.FirstOrDefault(s =>
                    mimic.CompareTag(s.tag) && Vector3.Distance(s.position, mimic.transform.position) < 1.0f);

                if (snapshot.tag != null)
                {
                    mimic.RestoreFromSnapshot(snapshot.position, snapshot.rotation, snapshot.velocity);
                }
                else
                {
                    Destroy(mimic.gameObject);
                }
            }


            // Handle Key Mimics
            foreach (var keyMimic in FindObjectsOfType<KeyMimicController>())
            {
                var snapshot = savedMimics.FirstOrDefault(s =>
                    keyMimic.CompareTag(s.tag) && Vector3.Distance(s.position, keyMimic.transform.position) < 1.0f);

                if (snapshot.tag != null)
                {
                    keyMimic.RestoreFromSnapshot(snapshot.position, snapshot.rotation, snapshot.velocity);
                }
                else
                {
                    Destroy(keyMimic.gameObject);
                }
            }

            // Recreate missing snapshots (if any enemies died before checkpoint restore)
            foreach (var snapshot in savedMimics)
            {
                GameObject prefabToUse = snapshot.type == MimicType.Key ? keyMimicPrefab : coinMimicPrefab;
                bool alreadyExists = FindObjectsOfType<MonoBehaviour>()
                                     .Any(x =>
                                         (snapshot.type == MimicType.Key && x is KeyMimicController && Vector3.Distance(x.transform.position, snapshot.position) < 0.1f) ||
                                         (snapshot.type == MimicType.Coin && x is MimicController && Vector3.Distance(x.transform.position, snapshot.position) < 0.1f));

                if (!alreadyExists)
                {
                    GameObject mimic = Instantiate(prefabToUse, snapshot.position, snapshot.rotation);

                    if (snapshot.type == MimicType.Coin && mimic.TryGetComponent<MimicController>(out var mc))
                        mc.RestoreFromSnapshot(snapshot.position, snapshot.rotation, snapshot.velocity);

                    else if (snapshot.type == MimicType.Key && mimic.TryGetComponent<KeyMimicController>(out var kmc))
                        kmc.RestoreFromSnapshot(snapshot.position, snapshot.rotation, snapshot.velocity);
                }
            }
            // Destroy all existing turrets before restoring
            foreach (var turret in FindObjectsOfType<LaserTurret>())
            {
                Destroy(turret.gameObject);
            }

            // Restore turrets as usual
            foreach (var snapshot in savedTurrets)
            {
                Instantiate(turretPrefab, snapshot.position, snapshot.rotation);
            }
        }



        {
            CheckpointManager.RespawnAllPickups();
        }
    
        // Reapply manual power-ups after full restore
        if (giveDash) powerUps.CollectPowerUp(PowerUpType.Dash);
        //if (giveSpeed) powerUps.CollectPowerUp(PowerUpType.Speed);
        //if (giveDoubleJump) powerUps.CollectPowerUp(PowerUpType.DoubleJump);
        if (giveGravityFlip) powerUps.CollectPowerUp(PowerUpType.GravityFlip);
        if (giveInfiniteSpeed) powerUps.CollectPowerUp(PowerUpType.InfiniteSpeed);
        if (giveInfiniteDoubleJump) powerUps.CollectPowerUp(PowerUpType.InfiniteDoubleJump);
        if (giveTeleport) powerUps.CollectPowerUp(PowerUpType.Teleport);
        if (giveSpeed)
            powerUps.CollectPowerUp(PowerUpType.Speed, speedDuration);

        if (giveDoubleJump)
            powerUps.CollectPowerUp(PowerUpType.DoubleJump, jumpDuration);
        {

        }
        if (giveFireballCharges > 0)
        {
            powerUps.AddFireballCharges(giveFireballCharges);
        }

        if (giveShieldStacks > 0)
        {
            for (int i = 0; i < giveShieldStacks; i++)
                powerUps.AddShield();
        }
    }
    public bool RespawnsSavedState()
    {
        return restoreSavedPickupState;
    }

}
