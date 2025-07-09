using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportControl : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float teleportDistance;
    [SerializeField] private float teleportCooldown = 0.5f;

    [Header("Teleport Guide (optional)")]
    [SerializeField] private GameObject teleportGuidePrefab;

    [Header("Teleport Visual Ghost")]
    [SerializeField] private float ghostAlpha = 0.3f;

    [Header("Vertical Mode Settings")]
    public float verticalXThreshold = 0f;  // Default fallback to 0


    private float lastTeleportTime;
    private bool isPaused = false;
    [HideInInspector]public bool updatingFromUI = false;


    private GameObject teleportGhost;
    private Animator ghostAnimator;
    private Animator playerAnimator;
    private PlayerPowerUps powerUps;
    private TeleportGuide teleportGuide;

    [Header("Dash & Teleport FX")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private GameObject teleportBurstPrefab;
    //[SerializeField] private float dashPreviewLength = 0.1f; // optional, for tiny streak



    private void Start()
    {
        powerUps = GetComponent<PlayerPowerUps>();
        playerAnimator = GetComponent<Animator>();

        // Clone visual ghost
        teleportGhost = Instantiate(gameObject, transform.position, Quaternion.identity);
        teleportGhost.name = "TeleportGhost";
        teleportGhost.tag = "Ghost";
        teleportGhost.layer = 13;

        // Remove unnecessary gameplay components from ghost
        Destroy(teleportGhost.GetComponent<TeleportControl>());
        Destroy(teleportGhost.GetComponent<PlayerController>());
        Destroy(teleportGhost.GetComponent<PlayerPowerUps>());
        Destroy(teleportGhost.GetComponent<Rigidbody2D>());
        Destroy(teleportGhost.GetComponent<Collider2D>());
        Destroy(teleportGhost.GetComponent<BoxCollider2D>());
        Destroy(teleportGhost.GetComponent<ItemCollector>());
        Destroy(teleportGhost.GetComponent<DevRespawn>());
        Destroy(teleportGhost.GetComponent<Timer>());
        foreach (var audio in teleportGhost.GetComponents<AudioSource>())
        {
            Destroy(audio);
        }

        // Destroy named child objects
        string[] childrenToDestroy = { "GroundCheck", "FirePoint" };
        foreach (string childName in childrenToDestroy)
        {
            Transform child = teleportGhost.transform.Find(childName);
            if (child != null)
                Destroy(child.gameObject);
        }




        // Fade all sprites
        foreach (var sr in teleportGhost.GetComponentsInChildren<SpriteRenderer>())
        {
            Color faded = sr.color;
            faded.a = ghostAlpha;
            sr.color = faded;

        }

        // Cache the ghost animator
        GhostSettings.LoadSettings();
        GhostSettings.LoadColors();

        ghostAnimator = teleportGhost.GetComponent<Animator>();
        GameObject guideObj = Instantiate(teleportGuidePrefab);
        teleportGuide = guideObj.GetComponent<TeleportGuide>();
        teleportGuide.player = transform;
        teleportGuide.teleportGhost = teleportGhost;
        teleportGhost.SetActive(IsEasyDifficulty() && GhostSettings.enableGhost);

    }

    private void Update()
    {
        if (isPaused) return;

        if (KeyBindings.GetKeyDown(ActionKey.ToggleGhost))
        {
            if (teleportGhost != null)
            {
                SetGhostVisibility(!teleportGhost.activeSelf);
            }
        }



        // Attempt teleport
        if (Time.time > lastTeleportTime + teleportCooldown && KeyBindings.GetKeyDown(ActionKey.Teleport))
        {
            if (powerUps.hasDash)
                powerUps.PerformDash();
            else
                PerformTeleport();
        }

        // Update ghost preview
        if (teleportGhost != null && teleportGhost.activeSelf)
        {
            Vector3 direction;
            float previewDistance;

            if (powerUps.hasDash)
            {
                direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
                previewDistance = powerUps.dashDistance;
            }
            else
            {
                direction = VerticalModeManager.IsVertical
                    ? (transform.position.x < verticalXThreshold ? Vector3.right : Vector3.left)
                    : (transform.position.y < -3f ? Vector3.up : Vector3.down);
                previewDistance = teleportDistance;
            }

            Vector3 targetPosition = transform.position + direction * previewDistance;
            teleportGhost.transform.position = targetPosition;
            teleportGhost.transform.localScale = transform.localScale;

            // Safety check
            bool isSafe = IsTeleportTargetSafe(teleportGhost.transform.position);

            Color ghostColor = GhostSettings.ghostColor;

            if (IsEasyDifficulty() && GhostSettings.enableTinting)
            {
                ghostColor = isSafe ? GhostSettings.safeColor : GhostSettings.unsafeColor;
            }

            ghostColor.a = GhostSettings.ghostAlpha;
            foreach (var sr in teleportGhost.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = ghostColor;
            }

            /* //Old
            foreach (var sr in teleportGhost.GetComponentsInChildren<SpriteRenderer>())
            {
                Color tinted = ghostColor;
                tinted.a = sr.color.a;
                sr.color = tinted;
            }*/


            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
            ghostAnimator.Play(stateInfo.shortNameHash, 0, stateInfo.normalizedTime);
        }
    }




    private void PerformTeleport()
    {
        Vector3 startPos = transform.position;
        Vector3 direction = VerticalModeManager.IsVertical
            ? (transform.position.x < verticalXThreshold? Vector3.right : Vector3.left)
            : (transform.position.y < -3f ? Vector3.up : Vector3.down);

        Vector3 newPosition = transform.position + direction * teleportDistance;

        // --- FX ---
        if (afterImagePrefab)
        {
            GameObject ghost = Instantiate(afterImagePrefab, startPos, transform.rotation);
            ghost.transform.localScale = transform.localScale;
        }

        if (teleportBurstPrefab)
        {
            GameObject burst = Instantiate(teleportBurstPrefab, newPosition, Quaternion.identity);

            if (burst.TryGetComponent<ParticleSystem>(out var ps))
            {
                Destroy(burst, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(burst, 1.5f); // fallback time if no particle system is found
            }
        }



        transform.position = newPosition;
        lastTeleportTime = Time.time;

        // Define which tags are allowed to be killed
        string[] killableTags = { "Enemy"};

        // Check if any enemies are hit
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(newPosition, 0.5f);
        foreach (var col in overlaps)
        {
            if (col.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                // Only kill if the tag is in the allowed list
                if (killableTags.Contains(col.tag))
                {
                    enemyHealth.Kill();
                }
            }
        }
    }



    public bool IsEasyDifficulty()
    {
        return SceneManager.GetActiveScene().name.Contains("Easy");
    }

    public void SetPauseState(bool pause)
    {
        isPaused = pause;
    }



    public bool IsTeleportTargetSafe(Vector3 destination)
    {
        // Check tagged colliders
        Collider2D[] hits = Physics2D.OverlapCircleAll(destination, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("ObbyCourse") || hit.CompareTag("Door") ||
                hit.CompareTag("DashWall") || hit.CompareTag("Spike") ||
                hit.CompareTag("Laser"))
                return false;
        }
        return true;
    }
    public void SetGhostVisibility(bool isVisible)
    {
        if (teleportGhost == null) 
            return;

        teleportGhost.SetActive(isVisible);
        GhostSettings.enableGhost = isVisible;
        GhostSettings.SaveSettings();

        // Also update the line if it's Easy difficulty
        if (IsEasyDifficulty() && teleportGuide != null &&
            teleportGuide.TryGetComponent(out LineRenderer line))
        {
            line.enabled = isVisible;
        }


        // Sync the toggle only if not triggered by UI
        if (!updatingFromUI)
        {
            GhostSettingsUI ui = FindObjectOfType<GhostSettingsUI>();
            if (ui != null && ui.ghostEnableToggle != null)
            {
                ui.ghostEnableToggle.SetIsOnWithoutNotify(isVisible);
            }
        }
        NotifyGhostUIUpdate(isVisible);

    }

    public void SetGhostVisibilityFromUI(bool isVisible)
    {
        updatingFromUI = true;
        SetGhostVisibility(isVisible);
        updatingFromUI = false;
    }
    private void NotifyGhostUIUpdate(bool visible)
    {
        GhostSettingsUI ui = FindObjectOfType<GhostSettingsUI>();
        if (ui != null && ui.ghostEnableToggle != null)
        {
            ui.ghostEnableToggle.SetIsOnWithoutNotify(visible); 
        }
    }

}