using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicController : MonoBehaviour
{
    public float detectionRange = 2.5f;
    public float chaseStopDistance = 6f;
    public float moveSpeed = 12f;
    public Sprite disguisedSprite;
    public Sprite revealedSprite;
    public Transform player;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool revealed = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player not found in scene. Make sure the Player has the 'Player' tag.");
            }
        }
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
                ChasePlayer();
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
        Vector2 direction = (player.transform.position - transform.position).normalized;
        Vector2 newPos = rb.position + direction * moveSpeed * Time.deltaTime;
        rb.MovePosition(newPos); // respects collisions!
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(revealed && collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player caught by Mimic!");
            collision.collider.GetComponent<PlayerController>().Respawn();
        }
    }
}


