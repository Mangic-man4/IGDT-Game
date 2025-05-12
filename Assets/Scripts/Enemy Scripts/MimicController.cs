using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicController : MonoBehaviour
{
    public float detectionRange = 2.5f;
    public float chaseStopDistance = 20f;
    public float moveSpeed = 4f;
    public Sprite disguisedSprite;  // coin sprite
    public Sprite revealedSprite;   // mimic monster sprite
    public Transform player;
    public LayerMask wallLayer;     // Assign this to the Obby Course layer in Inspector

    private SpriteRenderer sr;
    private bool revealed = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = disguisedSprite;
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (!revealed)
        {
            if (distance < detectionRange)
            {
                Reveal();
            }
        }
        else
        {
            if (distance > chaseStopDistance)
            {
                StopChase();
            }
            else
            {
                // Check if wall is between mimic and player
                Vector2 direction = (player.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, wallLayer);

                if (hit.collider == null)
                {
                    ChasePlayer();
                }
            }
        }
    }

    void Reveal()
    {
        revealed = true;
        sr.sprite = revealedSprite;
    }

    void StopChase()
    {
        revealed = false;
        sr.sprite = disguisedSprite;
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (revealed && other.CompareTag("Player"))
        {
            Debug.Log("Player caught by Mimic!");
            other.GetComponent<PlayerController>().Respawn();
        }
    }
}


