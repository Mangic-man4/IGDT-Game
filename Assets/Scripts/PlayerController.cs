using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class PlayerController : MonoBehaviour
{
    public float characterSpeed = 5.0f;
    public float characterJump = 2.0f;
    public float gravityScale = 1f;
    public Camera mainCamera;
    bool isGrounded = false;

    void Start()
    {

        //Lukitsee kameran pelaajan hahmoon
        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position
        }
        
    }

    
    void Update()
    {
        if ((Input.GetKey(KeyCode.A)
    }
}
