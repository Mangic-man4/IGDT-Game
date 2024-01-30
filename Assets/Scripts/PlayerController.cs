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
    private enum MovementState { idle, walk, jump, fall}
    
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Rigidbody2D>();
        //Lukitsee kameran pelaajan hahmoon
        //Hyppy ‰‰ni hahmolle
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

            isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            xAxisMovement = Input.GetAxisRaw("Horizontal");
            /*  //Vanha koodi
            if (isJumping > 0f) 
            {
                player.velocity = new Vector2(isJumping * characterSpeed, player.velocity.y);
            }
            else if (isJumping < 0f) 
            {
                player.velocity = new Vector2(isJumping * characterSpeed, player.velocity.y);
            }
            else 
            {
                player.velocity = new Vector2(0, player.velocity.y);
            }

            //Moves character left if "A" key is pressed
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                this.transform.Translate(Vector2.left * Time.deltaTime * characterSpeed);
                this.transform.localScale = new Vector2(-5, 5);
            
            }

            //Moves character right if "D" key is pressed
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) 
            {
                this.transform.Translate(Vector2.right * Time.deltaTime * characterSpeed);
                this.transform.localScale = new Vector2(5, 5);

            }*/

            //Uus koodi
            player.velocity = new Vector2(xAxisMovement * characterSpeed, player.velocity.y);

            if (xAxisMovement > 0f)
            {
                this.transform.localScale = new Vector2(5, 5);
            }
            else if (xAxisMovement < 0f)
            {
                this.transform.localScale = new Vector2(-5, 5);
            }

            //Makes character jump if Spacebar is pressed
            if (Input.GetKey(KeyCode.Space) && isTouchingGround)
            {
                player.velocity = new Vector2(player.velocity.x, characterJump);
                jumpsound.Play();
            }

            AnimationUpdate();

            /*Quit Game
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
            */
        }
    }
    private void AnimationUpdate()
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
