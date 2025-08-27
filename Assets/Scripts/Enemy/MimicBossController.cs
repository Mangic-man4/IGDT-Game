using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MimicBossController : MonoBehaviour
{
    [Tooltip("The Sprite Renderer and Animatior of the boss.\nShould NOT be on the root object with the Rigidbody.")]
    [SerializeField] private Transform visualRoot;

    [Header("General Settings")]
    public int maxHealth = 9;
    [HideInInspector] public int currentHealth;
    [SerializeField] private int currentPhase = 1;
    private bool isInvincible = false;

    public event Action OnDied;
    public bool IsDead { get; private set; } = false;

    [Header("Phase Thresholds Percentages")]
    public float phase2Threshold = 66f;
    public float phase3Threshold = 33f;

    [Header("Lunge Settings")]
    [Tooltip("The distance/force of the lunge attack")]
    public float lungeForce = 10f;
    [Tooltip("Cooldown between lunges")]
    public float lungeCooldown = 2f;
    private bool canLunge = true;
    [Tooltip("Chance to lunge when in range")]
    public float lungeChance = 0.2f; // Chance to lunge when in range
    [Tooltip("Distance at which it is able to lunge at the player")]
    public float attackDistance = 3f;
    private float lungeTimer = 0f;

    [Header("Lunge Feel")]

    [Tooltip("How long the burst (fast dash) actively drives movement before the glide begins.")]
    [Min(0f)] public float lungeTime = 0.15f;
    [Tooltip("How long the boss coasts after the burst while chase is paused.\nDynamic RBs: pairs with glideDrag.\nKinematic RBs: time-based decel over this duration.")]
    [Min(0f)] public float glideTime = 0.30f;
    [Tooltip("Temporary Rigidbody2D.drag used DURING the burst.\nAffects Dynamic bodies only (Kinematic ignores).\nLower = snappier burst.")]
    public float lungeDrag = 0f;
    [Tooltip("Temporary drag used DURING the glide to ease out the slide.\nDynamic bodies only.\nHigher = shorter slide, lower = longer slide.")]
    public float glideDrag = 2.5f;
    [Tooltip("Strength of the sprite-only wind-up shake (local units).\nShould be pretty low, around 0.10.")]
    [Min(0f)] public float shakeAmplitude = 0.06f;
    [Tooltip("Speed of the shake jitter (approx Hz).\nHigher = tighter, faster wobble.")]
    [Min(0f)] public float shakeFrequency = 60f;
    [Tooltip("Wind-up duration that plays the shake BEFORE any movement.\nSet > 0 for a clear attack tell.")]
    [Min(0f)] public float windupTime = 0.18f;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Explosive Drop Settings")]
    public GameObject explosiveCoinPrefab;
    [Tooltip("Spawnpoints for the Explosice Coin prefab. All spawnpoints spawn coins simultaneously")]
    public Transform[] explosiveDropPoints;
    [Tooltip("Cooldown between the attack")]
    //public float explosiveDropCooldown = 4f; // old fixed cooldown
    public Vector2 explosiveDropCooldownRange = new(2f, 4f); // new random cooldown range
    private float explosiveDropTimer = 0f;
    //[Tooltip("Adds a random amount of force to the coins")]
    //[SerializeField] private float randomLaunchForce = 5f;
    [Tooltip("Adds a random amount of torque to the coins")]
    [SerializeField] private float randomTorque = 10f;
    [Tooltip("Base speed at which the coins are launced at")]
    [SerializeField] private float baseLaunchForce = 6f;
    [Tooltip("Go higher -> increase the Y value\nFly farther horizontally -> increase the X value")]
    [SerializeField] private Vector2 baseLaunchDirection = new(1f, 1f);

    [Header("Mimic Spawn Settings")]
    [SerializeField] private GameObject[] mimicPrefabs;
    public Transform[] spawnPoints;
    public int maxMimics = 3;
    private readonly HashSet<BossMimicTracker> activeMimicSet = new();

    [Header("Phase Transition")]
    public GameObject[] phaseTransitionPowerups;
    public Transform dropPosition;
    [SerializeField] private GameObject jumpDustPrefab;

    [Header("Death Coin Drop")]
    [Tooltip("Assign the regular coin pickup prefab")]
    [SerializeField] private GameObject regularCoinPrefab;    // your normal pickup coin prefab
    [Tooltip("How many coins to spawn")]
    [SerializeField] private int deathCoinCount = 12;          // how many to spawn
    [Tooltip("Minimum shoot-out force")]
    [SerializeField] private float coinMinImpulse = 6f;        // shoot-out force range
    [Tooltip("Maximum shoot-out force")]
    [SerializeField] private float coinMaxImpulse = 10f;
    [Tooltip("Spawn offset from boss center")]
    [SerializeField] private float coinSpawnRadius = 0.2f;     // spawn offset from boss center
    [Tooltip("Gives a bit of spin")]
    [SerializeField] private float coinMaxTorque = 5f;         // little spin
    [Tooltip("Nudges directions slightly upward")]
    [SerializeField] private float coinUpBias = 0.15f;         // nudges directions slightly upward
    //[Tooltip("Avoid bouncing off corpse")]
    //[SerializeField] private float bossCoinNoCollideTime = 0.2f; // avoid bouncing off corpse

    // optional visuals
    [Tooltip("OPTIONAL")]
    [SerializeField] private GameObject coinBurstVFX;          // reuse your Explosion_FX if you want

    [Header("Damage Flicker")]
    public float invincibilityTime = 1f;
    public SpriteRenderer bossSprite;

    [Header("Phase Colors")]
    [SerializeField] private Color phase1Color = Color.white;
    [SerializeField] private Color phase2Color = Color.gray;
    [SerializeField] private Color phase3Color = Color.black;

    [Header("Boss HUD")]
    [SerializeField] private string displayName = "Mimic Boss";
    public string DisplayName => displayName;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    private BossHealthBarHUD bossBar;

    private bool canSpawnMimic = true;
    private bool canDropExplosives = true;
    private bool isNearPlayer = false;
    private bool isLunging = false;
    private bool hasDroppedOnce = false;

    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer sr;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.freezeRotation = true;
        //rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Causes weird spawn at 0,0,0
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f;
        StartCoroutine(EnableInterpolationAfterDelay(0.1f));

        if (TryGetComponent(out sr))
        {
            sr.color = phase1Color;
        }
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>(true);
        if (anim == null) anim = GetComponentInChildren<Animator>(true);

        // if you forgot to assign visualRoot, infer it from the sprite
        if (visualRoot == null && sr != null)
        {   
            // prefer the sprite's parent (e.g., your "Visual" GO); fallback to sprite itself
            visualRoot = (sr.transform.parent != null) ? sr.transform.parent : sr.transform;
        }

        currentHealth = maxHealth;
        bossBar = FindObjectOfType<BossHealthBarHUD>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        anim.Play("CoinMimic_Chase");
    }

    private void Update()
    {
        if (player == null) return;

        HandlePhaseTransitions();

        sr.flipX = (player.position.x < transform.position.x);

        if ((currentPhase == 1 || currentPhase == 3) && explosiveCoinPrefab != null && canDropExplosives)
        {
            explosiveDropTimer -= Time.deltaTime;

            if (explosiveDropTimer <= 0f)
            {
                StartCoroutine(DropExplosives());
                //explosiveDropTimer = explosiveDropCooldown; // old fixed cooldown
                explosiveDropTimer = UnityEngine.Random.Range(explosiveDropCooldownRange.x, explosiveDropCooldownRange.y); // new random cooldown range

                Debug.Log($"[Boss] Next explosive drop cooldown set to {explosiveDropTimer:F2} seconds");
            }
        }

        if ((currentPhase == 2 || currentPhase == 3) && ActiveMimicCount < maxMimics && canSpawnMimic)
        {
            StartCoroutine(SpawnMimic());
        }

        if (isNearPlayer && canLunge && UnityEngine.Random.value < lungeChance)
        {
            StartCoroutine(LungeAttack());
        }
    }
    private void FixedUpdate()
    {
        if (player == null) return;

        if (isLunging)
        {
            lungeTimer -= Time.fixedDeltaTime;
            if (lungeTimer <= 0f)
            {
                isLunging = false;
                rb.velocity = Vector2.zero;
            }

            return; // skip chasing while lunging
        }

        ChasePlayer();
    }

    private void HandlePhaseTransitions()
    {
        float healthPercent = ((float)currentHealth / maxHealth) * 100f;

        if (currentPhase == 1 && healthPercent <= phase2Threshold)
        {
            currentPhase = 2;
            DropPhasePowerup();

            if (sr != null)
            {
                sr.color = phase2Color;
            }
        }
        else if (currentPhase == 2 && healthPercent <= phase3Threshold)
        {
            currentPhase = 3;
            DropPhasePowerup();

            if (sr != null)
            {
                sr.color = phase3Color;
            }
        }
    }

    void ChasePlayer()
    {
        if (isLunging || player == null) return;

        Vector2 direction = (player.position - transform.position);
        float distance = direction.magnitude;

        if (distance > attackDistance)
        {
            direction.Normalize();
            Vector2 newPos = rb.position + moveSpeed * Time.fixedDeltaTime * direction;
            rb.MovePosition(newPos);
            isNearPlayer = false;
        }
        else
        {
            isNearPlayer = true;
        }
    }


    IEnumerator LungeAttack()
    {
        canLunge = false;
        isLunging = true;

        // lock aim at cast time
        Vector2 dir = (player.position - transform.position).normalized;

        // stop any current motion so the windup is stable
        rb.velocity = Vector2.zero;

        // 1) WINDUP: shake FIRST, no movement
        StartCoroutine(SpriteShake(windupTime, shakeAmplitude, shakeFrequency));
        yield return new WaitForSeconds(windupTime);

        // 2) LUNGE + GLIDE (handles Dynamic or Kinematic bodies)
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            float drag0 = rb.drag;

            // BURST
            rb.drag = lungeDrag;
            yield return new WaitForFixedUpdate(); // land force on physics step
            rb.AddForce(dir * lungeForce, ForceMode2D.Impulse);
            yield return new WaitForSeconds(lungeTime);

            // GLIDE
            rb.drag = glideDrag;
            yield return new WaitForSeconds(glideTime);

            // hand back control cleanly
            rb.velocity = Vector2.zero;
            rb.drag = drag0;
        }
        else // Kinematic path
        {
            // Treat lungeForce as initial speed (units/sec) for kinematic drive
            float speed = lungeForce;

            // BURST
            float t = 0f;
            while (t < lungeTime)
            {
                yield return new WaitForFixedUpdate();
                rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * dir);
                t += Time.fixedDeltaTime;
            }

            // GLIDE: decelerate linearly over glideTime
            t = 0f;
            float v = speed;
            float decel = (glideTime > 0f) ? (v / glideTime) : v;
            while (t < glideTime)
            {
                yield return new WaitForFixedUpdate();
                v = Mathf.Max(0f, v - decel * Time.fixedDeltaTime);
                rb.MovePosition(rb.position + Time.fixedDeltaTime * v * dir);
                t += Time.fixedDeltaTime;
            }
        }

        isLunging = false;

        // cooldown remainder
        float rest = Mathf.Max(0f, lungeCooldown - (windupTime + lungeTime + glideTime));
        if (rest > 0f) yield return new WaitForSeconds(rest);
        canLunge = true;
    }


    // keep this helper somewhere in your script
    IEnumerator SpriteShake(float dur, float amp, float freq)
    {
        // choose a visual transform that is not the physics root
        Transform t = visualRoot != null ? visualRoot
                      : (sr != null && sr.transform != transform ? sr.transform : null);
        if (t == null) yield break;

        Vector3 origin = t.localPosition;
        float tNow = 0f;
        while (tNow < dur)
        {
            float x = (Mathf.PerlinNoise(Time.time * freq, 0f) - 0.5f) * 2f * amp;
            float y = (Mathf.PerlinNoise(0f, Time.time * freq) - 0.5f) * 2f * amp;
            t.localPosition = origin + new Vector3(x, y, 0f);
            tNow += Time.deltaTime;
            yield return null;
        }
        t.localPosition = origin;
    }

    IEnumerator DropExplosives()
    {
        canDropExplosives = false;

        if (!hasDroppedOnce)
        {
            float startupDelay = UnityEngine.Random.Range(1f, 2f);
            yield return new WaitForSeconds(startupDelay);
            hasDroppedOnce = true;
        }

        bool playerOnRight = player.position.x > transform.position.x;
        int directionMultiplier = playerOnRight ? 1 : -1;

        foreach (Transform point in explosiveDropPoints)
        {
            // Mirror the local X offset
            Vector3 spawnOffset = point.localPosition;
            spawnOffset.x = Mathf.Abs(spawnOffset.x) * directionMultiplier;

            // Final spawn position = boss position + mirrored offset
            Vector3 spawnPos = transform.position + spawnOffset;

            // Random rotation
            Quaternion rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));

            GameObject coin = Instantiate(explosiveCoinPrefab, spawnPos, rotation);

            if (coin.TryGetComponent<Rigidbody2D>(out var coinRb))
            {
                // Combine base launch direction with randomness
                Vector2 launchDir = new(baseLaunchDirection.x * directionMultiplier, baseLaunchDirection.y);
                Vector2 randomizedForce = launchDir.normalized * UnityEngine.Random.Range(baseLaunchForce * 0.8f, baseLaunchForce * 1.2f);
                coinRb.AddForce(randomizedForce, ForceMode2D.Impulse);

                // Add spin
                coinRb.AddTorque(UnityEngine.Random.Range(-randomTorque, randomTorque), ForceMode2D.Impulse);
            }

        }
        // NOTE: Removed waiting from here because cooldown timing is already 
        // handled in Update(). Leaving it here caused the boss to wait twice (double time).
        canDropExplosives = true;
    }
    private GameObject PickMimicPrefab()
    {
        if (mimicPrefabs == null || mimicPrefabs.Length == 0) return null;
        return mimicPrefabs[UnityEngine.Random.Range(0, mimicPrefabs.Length)];
    }

    IEnumerator SpawnMimic()
    {
        canSpawnMimic = false;
        yield return new WaitForSeconds(3f);

        if (spawnPoints.Length > 0 && ActiveMimicCount < maxMimics)
        {
            bool playerOnRight = player.position.x > transform.position.x;
            int dir = playerOnRight ? 1 : -1;

            Transform point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPos = point.position;
            spawnPos.x = transform.position.x + dir * Mathf.Abs(point.localPosition.x);
            spawnPos.y = transform.position.y + point.localPosition.y;

            var prefab = PickMimicPrefab();
            if (prefab != null)
            {
                var go = Instantiate(prefab, spawnPos, Quaternion.identity);

                // face the player if your mimics use scale flipping
                var sc = go.transform.localScale;
                go.transform.localScale = new Vector3(Mathf.Abs(sc.x) * dir, sc.y, sc.z);

                // attach tracker so this mimic registers/unregisters with the boss
                if (!go.TryGetComponent<BossMimicTracker>(out var tracker)) tracker = go.AddComponent<BossMimicTracker>();
                tracker.Attach(this);

                // IMPORTANT: do NOT increment any int counter here
            }
        }

        yield return new WaitForSeconds(3f);
        canSpawnMimic = true;
    }


    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth = Mathf.Max(0, currentHealth - Mathf.Max(0, damage));
        StartCoroutine(DamageFlicker());

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    IEnumerator DamageFlicker()
    {
        isInvincible = true;

        float elapsed = 0f;

        while (elapsed < invincibilityTime)
        {
            if (bossSprite != null)
            {
                bossSprite.enabled = !bossSprite.enabled;
            }

            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        if (bossSprite != null)
        {
            bossSprite.enabled = true;
        }

        isInvincible = false;
    }

    void DropPhasePowerup()
    {
        foreach (var obj in phaseTransitionPowerups)
        {
            GameObject powerup = Instantiate(obj, dropPosition.position, Quaternion.identity);

            if (!powerup.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb = powerup.AddComponent<Rigidbody2D>();
            }

            rb.gravityScale = 1f;

            // Optional: limit falling speed
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            // Apply random launch force (tweak values as needed)
            float xForce = UnityEngine.Random.Range(-2f, 2f);
            float yForce = UnityEngine.Random.Range(4f, 6f);
            rb.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);

            // Add the cleanup component to remove Rigidbody later
            if (!powerup.TryGetComponent<PowerupFallCleanup>(out var cleanup))
                cleanup = powerup.AddComponent<PowerupFallCleanup>();

            cleanup.SetLandingVFX(jumpDustPrefab);
        }
    }


    void Die()
    {
        Debug.Log("[Boss] Died");
        IsDead = true;

        // TODO: Death animation
        DropDeathCoins();

        if (bossBar != null)
        {
            // Force fill to 0 immediately to avoid lingering visuals
            bossBar.ForceEmpty();

            // Start fading and auto-destroy
            bossBar.Hide(autoDestroy: true);
        }

        OnDied?.Invoke();
        Destroy(gameObject);
    }

    [System.Obsolete("No longer needed. BossMimicTracker auto-manages counts.")]
    public void OnMimicKilled() { /* no-op */ }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fireball"))
        {
            TakeDamage(1); // or pass the fireball damage value if it's dynamic
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Player"))
        {
            DamagePlayer(other);
        }
    }

    private void DamagePlayer (Collider2D other)
    {
        if (other.TryGetComponent<PlayerPowerUps>(out var powerUps) &&
        other.TryGetComponent<PlayerController>(out var playerController))
        {
            if (powerUps.IsShieldActive() && powerUps.IsTrapProtectionEnabled())
            {
                if (powerUps.TryUseShield())
                {
                    Debug.Log("Mimic Boss hit absorbed by shield.");
                    return;
                }
            }

            if (!powerUps.IsInvincible())
            {
                playerController.Die();
                Debug.Log("Player has died! Triggered by Mimic Boss.");
            }
        }
    }

    public int ActiveMimicCount
    {
        get { return activeMimicSet.Count; }
    }

    public void RegisterMimic(BossMimicTracker t)
    {
        if (t == null)
        {
            return;
        }

        activeMimicSet.Add(t);
    }

    public void UnregisterMimic(BossMimicTracker t)
    {
        if (t == null)
        {
            return;
        }

        activeMimicSet.Remove(t);
    }

    // Call this right before you Destroy the boss
    public void DropDeathCoins()
    {
        if (coinBurstVFX) Instantiate(coinBurstVFX, transform.position, Quaternion.identity);

        var bossCols = GetComponentsInChildren<Collider2D>(true);

        float count = Mathf.Max(1, deathCoinCount);
        float baseAngle = UnityEngine.Random.Range(0f, 360f / count);

        for (int i = 0; i < count; i++)
        {
            float angle = baseAngle + (360f * i / count) + UnityEngine.Random.Range(-10f, 10f);
            Vector2 dir = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            dir = (dir + new Vector2(0f, coinUpBias)).normalized;

            Vector3 spawnPos = transform.position + (Vector3)dir * coinSpawnRadius;

            var coin = Instantiate(regularCoinPrefab, spawnPos, Quaternion.identity);

            // physics kick
            if (!coin.TryGetComponent<Rigidbody2D>(out var crb))
                crb = coin.AddComponent<Rigidbody2D>();
            crb.velocity = Vector2.zero;
            crb.AddForce(dir * UnityEngine.Random.Range(coinMinImpulse, coinMaxImpulse), ForceMode2D.Impulse);
            crb.AddTorque(UnityEngine.Random.Range(-coinMaxTorque, coinMaxTorque), ForceMode2D.Impulse);

            // ensure cleanup + dust
            if (!coin.TryGetComponent<PowerupFallCleanup>(out var cleanup))
                cleanup = coin.AddComponent<PowerupFallCleanup>();
            if (jumpDustPrefab) cleanup.SetLandingVFX(jumpDustPrefab);

            // ignore boss colliders so they don't ping off the corpse; no need to re-enable
            var coinCols = coin.GetComponentsInChildren<Collider2D>(true);
            foreach (var bc in bossCols)
                foreach (var cc in coinCols)
                    if (bc && cc) Physics2D.IgnoreCollision(bc, cc, true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
    private IEnumerator EnableInterpolationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

}
