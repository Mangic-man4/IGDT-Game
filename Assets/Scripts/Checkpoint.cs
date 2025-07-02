using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer sR;
    [SerializeField] private Sprite activated;

    private bool isChecked = false;

    //  Checkpoint behaviour flags 
    [Header("Checkpoint Settings")]
    [Tooltip("Re-enable in-world pickups on respawn")]
    public bool respawnRestoresPowerUps = false;

    [Tooltip("Save & restore the player’s powerup snapshot")]
    public bool respawnRestoresSavedState = false;

    private ItemCollector.CollectorSnapshot savedCollector;
    private float savedTimer;
    private int savedScore;


    //  Manual powerups to give on respawn 
    [Header("Manual Powerups to Grant on Respawn")]
    public bool giveDash;
    public bool giveSpeed;
    public bool giveDoubleJump;
    public bool giveFireball;
    public bool giveGravityFlip;
    public bool giveInfiniteSpeed;
    public bool giveInfiniteDoubleJump;
    public bool giveTeleport;

    // Stored state (only used if respawnRestoresSavedState is true)
    private PlayerPowerUps.CheckpointPowerUpState savedState;


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

        isChecked = true;
        sR.sprite = activated;

        GetComponent<SpriteRenderer>().color = Color.green;

        Debug.Log($"Checkpoint {name} snapshot saved!");
    }





    public void RestoreCheckpointState(GameObject playerObj)
    {
        var powerUps = playerObj.GetComponent<PlayerPowerUps>();

        if (respawnRestoresPowerUps)
            CheckpointManager.RespawnAllPickups();

        if (respawnRestoresSavedState)
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
        {
            CheckpointManager.RespawnAllPickups();
        }
    
        // Reapply manual power-ups after full restore
        if (giveDash) powerUps.CollectPowerUp(PowerUpType.Dash);
        if (giveSpeed) powerUps.CollectPowerUp(PowerUpType.Speed);
        if (giveDoubleJump) powerUps.CollectPowerUp(PowerUpType.DoubleJump);
        if (giveFireball) powerUps.CollectPowerUp(PowerUpType.Fireball);
        if (giveGravityFlip) powerUps.CollectPowerUp(PowerUpType.GravityFlip);
        if (giveInfiniteSpeed) powerUps.CollectPowerUp(PowerUpType.InfiniteSpeed);
        if (giveInfiniteDoubleJump) powerUps.CollectPowerUp(PowerUpType.InfiniteDoubleJump);
        if (giveTeleport) powerUps.CollectPowerUp(PowerUpType.Teleport);
    }
    public bool RespawnsSavedState()
    {
        return respawnRestoresSavedState;
    }

}
