using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D player;
    public float characterSpeed = 5.0f;
    public float characterJump = 200f;
    public Camera mainCamera;
    AudioSource jumpsound;
    Vector2 cameraPos;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingGround;
    private float xAxisMovement = 0f;
    private bool isPaused = false;

    private float coyoteTime = 0.05f; // Time the jump "counts" after not touching a platform
    private float coyoteTimer = 0f;

    private bool isInLowGravityZone = false;
    public float normalGravityScale = 5f;
    public float normalDrag = 0f;
    public float lowGravityScale = 0.5f;
    public float lowGravityDrag = 1f;

    private Vector2 respawnPoint;
    private TeleportControl teleportControl;

    [SerializeField] private LayerMask platformLayer;

    public TrapInteraction trapInteraction;


    public float CoyoteTime
    {
        get { return coyoteTime; }
        set
        {
            coyoteTime = value;
            coyoteTimer = Mathf.Min(coyoteTimer, coyoteTime);
        }
    }

    Animator animator;
    private enum MovementState { idle, walk, jump, fall }

    void Start()
    {
        respawnPoint = transform.position;
        animator = GetComponent<Animator>();
        player = GetComponent<Rigidbody2D>();
        teleportControl = GetComponent<TeleportControl>();

        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position;
            jumpsound = GetComponent<AudioSource>();
        }

        trapInteraction = FindObjectOfType<TrapInteraction>();
    }

    void Update()
    {
        if (!isPaused)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) ||
                           Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);
        xAxisMovement = Input.GetAxisRaw("Horizontal");

        HandleMovementInput();
        HandleJumpInput();
        UpdateCoyoteTimer();
        AnimationUpdate();
    }

    void UpdateCoyoteTimer()
    {
        if (isTouchingGround)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
            coyoteTimer = Mathf.Clamp(coyoteTimer, 0f, coyoteTime);
        }
    }

    void HandleMovementInput()
    {
        player.velocity = new Vector2(xAxisMovement * characterSpeed, player.velocity.y);

        if (xAxisMovement > 0f)
        {
            this.transform.localScale = new Vector2(5, 5);
        }
        else if (xAxisMovement < 0f)
        {
            this.transform.localScale = new Vector2(-5, 5);
        }
    }

    void HandleJumpInput()
    {
        if (!isPaused && Input.GetKeyDown(KeyCode.Space) && (isTouchingGround || coyoteTimer > 0))
        {
            player.velocity = new Vector2(player.velocity.x, characterJump);
            jumpsound.Play();
            coyoteTimer = 0;
        }
    }

    void AnimationUpdate()
    {
        MovementState state;

        if (xAxisMovement > 0f)
        {
            state = MovementState.walk;
        }
        else if (xAxisMovement < 0f)
        {
            state = MovementState.walk;
        }
        else
        {
            state = MovementState.idle;
        }

        if (player.velocity.y > .1f)
        {
            state = MovementState.jump;
        }
        else if (player.velocity.y < -.1f)
        {
            state = MovementState.fall;
        }

        animator.SetInteger("movement", (int)state);
    }

    public void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LowGravityZone"))
        {
            isInLowGravityZone = true;
            player.gravityScale = lowGravityScale;
            player.angularDrag = lowGravityDrag;
        }
        else if (other.CompareTag("Checkpoint"))
        {
            // Get the checkpoint script component
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            // Check if the checkpoint has not been checked yet
            if (checkpoint != null && !checkpoint.IsChecked)
            {
                // Set the respawn point using the checkpoint's position
                SetRespawnPoint(other.transform.position);
                // Set the checkpoint as checked
                checkpoint.IsChecked = true;
                checkpoint.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LowGravityZone"))
        {
            isInLowGravityZone = false;
            player.gravityScale = normalGravityScale;
            player.angularDrag = normalDrag;
        }
    }

    public void SetRespawnPoint(Vector2 point)
    {
        respawnPoint = point;
    }

    public void Respawn()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName.Contains("Easy"))
        {
            transform.position = respawnPoint;
            player.velocity = Vector2.zero;
            
        }
        else
        {
            if (trapInteraction != null)
            {
                trapInteraction.ReloadCurrentScene();
            }
            else
            {
                Debug.LogError("TrapInteraction reference is null!");
            }
        }
    }
}
