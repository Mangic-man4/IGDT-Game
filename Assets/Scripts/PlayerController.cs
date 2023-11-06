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
    private float isJumping = 0f;
    Vector2 cameraPos;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingGround;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        //Lukitsee kameran pelaajan hahmoon
        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position;
        }
        
    }

    
    void Update()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isJumping = Input.GetAxis("Horizontal");

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
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(Vector2.left * Time.deltaTime * characterSpeed);
        }

        //Moves character right if "D" key is pressed
        if (Input.GetKey(KeyCode.D)) 
        {
            this.transform.Translate(Vector2.right * Time.deltaTime * characterSpeed);
        }

        //Makes character jump if Spacebar is pressed
        if (Input.GetKey(KeyCode.Space) && isTouchingGround)
        {
                player.velocity = new Vector2(player.velocity.x, characterJump);
        }
    }
}
