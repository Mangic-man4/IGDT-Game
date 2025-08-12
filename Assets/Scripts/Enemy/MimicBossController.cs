using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MimicBossController : MonoBehaviour
{
    [Header("General Settings")]
    public int maxHealth = 9;
    [SerializeField] private int currentHealth;
    [SerializeField] private int currentPhase = 1;
    private bool isInvincible = false;

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

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Explosive Drop Settings")]
    public GameObject explosiveCoinPrefab;
    [Tooltip("Spawnpoints for the Explosice Coin prefab. All spawnpoints spawn coins simultaneously")]
    public Transform[] explosiveDropPoints;
    [Tooltip("Cooldown between the attack")]
    public float explosiveDropCooldown = 4f;
    private float explosiveDropTimer = 0f;
    [Tooltip("Adds a random amount of force to the coins")]
    [SerializeField] private float randomLaunchForce = 5f;
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
    private int activeMimics = 0;

    [Header("Phase Transition")]
    public GameObject[] phaseTransitionPowerups;
    public Transform dropPosition;
    [SerializeField] private GameObject jumpDustPrefab;


    [Header("Damage Flicker")]
    public float invincibilityTime = 1f;
    public SpriteRenderer bossSprite;

    [Header("Phase Colors")]
    [SerializeField] private Color phase1Color = Color.white;
    [SerializeField] private Color phase2Color = Color.gray;
    [SerializeField] private Color phase3Color = Color.black;

    private bool canSpawnMimic = true;
    private bool canDropExplosives = true;
    private bool isNearPlayer = false;
    private bool isLunging = false;

    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer sr;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (TryGetComponent(out sr))
        {
            sr.color = phase1Color;
        }
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
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
                explosiveDropTimer = explosiveDropCooldown;
            }
        }

        if ((currentPhase == 2 || currentPhase == 3) && activeMimics < maxMimics && canSpawnMimic)
        {
            StartCoroutine(SpawnMimic());
        }

        if (isNearPlayer && canLunge && Random.value < lungeChance)
        {
            StartCoroutine(LungeAttack());
        }
    }
    private void FixedUpdate()
    {
        if (player == null) return;

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

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = Vector2.zero; // Clear any previous velocity
        rb.AddForce(direction * lungeForce, ForceMode2D.Impulse);
        Debug.Log("Boss Has Lunged!");

        yield return new WaitForSeconds(0.5f); // Pause chasing briefly after lunging
        isLunging = false;

        yield return new WaitForSeconds(lungeCooldown - 0.5f);
        canLunge = true;
    }


    IEnumerator DropExplosives()
    {
        canDropExplosives = false;

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
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            GameObject coin = Instantiate(explosiveCoinPrefab, spawnPos, rotation);

            if (coin.TryGetComponent<Rigidbody2D>(out var coinRb))
            {
                // Combine base launch direction with randomness
                Vector2 launchDir = new(baseLaunchDirection.x * directionMultiplier, baseLaunchDirection.y);
                Vector2 randomizedForce = launchDir.normalized * Random.Range(baseLaunchForce * 0.8f, baseLaunchForce * 1.2f);
                coinRb.AddForce(randomizedForce, ForceMode2D.Impulse);

                // Add spin
                coinRb.AddTorque(Random.Range(-randomTorque, randomTorque), ForceMode2D.Impulse);
            }

        }

        yield return new WaitForSeconds(explosiveDropCooldown);
        canDropExplosives = true;
    }
    private GameObject PickMimicPrefab()
    {
        if (mimicPrefabs == null || mimicPrefabs.Length == 0) return null;
        return mimicPrefabs[Random.Range(0, mimicPrefabs.Length)];
    }

    IEnumerator SpawnMimic()
    {
        canSpawnMimic = false;
        yield return new WaitForSeconds(3f);

        if (spawnPoints.Length > 0 && activeMimics < maxMimics)
        {
            bool playerOnRight = player.position.x > transform.position.x;
            int dir = playerOnRight ? 1 : -1;

            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPos = point.position;
            spawnPos.x = transform.position.x + dir * Mathf.Abs(point.localPosition.x);
            spawnPos.y = transform.position.y + point.localPosition.y;

            var prefab = PickMimicPrefab();
            if (prefab != null)
            {
                Instantiate(prefab, spawnPos, Quaternion.identity);
                activeMimics++;
            }
        }

        yield return new WaitForSeconds(3f);
        canSpawnMimic = true;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
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
            float xForce = Random.Range(-2f, 2f);
            float yForce = Random.Range(4f, 6f);
            rb.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);

            // Add the cleanup component to remove Rigidbody later
            if (!powerup.TryGetComponent<PowerupFallCleanup>(out var cleanup))
                cleanup = powerup.AddComponent<PowerupFallCleanup>();

            cleanup.SetLandingVFX(jumpDustPrefab);
        }
    }


    void Die()
    {
        // TODO: Death animation and coin explosion
        Destroy(gameObject);
    }

    public void OnMimicKilled()
    {
        activeMimics = Mathf.Max(0, activeMimics - 1);
    }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
