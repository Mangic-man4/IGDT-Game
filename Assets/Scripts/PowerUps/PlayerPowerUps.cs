using UnityEngine;
using System.Collections;

public class PlayerPowerUps : MonoBehaviour
{
    [Header("Power-up States")]
    // --- Power-up States ---
    public bool hasDash;
    public bool hasDoubleJump;
    public bool hasUsedDoubleJump;
    public bool gravityFlipped;
    public bool hasSpeed;
    public int fireballCharges;
    [SerializeField] private int shieldStacks = 0;
    public int ShieldStacks
    {
        get => shieldStacks;
        set
        {
            shieldStacks = Mathf.Clamp(value, 0, maxShieldStacks);
            UpdateShieldVisual();
        }
    }
    [Tooltip("Mainly used by the Shield, but can still be used without shields.")]
    public bool isInvincible = false;

    // --- Timers ---
    public float speedTimer;
    public float doubleJumpTimer;

    // --- Infinite Power Ups ---
    public bool hasInfiniteSpeed;
    public bool hasInfiniteDoubleJump;


    // --- Dash Settings ---
    [SerializeField] private float dashCooldown = 0.5f;
    public float dashDistance;
    private float lastDashTime;
    public bool isDashing = false;


    [Header("Other Settings")]
    // --- Dash Visual Effects ---
    public GameObject afterImagePrefab;
    public GameObject dashBurstPrefab;



    // --- Fireball Settings ---
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 0.5f;
    private float lastFireTime;

    // --- Speed Settings ---
    public float speedMultiplier = 2f;

    // --- Gravity Settings ---
    [HideInInspector] public bool previousGravityState;

    [Header("Shield Settings")]
    // --- Shield Settings ---
    [SerializeField] private SpriteRenderer bubbleVisual; // Optional visual if using child
    [SerializeField] private GameObject shieldPrefab;     // Optional prefab if not using child
    private GameObject shieldInstance;

    public bool protectFromEnemies = true;
    public bool protectFromTraps = true;
    public float invincibilityTime = 2f;
    public int maxShieldStacks = 3;
    [SerializeField] private Color[] shieldColors; // Array of colors for 1, 2, 3+ stacks
    private int lastShieldStackVisual = -1;


    // --- References ---
    private Rigidbody2D rb;
    private RigidbodySleepMode2D originalSleepMode;


