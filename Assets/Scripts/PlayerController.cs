using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D player;
    public float characterSpeed = 5.0f;
    public float characterJump = 200f;
    public float gravityScale = 1f;
    public Camera mainCamera;
    AudioSource jumpsound;
    Vector2 cameraPos;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingGround;
    private float xAxisMovement = 0f;
    private bool isPaused = false;

    Animator animator;
    private enum MovementState { idle, walk, jump, fall }

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Rigidbody2D>();
        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position;
            jumpsound = GetComponent<AudioSource>();
        }
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
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        xAxisMovement = Input.GetAxisRaw("Horizontal");

        HandleMovementInput();
        HandleJumpInput();

        AnimationUpdate();

        // Add other non-pause related logic here...

        // Quit Game
        if (Input.GetKey("escape"))
        {
            Application.Quit();
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
        if (Input.GetKey(KeyCode.Space) && isTouchingGround)
        {
            player.velocity = new Vector2(player.velocity.x, characterJump);
            jumpsound.Play();
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
}
