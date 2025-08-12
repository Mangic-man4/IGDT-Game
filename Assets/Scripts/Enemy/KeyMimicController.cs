using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMimicController : MonoBehaviour
{
    public static List<KeyMimicController> activeMimics = new();

    [Header("Mimic Behavior Settings")]
    [Tooltip("Distance at which the mimic activates")]
    public float detectionRange;

    [Tooltip("Distance beyond which the mimic gives up chase")]
    public float chaseStopDistance;

    [Tooltip("Movement speed while chasing the player")]
    public float chaseSpeed;

    [Tooltip("Seconds between fireball shots")]
    public float shootInterval;

    [Tooltip("Speed at which the fireball travels")]
    public float fireballSpeed;

    [Header("Scale Settings")]
    [Tooltip("Scale when revealed")]
    public Vector3 revealedScale;

    [Tooltip("Scale when disguised")]
    public Vector3 disguisedScale;

    [Header("Special Variant")]
    [Tooltip("If true, uses the aggro (chase) animation as the idle (no 'Key' idle).")]
    [SerializeField] private bool aggroAsIdle = false;

    [Tooltip("If true, use the animator trigger to play the reveal/awake sequence. If false, go straight to chase visuals.")]
    [SerializeField] private bool useRevealTrigger = true;

    [Header("Animation Names")]
    [SerializeField] private string idleAnim = "Key";
    [SerializeField] private string chaseAnim = "KeyMimic_Aggro";
    [SerializeField] private string revealTrigger = "Reveal";

    [Header("References")]
    private Transform player;
    public Sprite disguisedSprite;
    public Sprite revealedSprite;
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;

    private bool revealed = false;
    private float shootTimer;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public bool IsRevealed() => revealed;

    public void SetRevealed(bool value)
    {
        revealed = value;
        if (anim == null) return;

        if (revealed)
            PlayChase();
        else
            PlayIdle();
    }

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr.sprite = disguisedSprite;
        shootTimer = shootInterval;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("Player not found! Tag the player as 'Player'.");
        }

        PlayIdle(); // start visual idle (respects special variant)
        activeMimics.Add(this);
    }

    public void ResetMimicPos()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        revealed = false;

        transform.SetPositionAndRotation(originalPosition, originalRotation);
        //transform.localScale = disguisedScale;

        PlayIdle();
    }

    public void RespawnEnemies()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        revealed = false;

        transform.SetPositionAndRotation(originalPosition, originalRotation);
        //transform.localScale = disguisedScale;

        PlayIdle();

        gameObject.SetActive(true);
    }

    public void RestoreFromSnapshot(Vector3 pos, Quaternion rot, Vector2 velocity)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();

        transform.SetPositionAndRotation(pos, rot);

        if (rb != null)
        {
            rb.velocity = velocity;
            rb.angularVelocity = 0f;
            rb.freezeRotation = false;
        }

        revealed = false;

        PlayIdle();
    }



    void OnDestroy()
    {
        activeMimics.Remove(this);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (!revealed)
        {
            if (distance < detectionRange)
            {
                Reveal();
            }
        }
        else
        {
            if (distance > chaseStopDistance)
            {
                StopChase();
            }
            else
            {
                ChasePlayer();

                shootTimer -= Time.fixedDeltaTime;
                if (shootTimer <= 0f)
                {
                    ShootFireball();
                    shootTimer = shootInterval;
                }

                Vector2 direction = (player.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + 180f);
                if (sr != null)
                {
                    sr.flipY = (player.position.x > transform.position.x);
                }
            }
        }
    }

    void Reveal()
    {
        if (revealed) return;
        revealed = true;

        transform.localScale = revealedScale;

        if (useRevealTrigger)
        {
            // Enter base state then trigger; base respects special variant
            PlayIdle();
            if (anim != null) anim.SetTrigger(revealTrigger);
        }
        else
        {
            // Skip reveal sequence; go straight to chase visuals
            PlayChase();
        }
    }

    void StopChase()
    {
        revealed = false;
        transform.localScale = disguisedScale;
        transform.rotation = Quaternion.identity;
        PlayIdle();
    }

    // --- Animation helpers ---
    private void PlayIdle()
    {
        if (anim == null) return;
        anim.Play(aggroAsIdle ? chaseAnim : idleAnim);
    }

    private void PlayChase()
    {
        if (anim == null) return;
        anim.Play(chaseAnim);
    }

    public void ResetToIdle()
    {
        StopChase();
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 newPos = rb.position + chaseSpeed * Time.fixedDeltaTime * direction;
        rb.MovePosition(newPos);
    }

    void ShootFireball()
    {
        if (fireballPrefab == null || fireballSpawnPoint == null) return;

        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * fireballSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        fireball.transform.rotation = Quaternion.Euler(0, 0, angle + 160f);

        if (fireball.TryGetComponent<MimicFireball>(out var fb))
        {
            fb.SetOwner(this);
        }
    }

    public void ForceDeaggro()
    {
        StopChase();
    }
}