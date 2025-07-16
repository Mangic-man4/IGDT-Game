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

        if (anim != null)
        {
            anim.Play(revealed ? "KeyMimic_Aggro" : "Key");
        }
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
        anim.Play("Key");

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("Player not found! Tag the player as 'Player'.");
        }

        activeMimics.Add(this);
    }

    public void ResetMimicPos()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        revealed = false;

        transform.SetPositionAndRotation(originalPosition, originalRotation);

        if (anim != null)
            anim.Play("Key");
    }

    public void RespawnEnemies()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        revealed = false;

        transform.SetPositionAndRotation(originalPosition, originalRotation);

        if (anim != null)
            anim.Play("Key");

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

        if (anim != null)
            anim.Play("Key"); 
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
                sr.flipY = (player.position.x > transform.position.x);
            }
        }
    }

    void Reveal()
    {
        if (revealed) return;
        revealed = true;

        transform.localScale = revealedScale;
        anim.Play("Key");
        anim.SetTrigger("Reveal");
    }

    void StopChase()
    {
        revealed = false;
        transform.localScale = disguisedScale;
        anim.Play("Key");
        transform.rotation = Quaternion.identity;
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








