using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicController : MonoBehaviour
{
    [Header("Mimic Behavior Settings")]
    [Tooltip("Distance at which the mimic wakes up")]
    public float detectionRange;

    [Tooltip("Distance beyond which the mimic gives up chase")]
    public float chaseStopDistance;

    [Tooltip("Movement speed while chasing the player")]
    public float moveSpeed;

    [Tooltip("Time after awakening before the mimic becomes lethal")]
    public float lethalDelayTime;

    [Header("References")]
    public Transform player;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;
    private bool revealed = false;
    private bool lethal = false;

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
            rb.freezeRotation = true;
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
                sr.flipX = (player.position.x < transform.position.x); // Face player
            }
        }
    }


    void Reveal()
    {
        revealed = true;
        lethal = false; // reset lethal state
        anim.Play("CoinMimic_Chomp");
        StartCoroutine(BecomeLethalAfterDelay());
    }

    void StopChase()
    {
        revealed = false;
        lethal = false;
        anim.Play("Coin");
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 newPos = rb.position + moveSpeed * Time.deltaTime * direction;
        rb.MovePosition(newPos);
    }

    IEnumerator BecomeLethalAfterDelay()
    {
        yield return new WaitForSeconds(lethalDelayTime);
        lethal = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryKillPlayer(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        TryKillPlayer(collision);
    }

    void TryKillPlayer(Collision2D collision)
    { 
        if (lethal && revealed &&
            collision.collider.CompareTag("Player") && 
            collision.collider.TryGetComponent<PlayerController>(out var playerController)
            && collision.collider.TryGetComponent<PlayerPowerUps>(out var powerUps))
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






