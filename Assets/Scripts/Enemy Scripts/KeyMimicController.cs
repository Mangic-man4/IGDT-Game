using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMimicController : MonoBehaviour
{
    public float detectionRange = 4f;
    public float chaseStopDistance = 30f;
    public float chaseSpeed = 38f;
    public float shootInterval = 2f;
    public float fireballSpeed = 6f;
    public Transform player;
    public Sprite disguisedSprite;
    public Sprite revealedSprite;
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool revealed = false;
    private float shootTimer;
    private Animator anim;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr.sprite = disguisedSprite;
        shootTimer = shootInterval;
        anim.Play("Key"); // Start in idle animation

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("Player not found! Tag the player as 'Player'.");
        }
    }

    void Update()
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

                shootTimer -= Time.deltaTime;
                if (shootTimer <= 0f)
                {
                    ShootFireball();
                    shootTimer = shootInterval;
                }

                // Rotate toward player with +180 so thin end leads
                Vector2 direction = (player.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + 180f);

                // Flip sprite horizontally if the player is on the left
                sr.flipY = (player.position.x > transform.position.x);
            }
        }
    }

    void Reveal()
    {
        if (revealed) return; // prevent double triggering
        revealed = true;

        // Set scale immediately to prevent flash
        transform.localScale = new Vector3(2f, 2f, 1f);

        // Manually reset to frame 0 before the animation starts
        anim.Play("Key"); // forces evaluation of first (idle) frame immediately at new scale

        // Now trigger the actual animation
        anim.SetTrigger("Reveal");
    }


    void StopChase()
    {
        revealed = false;

        transform.localScale = Vector3.one;         // Reset size
        anim.Play("Key");                           // Play idle animation directly
        transform.rotation = Quaternion.identity;   // Reset rotation
    }


    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 newPos = rb.position + direction * chaseSpeed * Time.deltaTime;
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
    }
}





