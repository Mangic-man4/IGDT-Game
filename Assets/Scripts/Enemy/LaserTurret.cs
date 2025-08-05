using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LaserTurret : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;              // Still on root
    public SpriteRenderer laserSpriteRenderer;   // On child object (LaserBeam)

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right, 100f);
        Vector3 endPoint = hitInfo ? hitInfo.point : firePoint.position + firePoint.right * 100f;

        Debug.DrawRay(firePoint.position, firePoint.right, Color.red);
 
        float laserLength = Vector2.Distance(firePoint.position, endPoint);
 
        Debug.Log($"Laser Length: {laserLength}");

        laserSpriteRenderer.size = new Vector2(laserLength, laserSpriteRenderer.size.y);
        laserSpriteRenderer.transform.position = firePoint.position;
        laserSpriteRenderer.transform.rotation = firePoint.rotation; // just in case rotation matters
    }

 //   void ShootLaser()
 //   {
 //       if (laserSpriteRenderer== null) return;
 //
 //       RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);
 //       Vector3 endPoint;
 //
 //       Debug.DrawRay(firePoint.position, firePoint.right * 5f, Color.red);
 //
 //       if (hitInfo)
 //       {
 //           endPoint = hitInfo.point;
 //           
 //           if (hitInfo.collider.CompareTag("Player") &&
 //               hitInfo.collider.TryGetComponent<PlayerPowerUps>(out var powerUps) &&
 //               hitInfo.collider.TryGetComponent<PlayerController>(out var playerController))
 //           {
 //               if (powerUps.IsShieldActive() && powerUps.IsEnemyProtectionEnabled())
 //               {
 //                   if (powerUps.TryUseShield())
 //                   {
 //                       Debug.Log("Laser hit absorbed by shield.");
 //                       return;
 //                   }
 //               }
 //
 //               if (!powerUps.IsInvincible())
 //               {
 //                   playerController.Die();
 //                   Debug.Log("Player has died! Triggered by LaserTurret.");
 //               }
 //           
 //
 //       }
 //       else
 //       {
 //           endPoint = firePoint.position + firePoint.right * 100f;
 //       }
 //
 //       float laserLength = Vector2.Distance(firePoint.position, endPoint);
 //
 //       laserSpriteRenderer.size = new Vector2(laserLength, laserSpriteRenderer.size.y);
 //       laserSpriteRenderer.transform.position = firePoint.position;
 //   }
    public void RespawnTurret()
    {
        transform.SetPositionAndRotation(originalPosition, originalRotation);
        gameObject.SetActive(true);
    }
}
