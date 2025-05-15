using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class LaserTurret : MonoBehaviour
{
    public Transform firePoint; // Assign this in the inspector to the point where the laser starts.
    public LineRenderer lineRenderer; // Assign a LineRenderer component in the inspector.

    public GameObject laserColliderObject; 
    private BoxCollider2D laserCollider;

    private Vector3 lastEndPoint;
    private Vector2 lastDirection;
    private float lastLength;


    void Start()
    {
        if (laserColliderObject != null)
            laserCollider = laserColliderObject.GetComponent<BoxCollider2D>();
    }


    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);
        Vector3 start = firePoint.position;
        Vector3 end = hitInfo ? hitInfo.point : firePoint.position + firePoint.right * 100f;

        // === Kill player if hit ===
        if (hitInfo && hitInfo.collider.CompareTag("Player") &&
            hitInfo.collider.TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.Die();
            Debug.Log("Player has died! Triggered by LaserTurret.");
        }

        // === Update LineRenderer ===
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // === Update Collider if laser direction/length changed significantly ===
        if (laserCollider != null)
        {
            Vector2 direction = (end - start).normalized;
            float length = Vector2.Distance(start, end);

            // Skip updates if direction and length haven't changed (to reduce flicker)
            if (direction != lastDirection || Mathf.Abs(length - lastLength) > 0.01f)
            {
                lastDirection = direction;
                lastLength = length;
                lastEndPoint = end;

                Vector2 midPoint = (start + end) / 2f;

                laserCollider.transform.position = midPoint;
                laserCollider.transform.right = direction;
                laserCollider.size = new Vector2(length, 0.1f);
            }
        }
    }

    // This method is called when the player teleports into the turret.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroy the turret
        }
    }
}