    private void Awake()
    {
        if (TryGetComponent<Rigidbody2D>(out rb))
        {
            originalSleepMode = rb.sleepMode;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        previousGravityState = gravityFlipped;

    }

    private void Update()
    {
        HandleTimers();

        if (KeyBindings.GetKeyDown(ActionKey.FireballAttack) && Time.time > lastFireTime + fireCooldown)
        {
            TryFireball();
        }

        if (gravityFlipped != previousGravityState)
        {
            FlipGravity();
            previousGravityState = gravityFlipped;
        }

        // If shieldStacks was modified directly, update visuals
        if (shieldStacks != lastShieldStackVisual)
        {
            UpdateShieldVisual();
            lastShieldStackVisual = shieldStacks;
        }

        // Update shield position every frame if needed
        if (shieldInstance != null && shieldInstance.activeSelf)
        {
            shieldInstance.transform.position = transform.position;
        }
    }

    void LateUpdate()
    {
        if (shieldInstance != null && shieldInstance.activeSelf)
        {
            shieldInstance.transform.position = transform.position;
        }
    }


    public void CollectPowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Dash:
                hasDash = true;
                break;

            case PowerUpType.Fireball:
                fireballCharges += 10; // Difficulty-based adjustment could go here
                break;

            case PowerUpType.GravityFlip:
                gravityFlipped = !gravityFlipped;
                FlipGravity();
                previousGravityState = gravityFlipped;
                break;


            case PowerUpType.Speed:
                hasSpeed = true;
                speedTimer = 15f;
                break;

            case PowerUpType.DoubleJump:
                hasDoubleJump = true;
                doubleJumpTimer = 15f;
                break;

            case PowerUpType.Teleport: // Used to disable dash when teleport returns
                hasDash = false;
                break;

            case PowerUpType.InfiniteSpeed:
                hasSpeed = true;
                hasInfiniteSpeed = true;
                break;

            case PowerUpType.InfiniteDoubleJump:
                hasDoubleJump = true;
                hasInfiniteDoubleJump = true;
                break;

            case PowerUpType.Shield:
                ShieldStacks++;
                UpdateShieldVisual();
                break;

        }
    }

    private void HandleTimers()
    {
        if (hasSpeed && !hasInfiniteSpeed)
        {
            speedTimer -= Time.deltaTime;
            if (speedTimer <= 0f)
            {
                hasSpeed = false;
            }
        }

        if (hasDoubleJump && !hasInfiniteDoubleJump)
        {
            doubleJumpTimer -= Time.deltaTime;
            if (doubleJumpTimer <= 0f)
            {
                hasDoubleJump = false;
            }
        }
    }

    public void FlipGravity()
    {
        // Flip sprite
        Vector3 scale = transform.localScale;
        scale.y *= -1;
        transform.localScale = scale;

        // Flip gravity
        rb.gravityScale *= -1;
    }

    public void PerformDash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;

        int platformLayer = LayerMask.NameToLayer("Platform");
        bool wasOnMovingPlatform = transform.parent != null && transform.parent.gameObject.layer == platformLayer;

        if (wasOnMovingPlatform)
        {
            transform.SetParent(null);
            isDashing = true;
            StartCoroutine(DelayedDash());
        }
        else
        {
            ExecuteDash();
        }

        lastDashTime = Time.time;
    }

    private void ExecuteDash()
    {
        // Temporarily set to Discrete for pixel-precise dash behavior
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

        Vector2 startPos = rb.position;
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 rayOrigin = rb.position + direction * 0.5f;
        Vector2 targetPosition = rb.position + direction * dashDistance;

        Vector2 finalPos = targetPosition; // default

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, dashDistance, ~LayerMask.GetMask("Player", "Ghost", "IgnoreDash"));

        if (hit.collider != null)
        {
            string tag = hit.collider.tag;
            int platformLayer = LayerMask.NameToLayer("Platform");

            if (tag == "ObbyCourse" || tag == "Door" || hit.collider.gameObject.layer == platformLayer)
            {
                Debug.Log("Hit " + hit.collider.tag + " or Platform layer, stopping dash before wall.");
                Vector2 stopPosition = hit.point - direction * 0.05f;
                finalPos = stopPosition;
                rb.MovePosition(stopPosition);
            }
            else if (tag == "DashWall")
            {
                Debug.Log("DashWall hit, dashing through.");
                Collider2D wallCollider = hit.collider;
                wallCollider.isTrigger = true;
                finalPos = targetPosition;
                rb.MovePosition(targetPosition);
                StartCoroutine(DisableTriggerAfterDash(wallCollider));
                TryKillEnemyAtDashEndpoint(finalPos);
            }
            else if (IsEnemyTag(tag))
            {
                Debug.Log("Enemy hit during dash, dashing through.");
                Collider2D enemyCollider = hit.collider;
                enemyCollider.isTrigger = true;
                finalPos = targetPosition;
                rb.MovePosition(targetPosition);
                StartCoroutine(DisableTriggerAfterDash(enemyCollider));
                TryKillEnemyAtDashEndpoint(finalPos);
            }
        }
        else
        {
            Debug.Log("Nothing hit, dashing freely.");
            finalPos = targetPosition;
            rb.MovePosition(targetPosition);
            TryKillEnemyAtDashEndpoint(finalPos);
        }

        // FX
        if (afterImagePrefab)
        {
            GameObject ghost = Instantiate(afterImagePrefab, startPos, transform.rotation);
            ghost.transform.localScale = transform.localScale;
        }

        if (dashBurstPrefab)
        {
            GameObject burst = Instantiate(dashBurstPrefab, finalPos, Quaternion.identity);

            if (burst.TryGetComponent<ParticleSystem>(out var ps))
            {
                Destroy(burst, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(burst, 1.5f);
            }
        }
        // Restore Continuous after dash (in next frame to be safe)
        StartCoroutine(RestoreCollisionModeAfterFrames(3));
    }
    private IEnumerator RestoreCollisionModeAfterFrames(int frameDelay = 3)
    {
        for (int i = 0; i < frameDelay; i++)
        {
            yield return null;
        }

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }



    private void TryKillEnemyAtDashEndpoint(Vector2 position)
    {
        float killRadius = 0.5f; // Adjust for difficulty/sensitivity
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, killRadius);

        foreach (var col in hits)
        {
            if (col != null && IsEnemyTag(col.tag))
            {
                if (col.TryGetComponent<EnemyHealth>(out var enemy))
                {
                    enemy.Kill();
                }
                else
                {
                    Destroy(col.gameObject);
                }
            }
        }
    }


    private bool IsEnemyTag(string tag)
    {
        return tag == "Enemy" || tag == "CoinMimic" || tag == "KeyMimic";
    }

    private IEnumerator DisableTriggerAfterDash(Collider2D wallCollider)
    {
        yield return new WaitForSeconds(0.01f);
        if (wallCollider != null)
            wallCollider.isTrigger = false;
    }

    private void TryFireball()
    {
        if (fireballCharges <= 0 || fireballPrefab == null || firePoint == null)
            return;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        fireball.GetComponent<Fireball>().SetDirection(direction);

        fireballCharges--;
        lastFireTime = Time.time;

    }

    private IEnumerator DelayedDash()
    {
        yield return null; // Wait 1 frame for unparenting to take effect
        ExecuteDash();
        isDashing = false;

    }

    public void CollectShieldPowerUp()
    {
        ShieldStacks++;

        if (shieldInstance == null && shieldPrefab != null)
        {
            shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
        }

        UpdateShieldVisual();
    }

    public bool TryUseShield()
    {
        if (ShieldStacks > 0 && !isInvincible)
        {
            ShieldStacks--;
            StartCoroutine(InvincibilityCoroutine());
            UpdateShieldVisual();
            return true;
        }

        return false;
    }


    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        if (rb != null)
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        float elapsed = 0f;
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();

        // Flicker loop
        while (elapsed < invincibilityTime)
        {
            if (playerSprite != null)
            {
                playerSprite.enabled = !playerSprite.enabled;
            }

            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        if (playerSprite != null)
        {
            playerSprite.enabled = true;
        }

        isInvincible = false;

        // Restore sleep mode after invincibility ends
        if (rb != null)
            rb.sleepMode = originalSleepMode;
    }


    private void UpdateShieldVisual()
    {
        if (ShieldStacks <= 0)
        {
            if (shieldInstance != null)
                shieldInstance.SetActive(false);
            return;
        }

        if (shieldInstance == null && shieldPrefab != null)
        {
            shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
        }

        if (shieldInstance != null)
        {
            shieldInstance.SetActive(true);
            shieldInstance.transform.position = transform.position;
            shieldInstance.transform.SetParent(null);

            int colorIndex = Mathf.Clamp(ShieldStacks - 1, 0, shieldColors.Length - 1);
            if (shieldInstance.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.color = shieldColors[colorIndex];
            }
        }
    }
    public void AddShield()
    {
        ShieldStacks++;
    }


    public void AddFireballCharges(int amount)
    {
        fireballCharges += amount;
        // Optional: clamp to a max limit
        // fireballCharges = Mathf.Min(fireballCharges, maxFireballCharges);
    }



    // External accessors for trap/enemy detection
    public bool IsShieldActive() => ShieldStacks > 0;
    public bool IsTrapProtectionEnabled() => protectFromTraps;
    public bool IsEnemyProtectionEnabled() => protectFromEnemies;
    public bool IsInvincible() => isInvincible;


    [System.Serializable]
    public struct CheckpointPowerUpState
    {
        // --- Power-up States ---
        public bool hasDash;
        public bool hasDoubleJump;
        public bool hasUsedDoubleJump;
        public bool gravityFlipped;
        public bool hasSpeed;
        public int fireballCharges;
        public int ShieldStacks;

        // --- Timers ---
        public float speedTimer;
        public float doubleJumpTimer;

        // --- Infinite Power Ups ---
        public bool hasInfiniteSpeed;
        public bool hasInfiniteDoubleJump;
    }

    public CheckpointPowerUpState GetPowerUpState()
    {
        return new CheckpointPowerUpState
        {
            hasDash = this.hasDash,
            hasDoubleJump = this.hasDoubleJump,
            hasUsedDoubleJump = this.hasUsedDoubleJump,
            gravityFlipped = this.gravityFlipped,
            hasSpeed = this.hasSpeed,
            fireballCharges = this.fireballCharges,
            ShieldStacks = this.ShieldStacks,

            speedTimer = this.speedTimer,
            doubleJumpTimer = this.doubleJumpTimer,

            hasInfiniteSpeed = this.hasInfiniteSpeed,
            hasInfiniteDoubleJump = this.hasInfiniteDoubleJump
        };
    }

    public void SetPowerUpState(CheckpointPowerUpState state)
    {
        hasDash = state.hasDash;
        hasDoubleJump = state.hasDoubleJump;
        hasUsedDoubleJump = state.hasUsedDoubleJump;
        gravityFlipped = state.gravityFlipped;
        hasSpeed = state.hasSpeed;
        fireballCharges = state.fireballCharges;
        ShieldStacks = state.ShieldStacks;

        speedTimer = state.speedTimer;
        doubleJumpTimer = state.doubleJumpTimer;

        hasInfiniteSpeed = state.hasInfiniteSpeed;
        hasInfiniteDoubleJump = state.hasInfiniteDoubleJump;

        UpdateShieldVisual();

        // Optional: Reapply any state visuals like flipping gravity
        if (gravityFlipped != previousGravityState)
        {
            FlipGravity();
            previousGravityState = gravityFlipped;
        }
    }


    public void RespawnPowerUps()
    {
        // OPTIONAL: Reset in-world pickups, if needed
        Debug.Log("Would reset collectible powerups here.");
    }

    public void ClearAllPowerUps()
    {
        // Clear normal power-ups
        hasDash = false;
        hasDoubleJump = false;
        hasUsedDoubleJump = false;
        hasSpeed = false;
        gravityFlipped = false;
        fireballCharges = 0;
        ShieldStacks = 0; 
        UpdateShieldVisual();

        // Clear timers
        speedTimer = 0f;
        doubleJumpTimer = 0f;

        // Clear infinite power-ups
        hasInfiniteSpeed = false;
        hasInfiniteDoubleJump = false;

        // Restore gravity if it was flipped
        if (previousGravityState)
        {
            FlipGravity();  // restore to default
            previousGravityState = false;
        }
    }
}

