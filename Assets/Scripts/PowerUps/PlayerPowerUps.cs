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
    public bool hasUsedDoubleJump = false;


    private float baseSpeed = 5f;
    public float speedMultiplier = 2f;
    private float currentSpeed;

    private float dashCooldown = 0.5f;
    private float lastDashTime;
    [SerializeField] private float dashDistance = 3f;
    private Rigidbody2D rb;

    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint; // Point in front of player

    private float fireCooldown = 0.5f;
    private float lastFireTime;





    private void Start()
    {
        currentSpeed = baseSpeed;

        rb = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        HandleTimers();

        if (Input.GetKeyDown(KeyCode.E) && Time.time > lastFireTime + fireCooldown)
        {
            TryFireball();
        }
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
                //currentSpeed = baseSpeed * speedMultiplier;
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

        Vector2 dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 rayOrigin = rb. position + dashDirection * 0.5f;
        Vector2 targetPosition = rb.position + dashDirection * dashDistance;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, dashDirection, dashDistance, ~LayerMask.GetMask("Player"));


        if (hit.collider != null)
        {
            string tag = hit.collider.tag;

            if (tag == "ObbyCourse" || tag == "Door")
            {
                Debug.Log("Hit " + hit.collider.tag + ", stopping dash before wall.");

                // Blocked: dash stops right before the wall
                Vector2 stopPosition = hit.point - dashDirection * 0.05f;
                rb.MovePosition(stopPosition);
            }
            else if (tag == "DashWall")
            {
                Debug.Log("DashWall hit, dashing through.");

                // Enable trigger on DashWall collider for the duration of the dash
                Collider2D wallCollider = hit.collider;
                wallCollider.isTrigger = true;

                // Dash through the wall
                rb.MovePosition(targetPosition);

                // Disable the trigger after the dash
                StartCoroutine(DisableTriggerAfterDash(wallCollider));

            }
        }
        else
        {
            Debug.Log("Nothing hit, dashing freely.");

            // Nothing hit, dash freely
            rb.MovePosition(targetPosition);
        }

        lastDashTime = Time.time;
    }

    // Coroutine to disable the trigger after a short delay (the length of the dash)
    private IEnumerator DisableTriggerAfterDash(Collider2D wallCollider)
    {
        yield return new WaitForSeconds(0.01f); // Adjust this to the length of your dash or required time
        wallCollider.isTrigger = false;  // Disable trigger again after the dash is complete
    }

    void TryFireball()
    {
        if (fireballCharges > 0)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            fireball.GetComponent<Fireball>().SetDirection(direction);

            fireballCharges--;
            lastFireTime = Time.time;
        }
    }
}
