using UnityEngine;


public class LaserTurret : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;              // Still on root
    public float maxDistance = 100f;
    public LayerMask layerMask;

    private SpriteRenderer laserRenderer;   // On child object (LaserBeam)

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // For laser
    private float distance;
    private Vector3 direction;

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void Start()
    {
        if (firePoint != null)
            laserRenderer = firePoint.GetComponent<SpriteRenderer>();

        GetPlacementDirection();
    }

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        if (firePoint == null || laserRenderer == null) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, layerMask);
        distance = hit.collider ? hit.distance : maxDistance;

        Vector2 size = laserRenderer.size;
        size.x = distance;
        laserRenderer.size = size;

        // Position the laser midpoint in world space
        firePoint.position = transform.position + (direction * distance * 0.5f);

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

    public void RespawnTurret()
    {
        transform.SetPositionAndRotation(originalPosition, originalRotation);
        gameObject.SetActive(true);
    }

    void GetPlacementDirection()
    {
        // Determine direction based on scale.x (lossyScale.x)
        float facing = Mathf.Sign(transform.lossyScale.x);
        direction = new Vector2(facing, 0f);
    }
}
