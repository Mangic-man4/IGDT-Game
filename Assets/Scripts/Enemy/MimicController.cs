using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicController : MonoBehaviour
{
    public float detectionRange = 2.5f;
    public float chaseStopDistance = 6f;
    public float moveSpeed = 12f;
    public Transform player;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;
    private bool revealed = false;

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

        anim.Play("Idle"); // Start in idle animation
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

                // ✅ Flip the sprite to face the player
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




