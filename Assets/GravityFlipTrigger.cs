using UnityEngine;

public class GravityFlipTrigger : MonoBehaviour
{
    private bool isPlayerInside = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            ToggleGravity(other.gameObject.GetComponent<Rigidbody2D>(), other.gameObject.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    void ToggleGravity(Rigidbody2D rb, Transform playerTransform)
    {
        rb.gravityScale *= -1;

        // Rotate the player 180 degrees to flip upside down
        playerTransform.Rotate(Vector3.forward, 180f);

        // Flip the player's sprite along with the gravity change
        SpriteRenderer spriteRenderer = playerTransform.GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
