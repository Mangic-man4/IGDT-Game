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
    [SerializeField] float landingVFXMinSpeed = 0f; // optional: only poof if falling at least this fast
    private bool landingVFXPlayed = false;          // one-shot guard per landing

    [Header("Grounding")]
    [SerializeField, Tooltip("Minimum upward normal.y to count as ground. 0.5 ≈ slopes up to ~60°.")]
    float minGroundNormalY = 0.5f;
    [SerializeField, Tooltip("Ray length to verify support while in landed mode.")]
    float groundCheckDistance = 0.2f;
    [SerializeField, Tooltip("Which layers are considered solid ground for support checks.")]
    LayerMask groundMask;
    [SerializeField] private float settleGrace = 0.1f; // small delay before support re-check
    private float landedAtTime = -1f;

    Rigidbody2D rb;
    Collider2D landingCollider;   // solid collider used only for terrain land
    Collider2D pickupTrigger;     // existing trigger collider on prefab

    bool landed;
    bool consumed;

    // Cache last ground we landed on (helps with crumbling platforms)
    Collider2D lastSupportingCollider;

    public void SetLandingVFX(GameObject prefab)
    {
        landingVFXPrefab = prefab;
    }

    // One-shot initializer for runtime-attached pickups
    public void Init(GameObject landingVFX,
                     LayerMask groundMask,
                     float minNormalY = 0.5f,
                     float checkDist = 0.2f,
                     float settleGraceSeconds = 0.05f)
    {
        landingVFXPrefab = landingVFX;
        this.groundMask = groundMask;
        this.minGroundNormalY = minNormalY;
        this.groundCheckDistance = checkDist;
        this.settleGrace = settleGraceSeconds;
    }

    // Safety default if Init() not called
    void OnEnable()
    {
        if (groundMask.value == 0)
        {
            // Default to some common layers. Prefer setting this via Init() from the spawner.
            groundMask = LayerMask.GetMask("Ground", "Platform", "TestPlatforms");

        }
    }

    void Awake()
    {
        if (TryGetComponent<Rigidbody2D>(out rb))
        {
            rb.freezeRotation = true;
            rb.angularVelocity = 0f;
        }

        // Find existing colliders: keep the trigger, add/ensure a solid
        foreach (var c in GetComponents<Collider2D>())
        {
            if (c.isTrigger)
            {
                pickupTrigger = c;
            }
            else if (landingCollider == null)
            {
                landingCollider = c;
            }
        }
        if (landingCollider == null)
        {
            landingCollider = gameObject.AddComponent<BoxCollider2D>();
            landingCollider.isTrigger = false;
        }

        // Solid should never block player/mimics, only terrain
        IgnorePlayerAndMimics();
    }

    void Update()
    {
        if (landed && !consumed)
        {
            float sinceLanded = Time.time - landedAtTime;

            if (sinceLanded < settleGrace)
            {
                // within grace window, skip the support check to avoid flip-flop
                return;
            }

            if (!IsStillSupported())
            {
                ResumeFalling();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (consumed) return;

        string colTag = collision.collider.tag;
        int colLayer = collision.collider.gameObject.layer;

        // Determine average contact normal
        Vector2 avgNormal = Vector2.zero;
        int count = collision.contactCount;
        for (int i = 0; i < count; i++)
        {
            avgNormal += collision.GetContact(i).normal;
        }
        if (count > 0)
        {
            avgNormal /= count;
        }

        float vy = rb != null ? rb.velocity.y : 0f;

        // Only respond to solid ground/obby collisions
        if (!(collision.collider.CompareTag("Ground") || collision.collider.CompareTag("ObbyCourse")))
        {
            // If we bump player/mimic anyway, keep ignoring solid collision
            if (IsPlayer(collision.collider) || IsMimic(collision.collider))
            {
                if (landingCollider != null)
                {
                    Physics2D.IgnoreCollision(landingCollider, collision.collider, true);
                }
            }
            return;
        }

        // Ceilings or steep underside hits should not count as landing
        if (avgNormal.y < minGroundNormalY)
        {
            if (avgNormal.y <= -minGroundNormalY && rb != null && !landed)
            {
                if (rb.velocity.y > -1f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -1f);
                }
            }
            return;
        }

        // Land poof at first contact point (one-shot per landing)
        if (landingVFXPrefab != null && !landingVFXPlayed)
        {
            if (rb == null || Mathf.Abs(rb.velocity.y) >= landingVFXMinSpeed)
            {
                Vector3 pos = transform.position;
                if (collision.contactCount > 0)
                {
                    pos = collision.GetContact(0).point;
                }

                var vfx = Instantiate(landingVFXPrefab, pos + new Vector3(0.5f, 0f, 0f), Quaternion.identity);
                Destroy(vfx, landingVFXLifetime);
                landingVFXPlayed = true;
            }

        }

        lastSupportingCollider = collision.collider;
        ConvertToPickupMode();
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!landed) return;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed) return;

        if (IsPlayer(other))
        {
            var ppu = other.GetComponent<PlayerPowerUps>();
            var pup = GetComponent<PowerUpPickup>(); // holds the powerUpType

            if (ppu != null && pup != null)
            {
                ppu.CollectPowerUp(pup.powerUpType);
                consumed = true;
                Destroy(gameObject);
                return;
            }

            // Fallback if something’s missing: still ensure pickup trigger isn’t blocked
            ConvertToPickupMode();
        }
    }

    void ConvertToPickupMode()
    {
        if (landed) return;
        landed = true;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // gravity off, triggers still fire
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (landingCollider != null)
        {
            landingCollider.enabled = false; // disable solid
        }

        landedAtTime = Time.time;
    }

    void ResumeFalling()
    {
        landed = false;
        lastSupportingCollider = null;
        landingVFXPlayed = false; // allow VFX on next landing

        if (landingCollider != null)
        {
            landingCollider.enabled = true;
        }

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            if (rb.velocity.y > -0.1f)
            {
                rb.velocity = new Vector2(rb.velocity.x, -0.1f); // tiny downward nudge
            }
        }
    }

    bool IsStillSupported()
    {
        if (groundMask.value == 0)
        {
            return true;
        }

        // Use the *bottom* of our current bounds as probe origin/area
        Bounds b;
        if (pickupTrigger != null) b = pickupTrigger.bounds;
        else if (landingCollider != null) b = landingCollider.bounds;
        else
        {
            // Last-resort long ray from slightly below pivot
            Vector2 fallbackOrigin = (Vector2)transform.position + Vector2.down * 0.25f;
            float longDist = Mathf.Max(0.5f, groundCheckDistance);
            RaycastHit2D rh = Physics2D.Raycast(fallbackOrigin, Vector2.down, longDist, groundMask);
            return rh.collider != null && rh.normal.y >= minGroundNormalY;
        }

        // Small overlap box right under our feet
        Vector2 center = new Vector2(b.center.x, b.min.y - 0.03f);
        Vector2 size = new Vector2(Mathf.Max(0.05f, b.size.x * 0.8f), 0.06f);
        Collider2D hitCol = Physics2D.OverlapBox(center, size, 0f, groundMask);

        if (hitCol == null)
            return false;

        // Optional short ray to verify "upward-ish" surface
        RaycastHit2D rh2 = Physics2D.Raycast(center, Vector2.down, Mathf.Max(0.15f, groundCheckDistance), groundMask);
        if (rh2.collider == null)
            return true;

        bool ok = rh2.normal.y >= minGroundNormalY;
        return ok;
    }


    // --- helpers ---
    void IgnorePlayerAndMimics()
    {
        if (landingCollider == null) return;

        // Ignore player
        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            foreach (var c in player.GetComponentsInChildren<Collider2D>(true))
            {
                Physics2D.IgnoreCollision(landingCollider, c, true);
            }
        }

        // Ignore mimics
        foreach (var tag in mimicTags)
        {
            var arr = GameObject.FindGameObjectsWithTag(tag);
            foreach (var go in arr)
            {
                foreach (var c in go.GetComponentsInChildren<Collider2D>(true))
                {
                    Physics2D.IgnoreCollision(landingCollider, c, true);
                }
            }
        }

        // Optional layer-wide ignore
        if (mimicLayer >= 0)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, mimicLayer, true);
        }
    }

    bool IsPlayer(Component c)
    {
        return c != null && c.CompareTag(playerTag);
    }

    bool IsMimic(Component c)
    {
        if (c == null) return false;
        string t = c.tag;
        for (int i = 0; i < mimicTags.Length; i++)
        {
            if (t == mimicTags[i]) return true;
        }
        return false;
    }
}