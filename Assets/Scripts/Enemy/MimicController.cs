using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicController : MonoBehaviour
{
    public float detectionRange = 2.5f;
    public float chaseStopDistance = 6f;
    public float moveSpeed = 12f;
    private Transform player;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;
    private bool revealed = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public bool IsRevealed() => revealed;

    public void SetRevealed(bool value)
    {
        revealed = value;

        if (anim != null)
        {
            anim.Play(revealed ? "CoinMimic_Chomp" : "Coin");
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

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player not found in scene. Make sure the Player has the 'Player' tag.");
            }
        }

        anim.Play("Coin"); // Start in idle animation
    }

    public void ResetMimicPos()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        revealed = false;

        transform.SetPositionAndRotation(originalPosition, originalRotation);

        if (anim != null)
            anim.Play("Coin");
    }

    public void RespawnEnemies()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        revealed = false;

        transform.SetPositionAndRotation(originalPosition, originalRotation);

        if (anim != null)
            anim.Play("Coin");

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
            anim.Play("Coin");
    }



    void Update()
    {
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

                // Flip the sprite to face the player
                sr.flipX = (player.position.x < transform.position.x);
            }
        }
    }


    void Reveal()
    {
        revealed = true;
        anim.Play("CoinMimic_Chomp"); // Start chomp loop
    }

    void StopChase()
    {
        revealed = false;
        anim.Play("Coin"); // Go back to idle animation
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 newPos = rb.position + moveSpeed * Time.deltaTime * direction;
        rb.MovePosition(newPos);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!revealed || !collision.collider.CompareTag("Player")) return;

        if (collision.collider.TryGetComponent<PlayerPowerUps>(out var powerUps) &&
            collision.collider.TryGetComponent<PlayerController>(out var playerController))
        {
            if (powerUps.IsShieldActive() && powerUps.IsEnemyProtectionEnabled())
            {
                if (powerUps.TryUseShield())
                {
                    Debug.Log("Mimic hit absorbed by shield.");
                    return;
                }
            }

            if (!powerUps.IsInvincible())
            {
                playerController.Die();
                Debug.Log("Player caught by Mimic!");
            }
        }
    }
}




