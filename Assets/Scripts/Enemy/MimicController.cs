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

    [Tooltip("Extra margin added to chaseStopDistance to avoid stop/start thrashing")]
    public float chaseStopBuffer = 0.25f;

    [Tooltip("Movement speed while chasing the player")]
    public float moveSpeed;

    [Tooltip("Time after awakening before the mimic becomes lethal")]
    public float lethalDelayTime;

    [Header("Physics Tuning")]
    [Tooltip("Linear drag while chasing (usually 0)")]
    public float chaseDrag = 0f;

    [Tooltip("Linear drag when not chasing to prevent sliding")]
    public float idleDrag = 8f;

    [Tooltip("Optional smoothing when setting velocity")]
    public float velocitySmoothing = 0.1f;

    [Header("Special Variant")]
    [Tooltip("If true, uses the aggro (chase) animation as the idle (no 'Coin' idle).")]
    [SerializeField] private bool aggroAsIdle = false;

    [Tooltip("If false, skip the awake animation and go straight to chase visual.")]
    [SerializeField] private bool useAwakeAnimation = true;

    [Header("Animation Names (defaults for Coin Mimic)")]
    [SerializeField] private string idleAnim = "Coin";
    [SerializeField] private string awakeAnim = "CoinMimic_Awake";
    [SerializeField] private string chaseAnim = "CoinMimic_Chase";

    [Header("References")]
    public Transform player;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;

    private bool revealed = false;
    private bool lethal = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // cached target velocity for FixedUpdate
    private Vector2 targetVelocity;
    private Vector2 currentVelocity; // used for smoothing

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

        if (rb != null)
        {
            // Prevent spin/jitter from contacts
            rb.freezeRotation = true;

            // Make visuals smoother between physics steps
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            // Start at idle drag so we don't drift before reveal
            rb.drag = idleDrag;

        }

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

        PlayIdle();
    }

    public void ResetMimicPos()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.drag = idleDrag;
        }

        revealed = false;
        lethal = false;
        transform.SetPositionAndRotation(originalPosition, originalRotation);
        
        PlayIdle();
    }

    public void RespawnEnemies()
    {
        ResetMimicPos();
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
            rb.drag = idleDrag;
        }

        revealed = false;
        lethal = false;

        PlayIdle();
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (!revealed)
        {
            if (distance < detectionRange)
            {
                Reveal();
            }
        }

        if (lethal)
        {
            // Stop only when clearly outside (buffer avoids jitter at the edge)
            if (distance > (chaseStopDistance + chaseStopBuffer))
            {
                StopChase();
            }
            else
            {
                // Compute desired velocity; applied in FixedUpdate
                Vector2 dir = (player.position - transform.position).normalized;
                targetVelocity = dir * moveSpeed;

                if (sr != null)
                {
                    sr.flipX = (player.position.x < transform.position.x);
                }
            }
        }
        else
        {
            // Not lethal means we shouldn't drift; ensure no target motion
            targetVelocity = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        // Smoothly approach target velocity to remove shakiness
        if (velocitySmoothing <= 0f)
        {
            rb.velocity = targetVelocity;
        }
        else
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, velocitySmoothing);
        }
    }

    void Reveal()
    {
        revealed = true;
        lethal = false; // reset lethal state

        if (rb != null)
        {
            // When waking up, remove idle drag so chase feels snappy
            rb.drag = chaseDrag;
            rb.velocity = Vector2.zero;
        }

        if (useAwakeAnimation)
        {
            PlayAwake();
        }
        else
        {
            PlayChase();
        }

        StartCoroutine(BecomeLethalAfterDelay());
    }

    void StopChase()
    {
        revealed = false;
        lethal = false;

        if (rb != null)
        {
            // Kill motion and add drag so we don't slide away
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.drag = idleDrag;
        }

        targetVelocity = Vector2.zero;

        PlayIdle();
    }

    // Obsolete
    /*
    void ChasePlayer()
    {
        if (rb == null) return;
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 newPos = rb.position + moveSpeed * Time.deltaTime * direction;
        rb.MovePosition(newPos);
    }*/

    IEnumerator BecomeLethalAfterDelay()
    {
        yield return new WaitForSeconds(lethalDelayTime);
        lethal = true;

        if (rb != null)
        {
            rb.drag = chaseDrag;
        }

        PlayChase();
    }

    // --- Animation helpers ---
    private void PlayIdle()
    {
        if (anim == null) return;
        anim.Play(aggroAsIdle ? chaseAnim : idleAnim);
    }

    private void PlayAwake()
    {
        if (anim == null) return;
        anim.Play(awakeAnim);
    }

    private void PlayChase()
    {
        if (anim == null) return;
        anim.Play(chaseAnim);
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseStopDistance + chaseStopBuffer);
    }
}