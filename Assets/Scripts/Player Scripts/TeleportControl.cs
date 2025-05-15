using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportControl : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float teleportDistance = 3f;
    [SerializeField] private float teleportCooldown = 0.5f;

    [Header("Teleport Guide (optional)")]
    [SerializeField] private GameObject teleportGuidePrefab;

    private float lastTeleportTime;
    private bool isPaused = false;

    private PlayerPowerUps powerUps;

    [Header("Teleport Visual Ghost")]
    [SerializeField] private float ghostAlpha = 0.3f;

    private GameObject teleportGhost;
    private Animator ghostAnimator;
    private Animator playerAnimator;

    private TeleportGuide teleportGuide;



    private void Start()
    {
        powerUps = GetComponent<PlayerPowerUps>();
        playerAnimator = GetComponent<Animator>();

        // Clone visual ghost
        teleportGhost = Instantiate(gameObject, transform.position, Quaternion.identity);
        teleportGhost.name = "TeleportGhost";

        // Remove unnecessary gameplay components from ghost
        Destroy(teleportGhost.GetComponent<TeleportControl>());
        Destroy(teleportGhost.GetComponent<PlayerController>());
        Destroy(teleportGhost.GetComponent<PlayerPowerUps>());
        Destroy(teleportGhost.GetComponent<Rigidbody2D>());
        Destroy(teleportGhost.GetComponent<Collider2D>());

        // Optional: Remove any audio source or effects
        Destroy(teleportGhost.GetComponent<AudioSource>());

        // Fade all sprites
        foreach (var sr in teleportGhost.GetComponentsInChildren<SpriteRenderer>())
        {
            Color faded = sr.color;
            faded.a = ghostAlpha;
            sr.color = faded;

        }

        // Cache the ghost animator
        ghostAnimator = teleportGhost.GetComponent<Animator>();
        GameObject guideObj = Instantiate(teleportGuidePrefab);
        teleportGuide = guideObj.GetComponent<TeleportGuide>();
        teleportGuide.player = transform;
        teleportGuide.teleportDistance = teleportDistance;
        teleportGuide.teleportGhost = teleportGhost;
        teleportGhost.SetActive(IsEasyDifficulty());

    }

    private void Update()
    {
        if (isPaused) return;

        // Toggle teleport guide with G
        if (Input.GetKeyDown(KeyCode.G) && teleportGhost != null)
        {
            teleportGhost.SetActive(!teleportGhost.activeSelf);
        }


        bool canTeleport = Time.time > lastTeleportTime + teleportCooldown;

        if (canTeleport && Input.GetKeyDown(KeyCode.F))
        {
            if (powerUps.hasDash)
            {
                powerUps.PerformDash(); // Dash overrides teleport
            }
            else
            {
                PerformTeleport();
            }
        }

        if (teleportGhost != null && teleportGhost.activeSelf)
        {
            Vector3 direction;
            float previewDistance;

            // Check dash mode
            if (powerUps != null && powerUps.hasDash)
            {
                direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
                previewDistance = powerUps.dashDistance;
            }
            else
            {
                direction = VerticalModeManager.IsVertical
                    ? (transform.position.x < 0 ? Vector3.right : Vector3.left)
                    : (transform.position.y < -3f ? Vector3.up : Vector3.down);

                previewDistance = teleportDistance;
            }

            // Final target position
            Vector3 targetPosition = transform.position + direction * previewDistance;
            teleportGhost.transform.position = targetPosition;
            teleportGhost.transform.localScale = transform.localScale;

            // === Safety check and sprite tint ===
            bool isSafe = IsTeleportTargetSafe(targetPosition);
            Color ghostColor = isSafe ? Color.green : Color.red;

            foreach (var sr in teleportGhost.GetComponentsInChildren<SpriteRenderer>())
            {
                Color c = ghostColor;
                c.a = sr.color.a; // Preserve alpha
                sr.color = c;
            }

            // Sync animation state
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
            ghostAnimator.Play(stateInfo.shortNameHash, 0, stateInfo.normalizedTime);
        }

    }

    private void PerformTeleport()
    {
        Vector3 newPosition = transform.position;

        if (VerticalModeManager.IsVertical)
        {
            // Horizontal teleport (left/right)
            newPosition += transform.position.x < 0
                ? Vector3.right * teleportDistance
                : Vector3.left * teleportDistance;
        }
        else
        {
            // Vertical teleport (up/down)
            newPosition += transform.position.y < -3f
                ? Vector3.up * teleportDistance
                : Vector3.down * teleportDistance;
        }

        transform.position = newPosition;
        lastTeleportTime = Time.time;
    }

    private bool IsEasyDifficulty()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName.Contains("Easy");
    }


    public void SetPauseState(bool pause)
    {
        isPaused = pause;
    }

    public bool IsTeleportTargetSafe(Vector3 destination)
    {
        float checkRadius = 0.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(destination, checkRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("ObbyCourse") || hit.CompareTag("Door") || hit.CompareTag("DashWall") || hit.CompareTag("Spike") || hit.CompareTag("Laser"))
                return false;
        }

        return true;
    }

}