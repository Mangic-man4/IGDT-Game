using System.Collections;
using System.Collections.Generic;
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
    public float groundCheckRadius;
    public LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;

    [Header("Gravity Zones")]
    public float normalGravityScale = 5f;
    public float normalDrag = 0f;
    public float lowGravityScale = 0.5f;
    public float lowGravityDrag = 1f;

    [Header("Camera + Audio")]
    public Camera mainCamera;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource jumpSound;
    private PlayerPowerUps powerUps;
    private TeleportControl teleportControl;

    private float xInput;
    private bool isGrounded;
    private bool isPaused = false;
    private bool isInLowGravityZone = false;

    private readonly float coyoteTime = 0.05f;
    private float coyoteTimer;

    private Vector2 respawnPoint;

    private enum MovementState { Idle, Walk, Jump, Fall }

    // === Unity Methods ===
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        powerUps = GetComponent<PlayerPowerUps>();
        teleportControl = GetComponent<TeleportControl>();
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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) ||
                     Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);

        xInput = Input.GetAxisRaw("Horizontal");

        HandleJumpInput();
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
    }

    private void HandleJumpInput()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (isGrounded || coyoteTimer > 0f)
        {
            Jump();
            powerUps.hasUsedDoubleJump = false;
        }
        else if (powerUps.hasDoubleJump && !powerUps.hasUsedDoubleJump)
        {
            Jump();
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

    public void Die()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Contains("Easy"))
        {
            transform.position = respawnPoint;
            rb.velocity = Vector2.zero;
            Debug.Log("Player died — respawning at checkpoint.");
        }
        else
        {
            Debug.Log("Player died — reloading scene.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1f;
        }
    }


    // === Collision Events ===
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("LowGravityZone"))
        {
            isInLowGravityZone = true;
            rb.gravityScale = lowGravityScale;
            rb.drag = lowGravityDrag;
        }
        else if (other.CompareTag("Checkpoint"))
        {
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            if (checkpoint != null && !checkpoint.IsChecked)
            {
                SetRespawnPoint(other.transform.position);
                checkpoint.IsChecked = true;
                checkpoint.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("LowGravityZone"))
        {
            isInLowGravityZone = false;
            rb.gravityScale = normalGravityScale;
            rb.drag = normalDrag;
        }
    }
}