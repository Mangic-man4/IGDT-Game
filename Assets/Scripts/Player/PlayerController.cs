using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseMoveSpeed = 10f;   
    public float jumpForce = 20f;

    [Header("Environment Layers")]
    public Transform groundCheck;
    [SerializeField] private Vector2 groundCheckBoxSize = new (0.7f, 0.2f); // New box-based groundCheck detection method
    //private readonly float groundCheckRadius; //Old circle radius-based groundCheck detection method
    public LayerMask groundLayer;
    public LayerMask testPlatforms;
    [SerializeField] private LayerMask platformLayer;

    [Header("Gravity Zones")]
    public float normalGravityScale = 5f;
    public float normalDrag = 0f;
    public float lowGravityScale = 0.5f;
    public float lowGravityDrag = 1f;

    [Header("Camera + Audio")]
    public Camera mainCamera;
    [Tooltip("The death effect you want to play at the player's death")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Checkpoint toggle")]
    [SerializeField] private bool checkpointsEnabled = false;

    [Header("Particle effects")]
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameObject spawnEffectPrefab;
    [SerializeField] private GameObject dustJumpVFX;
    [SerializeField] private GameObject doubleJumpVFX;
    //[SerializeField] private GameObject speedLines;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource jumpSound;
    private PlayerPowerUps powerUps;

    private float xInput;
    private bool isGrounded;
    private bool isPaused = false;

    private readonly float coyoteTime = 0.05f;
    private float coyoteTimer;

    private Vector2 respawnPoint;

    private Checkpoint currentCheckpoint;


    private enum MovementState { Idle, Walk, Jump, Fall }

    // === Unity Methods ===
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        powerUps = GetComponent<PlayerPowerUps>();
        jumpSound = GetComponent<AudioSource>();

        if (mainCamera) _ = mainCamera.transform.position; // Accessed once

        respawnPoint = transform.position;
    }

    private void Update()
    {
        if (isPaused) return;

        HandleInput();
        UpdateCoyoteTimer();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (isPaused) return;

        Move();
    }

    // === Input + Logic ===
    private void HandleInput()
    {
        /* // Old detection method
             isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) ||
             Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer) ||
             Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, testPlatforms);
        */

        // Nre detection method
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0f, groundLayer | platformLayer | testPlatforms);

        //xInput = Input.GetAxisRaw("Horizontal"); //old

        xInput = KeyBindings.GetAxisRaw(ActionAxis.Horizontal);


        HandleJumpInput();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckBoxSize);
    }


    private void Move()
    {
        float moveSpeed = baseMoveSpeed;

        if (powerUps.hasSpeed)
        {
            moveSpeed *= powerUps.speedMultiplier;
        }

        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);

        FlipSprite();

        // Speed Lines Control 
        // Remove because it's ugly
        /*if (powerUps.hasSpeed && Mathf.Abs(xInput) > 0.01f)
        {
            if (!speedLines.activeSelf)
                speedLines.SetActive(true);
        }
        else
        {
            if (speedLines.activeSelf)
                speedLines.SetActive(false);
        }*/
    }

    private void HandleJumpInput()
    {
        if (!KeyBindings.GetKeyDown(ActionKey.Jump)) return;

        if (isGrounded || coyoteTimer > 0f)
        {
            Jump();

            // Dust VFX for regular jump
            if (dustJumpVFX != null)
            {
                Instantiate(dustJumpVFX, transform.position + new Vector3(0f, -1f, 0f), Quaternion.identity);
            }

            powerUps.hasUsedDoubleJump = false;
        }
        else if (powerUps.hasDoubleJump && !powerUps.hasUsedDoubleJump)
        {
            Jump();

            // Instantiate double jump visual effect
            if (doubleJumpVFX != null)
            {
                Instantiate(doubleJumpVFX, transform.position + new Vector3(0f, -1f, 0f), Quaternion.identity);
            }

            powerUps.hasUsedDoubleJump = true;

        }
    }

    private void Jump()
    {
        float direction = Mathf.Sign(rb.gravityScale); // +1 or -1 depending on gravity
        rb.velocity = new Vector2(rb.velocity.x, jumpForce * direction);
        jumpSound.Play();
        coyoteTimer = 0f;
    }

    private void UpdateCoyoteTimer()
    {
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
            coyoteTimer = Mathf.Clamp(coyoteTimer, 0f, coyoteTime);
        }
    }

    private void FlipSprite()
    {
        float scale = 5f;
        if (xInput != 0)
        {
            float flipY = Mathf.Sign(transform.localScale.y);
            transform.localScale = new Vector3(Mathf.Sign(xInput) * scale, flipY * scale, 1f);
        }
    }

    private void UpdateAnimation()
    {
        MovementState state = MovementState.Idle;

        if (Mathf.Abs(xInput) > 0.1f) state = MovementState.Walk;
        if (rb.velocity.y > 0.1f) state = MovementState.Jump;
        else if (rb.velocity.y < -0.1f) state = MovementState.Fall;

        animator.SetInteger("movement", (int)state);
    }

    // === Pause ===
    public void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    // === Respawning ===   
    public void SetRespawnPoint(Vector2 point)
    {
        respawnPoint = point;
    }

    public void Respawn() //Backward-compatible respawn method. Use Die() for all new death-related logic.
    {
        Die();
    }
    public void SetActiveCheckpoint(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;
        SetRespawnPoint(checkpoint.transform.position);
        Debug.Log($"Set currentCheckpoint to: {checkpoint.name}");
    }



    public void Die()
    {
        //string sceneName = SceneManager.GetActiveScene().name;

        foreach (var effect in GameObject.FindGameObjectsWithTag("PickupEffect"))
        {
            Destroy(effect);
        }

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (!checkpointsEnabled)
        {
            StartCoroutine(DelayedReloadScene()); // Delay the reload
            return;
        }

        Checkpoint active = currentCheckpoint;

        var pwr = GetComponent<PlayerPowerUps>();
        _ = GetComponent<ItemCollector>();
        _ = FindObjectOfType<Timer>();

        if (pwr != null) pwr.ClearAllPowerUps();

        // Force all KeyMimics to deaggro
        foreach (var mimic in KeyMimicController.activeMimics)
            mimic.ResetToIdle();

        if (active == null)
        {
            Debug.Log("Easy death with NO checkpoint - hard resetting level");
            StartCoroutine(DelayedReloadScene());
            return;
        }

        Debug.Log($"Easy death WITH checkpoint - restoring snapshot from {active.name}");

        transform.position = respawnPoint;
        rb.velocity = Vector2.zero;

        active.RestoreCheckpointState(gameObject);

        if (spawnEffectPrefab != null)
        {
            Instantiate(spawnEffectPrefab, transform.position, Quaternion.identity);
        }

        PowerUpSpawner.ForceSpawnOnAll();
    }



    // === Collision Events ===
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("LowGravityZone"))
        {
            rb.gravityScale = lowGravityScale;
            rb.drag = lowGravityDrag;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("LowGravityZone"))
        {
            rb.gravityScale = lowGravityScale;
            rb.drag = lowGravityDrag;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("LowGravityZone"))
        {
            rb.gravityScale = normalGravityScale;
            rb.drag = normalDrag;
        }
    }

    public void UnparentFromPlatform()
    {
        StartCoroutine(UnparentDelayed());
    }

    private IEnumerator UnparentDelayed()
    {
        yield return null;
        transform.SetParent(null);
    }
    private IEnumerator DelayedReloadScene()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Disable movement scripts
        if (TryGetComponent<PlayerController>(out var controller)) controller.enabled = false;

        if (TryGetComponent<TeleportControl>(out var teleport)) teleport.enabled = false;

        if (TryGetComponent<PlayerPowerUps>(out var powerUps)) powerUps.enabled = false;

        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer)) spriteRenderer .enabled = false;

        foreach (var col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        // Freeze physics
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        yield return new WaitForSeconds(1.5f); // adjust delay as needed

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

}