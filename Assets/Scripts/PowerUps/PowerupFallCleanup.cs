using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PowerupFallCleanup : MonoBehaviour
{
    [Header("Filters")]
    [SerializeField] string playerTag = "Player";
    [SerializeField] string[] mimicTags = { "KeyMimic", "CoinMimic", "MimicBoss" };
    [SerializeField] int mimicLayer = 19; // optional

    [Header("Landing VFX (optional)")]
    [SerializeField] GameObject landingVFXPrefab;   // assign your jump-poof prefab here
    [SerializeField] float landingVFXLifetime = 1.5f;

    Rigidbody2D rb;
    Collider2D landingCollider;   // solid collider used only for terrain land
    Collider2D pickupTrigger;     // existing trigger collider on prefab
    bool landed;
    bool consumed;

    public void SetLandingVFX(GameObject prefab) => landingVFXPrefab = prefab;


    void Awake()
    {
        
        if (TryGetComponent<Rigidbody2D>(out rb))
        {
            rb.freezeRotation = true; // or: rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
            rb.angularVelocity = 0f;
        }

        // Find existing colliders: keep the trigger, add/ensure a solid
        foreach (var c in GetComponents<Collider2D>())
        {
            if (c.isTrigger) pickupTrigger = c;
            else if (landingCollider == null) landingCollider = c;
        }
        if (landingCollider == null)
        {
            landingCollider = gameObject.AddComponent<BoxCollider2D>();
            landingCollider.isTrigger = false;
        }

        // Solid should never block player/mimics, only terrain
        IgnorePlayerAndMimics();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (consumed) return;

        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("ObbyCourse"))
        {
            // Land poof
            if (landingVFXPrefab != null)
            {
                Vector3 pos = transform.position;
                if (collision.contactCount > 0) pos = collision.GetContact(0).point;
                var vfx = Instantiate(landingVFXPrefab, pos + new Vector3(0.5f, 0f, 0f), Quaternion.identity);
                Destroy(vfx, landingVFXLifetime);
            }

            ConvertToPickupMode();   // sit on ground, trigger still active
            return;
        }

        // If we bump player/mimic anyway, ignore that pair so no bounce
        if (IsPlayer(collision.collider) || IsMimic(collision.collider))
        {
            if (landingCollider != null)
                Physics2D.IgnoreCollision(landingCollider, collision.collider, true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed) return;

        if (IsPlayer(other))
        {
            // --- 1-liner hook: collect immediately ---
            var ppu = other.GetComponent<PlayerPowerUps>();
            var pup = GetComponent<PowerUpPickup>(); // holds the powerUpType

            if (ppu != null && pup != null)
            {
                ppu.CollectPowerUp(pup.powerUpType);
                consumed = true;
                Destroy(gameObject);
                return; // no need to ConvertToPickupMode() if we’re gone
            }

            // Fallback if something’s missing: still ensure pickup trigger isn’t blocked
            ConvertToPickupMode();
        }
    }

    void ConvertToPickupMode()
    {
        if (landed) return;
        landed = true;

        // Freeze physics without destroying the body (prevents tunneling)
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // gravity off, triggers still fire
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Disable only the solid; keep trigger alive for pickup
        if (landingCollider != null) landingCollider.enabled = false;
    }

    // --- helpers ---
    void IgnorePlayerAndMimics()
    {
        if (landingCollider == null) return;

        // Player
        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
            foreach (var pc in player.GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(landingCollider, pc, true);

        // Mimics by tag
        foreach (var tag in mimicTags)
        {
            var mimics = GameObject.FindGameObjectsWithTag(tag);
            foreach (var go in mimics)
                foreach (var c in go.GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(landingCollider, c, true);
        }

        // Mimics by layer (belt & suspenders)
        foreach (var c in FindObjectsOfType<Collider2D>())
            if (c.gameObject.layer == mimicLayer)
                Physics2D.IgnoreCollision(landingCollider, c, true);
    }

    bool IsPlayer(Component c) => c != null && c.CompareTag(playerTag);
    bool IsMimic(Component c)
    {
        if (c == null) return false;
        if (c.gameObject.layer == mimicLayer) return true;
        foreach (var t in mimicTags) if (c.CompareTag(t)) return true;
        return false;
    }
}
