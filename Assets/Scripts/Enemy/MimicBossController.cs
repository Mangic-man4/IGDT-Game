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

    [Header("Phase Thresholds")]
    public int phase2Threshold = 6;
    public int phase3Threshold = 3;

    [Header("Attack Settings")]
    public float lungeForce = 10f;
    public float lungeCooldown = 2f;
    private bool canLunge = true;

    [Header("Explosive Drop Settings")]
    public GameObject explosiveCoinPrefab;
    public Transform[] explosiveDropPoints;
    public float explosiveDropCooldown = 4f;
    private float explosiveDropTimer = 0f;

    [Header("Mimic Spawn Settings")]
    public GameObject mimicPrefab;
    public Transform[] spawnPoints;
    public int maxMimics = 3;
    private int activeMimics = 0;

    [Header("Phase Transition")]
    public GameObject[] phaseTransitionPowerups;
    public Transform dropPosition;

    [Header("Damage Flicker")]
    public float invincibilityTime = 1f;
    public SpriteRenderer bossSprite;

    private bool canSpawnMimic = true;
    private bool canDropExplosives = true;

    private Rigidbody2D rb;
    private Transform player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        HandlePhaseTransitions();

        if (canLunge)
        {
            StartCoroutine(LungeAttack());
        }

        if ((currentPhase == 1 || currentPhase == 3) && explosiveCoinPrefab != null)
        {
            explosiveDropTimer -= Time.deltaTime;

            if (explosiveDropTimer <= 0f)
            {
                DropExplosives();
                explosiveDropTimer = explosiveDropCooldown;
            }
        }


        if ((currentPhase == 2 || currentPhase == 3) && activeMimics < maxMimics && canSpawnMimic)
        {
            StartCoroutine(SpawnMimic());
        }

    }

    private void HandlePhaseTransitions()
    {
        float healthPercent = (currentHealth / maxHealth) * 100f;

        if (currentPhase == 1 && healthPercent <= phase2Threshold)
        {
            currentPhase = 2;
            DropPhasePowerup();
        }
        else if (currentPhase == 2 && healthPercent <= phase3Threshold)
        {
            currentPhase = 3;
            DropPhasePowerup();
        }
    }

    IEnumerator LungeAttack()
    {
        canLunge = false;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.AddForce(direction * lungeForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(lungeCooldown);
        canLunge = true;
    }

    IEnumerator DropExplosives()
    {
        canDropExplosives = false;

        yield return new WaitForSeconds(explosiveDropCooldown);

        foreach (Transform point in explosiveDropPoints)
        {
            Instantiate(explosiveCoinPrefab, point.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(explosiveDropCooldown); // cooldown before next drop
        canDropExplosives = true;
    }


    IEnumerator SpawnMimic()
    {
        canSpawnMimic = false;

        yield return new WaitForSeconds(3f); // mimic spawn delay

        if (spawnPoints.Length > 0)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(mimicPrefab, point.position, Quaternion.identity);
            activeMimics++;
        }

        yield return new WaitForSeconds(3f); // cooldown before next spawn
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
            Instantiate(obj, dropPosition.position, Quaternion.identity);
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
    }
}
