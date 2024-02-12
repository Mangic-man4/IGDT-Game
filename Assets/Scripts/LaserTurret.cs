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

            if (hitInfo.collider.CompareTag("Player"))
            {
                // Assuming the player has a script with a method to handle death.
                // Get the current scene's build index
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

                // Load the scene with the current build index
                SceneManager.LoadScene(currentSceneIndex);

                // Set Time.timeScale to 1f after scene reloads
                Time.timeScale = 1f;

                Debug.Log("Player has died! Reloading scene...");
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
