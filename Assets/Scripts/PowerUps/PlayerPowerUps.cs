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

        if (Input.GetKeyDown(KeyCode.E) && Time.time > lastFireTime + fireCooldown)
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

        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 rayOrigin = rb.position + direction * 0.5f;
        Vector2 targetPosition = rb.position + direction * dashDistance;
            
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

        lastDashTime = Time.time;
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
}

