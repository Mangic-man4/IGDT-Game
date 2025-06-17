using UnityEngine;
using System.Collections;

public class PlayerPowerUps : MonoBehaviour
{
    // --- Power-up States ---
    public bool hasDash;
    public bool hasDoubleJump;
    public bool hasUsedDoubleJump;
    public bool gravityFlipped;
    public bool hasSpeed;
    public int fireballCharges;

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

    [Header("Dash Visual Effects")]
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
    private bool previousGravityState;


    // --- References ---
    private Rigidbody2D rb;

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
                if (!gravityFlipped)
                {
                    gravityFlipped = true;
                    FlipGravity();
                    previousGravityState = gravityFlipped;
                }
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

    private void FlipGravity()
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
        Vector2 startPos = rb.position;
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 rayOrigin = rb.position + direction * 0.5f;
        Vector2 targetPosition = rb.position + direction * dashDistance;
        Vector2 finalPos = targetPosition;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, dashDistance, ~LayerMask.GetMask("Player", "Ghost", "IgnoreDash"));

        if (hit.collider != null)
        {
            string tag = hit.collider.tag;

            if (tag == "ObbyCourse" || tag == "Door")
            {
                Debug.Log("Hit " + hit.collider.tag + ", stopping dash before wall.");
                Vector2 stopPosition = hit.point - direction * 0.05f;
                rb.MovePosition(stopPosition);
            }
            else if (tag == "DashWall")
            {
                Debug.Log("DashWall hit, dashing through.");
                Collider2D wallCollider = hit.collider;
                wallCollider.isTrigger = true;
                rb.MovePosition(targetPosition);
                StartCoroutine(DisableTriggerAfterDash(wallCollider));
                TryKillEnemyAtDashEndpoint(targetPosition);
            }
            else if (IsEnemyTag(tag))
            {
                Debug.Log("Enemy hit during dash, dashing through.");

                Collider2D enemyCollider = hit.collider;
                enemyCollider.isTrigger = true;
                rb.MovePosition(targetPosition);
                StartCoroutine(DisableTriggerAfterDash(enemyCollider)); // reuse same trigger disable
                TryKillEnemyAtDashEndpoint(targetPosition);
            }
        }
        else
        {
            Debug.Log("Nothing hit, dashing freely.");
            rb.MovePosition(targetPosition);
            TryKillEnemyAtDashEndpoint(targetPosition);

        }

        // ---- FX ----
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
                Destroy(burst, 1.5f); // fallback time if no particle system is found
            }
        }



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
        return tag == "Enemy" || tag == "CoinMimic";
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

        speedTimer = state.speedTimer;
        doubleJumpTimer = state.doubleJumpTimer;

        hasInfiniteSpeed = state.hasInfiniteSpeed;
        hasInfiniteDoubleJump = state.hasInfiniteDoubleJump;

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

