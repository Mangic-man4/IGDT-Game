using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerPowerUps : MonoBehaviour
{
    public bool hasDash;
    public int fireballCharges;
    public bool gravityFlipped;
    public bool hasSpeed;
    public bool hasDoubleJump;

    public float speedTimer;
    public float doubleJumpTimer;

    private float baseSpeed = 5f;
    private float speedMultiplier = 2f;
    private float currentSpeed;

    private float dashCooldown = 0.5f;
    private float lastDashTime;

    private void Start()
    {
        currentSpeed = baseSpeed;
    }

    private void Update()
    {
        HandleTimers();
    }

    public void CollectPowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Dash:
                hasDash = true;
                // TODO: Disable teleport, show phantom if needed
                break;
            case PowerUpType.Fireball:
                fireballCharges += 10; // Adjust for difficulty
                break;
            case PowerUpType.GravityFlip:
                gravityFlipped = !gravityFlipped;
                FlipGravity();
                break;
            case PowerUpType.Speed:
                hasSpeed = true;
                speedTimer = 15f;
                currentSpeed = baseSpeed * speedMultiplier;
                break;
            case PowerUpType.DoubleJump:
                hasDoubleJump = true;
                doubleJumpTimer = 15f;
                break;
            case PowerUpType.Teleport: //For disabling dash
                hasDash = false;
                break;
        }
    }

    private void HandleTimers()
    {
        if (hasSpeed)
        {
            speedTimer -= Time.deltaTime;
            if (speedTimer <= 0f)
            {
                hasSpeed = false;
                currentSpeed = baseSpeed;
            }
        }

        if (hasDoubleJump)
        {
            doubleJumpTimer -= Time.deltaTime;
            if (doubleJumpTimer <= 0f)
            {
                hasDoubleJump = false;
            }
        }
    }

    private void FlipGravity()
    {
        Vector3 scale = transform.localScale;
        scale.y *= -1;
        transform.localScale = scale;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale *= -1;
    }

    public void PerformDash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;

        Vector3 dashDirection = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        float dashDistance = 3f; // adjust as needed
        Vector3 targetPosition = transform.position + dashDirection * dashDistance;

        // Optional: Check for walls using LayerMask here if needed

        transform.position = targetPosition;
        lastDashTime = Time.time;

        // Optional: Phantom image effect goes here
    }
}
