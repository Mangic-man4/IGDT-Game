using UnityEngine;

public class PowerupFallCleanup : MonoBehaviour
{
    private Rigidbody2D rb;
    //private Collider2D pickupTrigger; // Optional: reference to the trigger collider for pickup

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Dynamically add a solid collider for landing detection
        var landingCollider = gameObject.AddComponent<BoxCollider2D>();
        landingCollider.isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("ObbyCourse"))
        {
            RemovePhysics();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RemovePhysics();
        }
    }

    private void RemovePhysics()
    {
        // Remove Rigidbody and any non-trigger colliders
        if (rb != null)
        {
            Destroy(rb);
        }

        // Remove all non-trigger colliders (leave pickup trigger if needed)
        foreach (var col in GetComponents<Collider2D>())
        {
            if (!col.isTrigger)
            {
                Destroy(col);
            }
        }

        // Optional: You could add a little pickup animation here
    }
}
