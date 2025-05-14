using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class LaserTurret : MonoBehaviour
{
    public Transform firePoint; // Assign this in the inspector to the point where the laser starts.
    public LineRenderer lineRenderer; // Assign a LineRenderer component in the inspector.

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);
        if (hitInfo)
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hitInfo.point);

            if (hitInfo.collider.CompareTag("Player") && hitInfo.collider.TryGetComponent<PlayerController>(out var playerController))
            {
                playerController.Die();
                Debug.Log("Player has died! Triggered by LaserTurret.");
            }
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.right * 100);
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
