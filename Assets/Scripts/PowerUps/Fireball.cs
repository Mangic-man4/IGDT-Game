using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 5f;

    private Vector2 direction;

    /// <summary>
    /// Set the direction the fireball should move and schedule its destruction.
    /// </summary>
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        transform.Translate(speed * Time.fixedDeltaTime * direction);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for valid target tags
        if (other.CompareTag("Enemy") || other.CompareTag("CoinMimic") || other.CompareTag("KeyMimic"))
        {
            if (other.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player") && !other.CompareTag("PowerUp") && !other.CompareTag("Laser"))
        {
            Destroy(gameObject);
        }
    }

}