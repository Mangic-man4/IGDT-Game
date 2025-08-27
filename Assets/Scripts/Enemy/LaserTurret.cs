using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Child transform that holds the SpriteRenderer for the beam. Pivot should be centered.")]
    public Transform firePoint;                       // Child with SpriteRenderer
    public float maxDistance = 100f;
    public LayerMask layerMask;

    [Header("Muzzle & Beam")]
    [Tooltip("Local-space offset of the ray start from the turret pivot (e.g. barrel tip).")]
    public Vector2 localMuzzleOffset = Vector2.zero;  // Adjust in Inspector to match your sprite
    [Tooltip("Small offset along the direction so the ray doesn't hit own collider.")]
    public float selfHitEpsilon = 0.01f;

    private SpriteRenderer laserRenderer;             // On firePoint
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    // Computed per frame
    private Vector3 dir;
    private Vector3 worldStart;
    private float distance;

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
    }

    void Start()
    {
        if (firePoint != null)
            laserRenderer = firePoint.GetComponent<SpriteRenderer>();

        if (laserRenderer == null)
        {
            Debug.LogWarning("[LaserTurret] Missing SpriteRenderer on firePoint.");
        }
    }

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        if (firePoint == null || laserRenderer == null) return;

        dir = (-transform.right).normalized;

        // 1) World start: turret pivot + local muzzle offset (rotated with the turret).
        worldStart = transform.TransformPoint(localMuzzleOffset);

        // 2) Raycast
        Vector3 rayStart = worldStart + dir * selfHitEpsilon;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, dir, maxDistance, layerMask);
        distance = hit.collider ? hit.distance : maxDistance;

        // 3) Align the beam child so its local +X points in firing direction.
        firePoint.right = dir;

        var size = laserRenderer.size;
        size.x = distance;
        laserRenderer.size = size;

        // 4) Position the beam center at midpoint between start and hit (or max range)
        firePoint.position = worldStart + dir * (distance * 0.5f);

        // 5) Player hit logic
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            var powerUps = hit.collider.GetComponent<PlayerPowerUps>();
            var playerController = hit.collider.GetComponent<PlayerController>();

            if (powerUps != null && playerController != null)
            {
                if (powerUps.IsShieldActive() && powerUps.IsEnemyProtectionEnabled())
                {
                    if (powerUps.TryUseShield())
                    {
                        Debug.Log("Laser hit absorbed by shield.");
                        return;
                    }
                }

                if (!powerUps.IsInvincible())
                {
                    playerController.Die();
                    Debug.Log("Player has died! Triggered by LaserTurret.");
                }
            }
        }
    }

    public void ResetTurret()
    {
        transform.SetPositionAndRotation(originalPosition, originalRotation);
        transform.localScale = originalScale;
    }

    public void RespawnTurret()
    {
        ResetTurret();
        gameObject.SetActive(true);
    }
}
