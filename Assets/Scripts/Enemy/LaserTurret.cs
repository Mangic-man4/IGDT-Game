using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LaserTurret : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;              // Still on root
    public LineRenderer laserBeamRenderer;   // On child object (LaserBeam)

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        if (firePoint == null || laserBeamRenderer == null) return;

        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);
        Vector3 endPoint;

        if (hitInfo)
        {
            endPoint = hitInfo.point;

            if (hitInfo.collider.CompareTag("Player") &&
                hitInfo.collider.TryGetComponent<PlayerPowerUps>(out var powerUps) &&
                hitInfo.collider.TryGetComponent<PlayerController>(out var playerController))
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
        else
        {
            endPoint = firePoint.position + firePoint.right * 100f;
        }

        laserBeamRenderer.SetPosition(0, firePoint.position);
        laserBeamRenderer.SetPosition(1, endPoint);
    }
}
