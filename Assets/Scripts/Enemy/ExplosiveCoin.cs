using UnityEngine;

public class ExplosiveCoin : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 2.5f;
    public LayerMask explosionLayers;
    public GameObject explosionEffectPrefab;

    [Header("Throw Settings")]
    public bool autoLaunchOnStart = false;
    public float launchForce = 5f;
    public Vector2 launchDirection = new(1f, 1f);

    private Rigidbody2D rb;
    private bool hasExploded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Apply arc force (not needed because the boss code handles this already)
        if (autoLaunchOnStart)
        {
            rb.AddForce(launchDirection.normalized * launchForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;

        string tag = collision.collider.tag;
        if (tag == "Player" || tag == "Fireball" || tag == "Ground" || tag == "ObbyCourse")
        {
            Explode();
        }
    }


    void Explode()
    {
        hasExploded = true;

        if (explosionEffectPrefab != null)
        {
            if (explosionEffectPrefab != null)
            {
                GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 1.5f); // Destroy after 1.5 seconds (adjust as needed)
            }
        }

        // Damage all valid targets in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explosionLayers);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player") &&
                hit.TryGetComponent(out PlayerPowerUps powerUps) &&
                hit.TryGetComponent(out PlayerController playerController))
            {
                if (powerUps.IsShieldActive() && powerUps.IsTrapProtectionEnabled())
                {
                    if (powerUps.TryUseShield())
                    {
                        Debug.Log("Explosion absorbed by shield.");
                        continue;
                    }
                }

                if (!powerUps.IsInvincible())
                {
                    playerController.Die();
                    Debug.Log("Player killed by explosion.");
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
