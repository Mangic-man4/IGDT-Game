using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMimicController : MonoBehaviour
{
    public static List<KeyMimicController> activeMimics = new List<KeyMimicController>();

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
    public Transform player;
    public Sprite disguisedSprite;
    public Sprite revealedSprite;
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;
    private bool revealed = false;
    private float shootTimer;

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
        Vector2 newPos = rb.position + direction * chaseSpeed * Time.fixedDeltaTime;
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

        fireball.GetComponent<MimicFireball>()?.SetOwner(this);
    }

    public void ForceDeaggro()
    {
        StopChase();
    }
}








