using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportControl : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float teleportDistance;
    [SerializeField] private float teleportCooldown = 0.5f;

    [Header("Cooldown Ready Cue")]
    [SerializeField] private AudioClip cooldownReadySfx;
    [SerializeField, Range(0f, 1f)] private float cooldownReadyVolume = 1f;
    [SerializeField] private GameObject cooldownReadyVfx;   // optional (sparkle/ping)
    [SerializeField] private Vector3 cooldownReadyVfxOffset = Vector3.zero;

    // Optional: assign a dedicated SFX AudioSource on the Main Camera in the Inspector.
    // If null, we’ll fall back to Camera.main + PlayClipAtPoint.
    [SerializeField] private AudioSource cameraSfxSource;

    private bool cooldownReadyPlayed = true; // true at start so we don't ping on scene load

    [Header("Teleport Guide (optional)")]
    [SerializeField] private GameObject teleportGuidePrefab;

    [Header("Teleport Visual Ghost")]
    [SerializeField] private float ghostAlpha = 0.3f;

    public float shadowAlphaMax = GhostSettings.shadowAlpha;

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
        teleportGhost.SetActive(IsApprenticeDifficulty() && GhostSettings.enableGhost);

    }

    private void Update()
    {
        if (isPaused) return;

        // Cooldown ready cue (fires once when lockout ends)
        if (!cooldownReadyPlayed && Time.time >= lastTeleportTime + teleportCooldown)
        {
            // --- SFX at the camera ---
            if (cooldownReadySfx != null)
            {
                if (cameraSfxSource != null)
                {
                    // Best: uses your mixer routing & 2D/3D settings on that source
                    cameraSfxSource.PlayOneShot(cooldownReadySfx, cooldownReadyVolume);
                }
                else
                {
                    // Fallback: play at camera position
                    var cam = Camera.main;
                    var pos = cam ? cam.transform.position : transform.position;
                    AudioSource.PlayClipAtPoint(cooldownReadySfx, pos, cooldownReadyVolume);
                }
            }

            // --- VFX that follows the player (flip by facing/gravity, stable size) ---
            if (cooldownReadyVfx != null)
            {
                // Determine facing (+1 right / -1 left) and gravity (+1 normal / -1 inverted)
                float signX = Mathf.Sign(transform.localScale.x);
                float signY = Mathf.Sign(transform.localScale.y);

                // Flip the offset by facing/gravity so the star appears by the eyes correctly
                Vector3 flippedOffset = new Vector3(
                    cooldownReadyVfxOffset.x * signX,
                    cooldownReadyVfxOffset.y * signY,
                    cooldownReadyVfxOffset.z
                );

                // Spawn
                var fx = Instantiate(cooldownReadyVfx, transform.position + flippedOffset, Quaternion.identity);

                // Follow the player but neutralize parent scale so size stays as in prefab
                fx.transform.SetParent(transform, worldPositionStays: true);
                fx.transform.localScale = Vector3.one;

                // Make sure particle size/space isn't affected by parent transforms
                var systems = fx.GetComponentsInChildren<ParticleSystem>(true);
                for (int i = 0; i < systems.Length; i++)
                {
                    var main = systems[i].main;
                    main.scalingMode = ParticleSystemScalingMode.Local;     // ignore parent scale
                    main.simulationSpace = ParticleSystemSimulationSpace.World; // keep motion stable if you move
                    systems[i].Clear(true);
                    systems[i].Play(true);
                }

                // Optional: ensure it renders on top
                var rends = fx.GetComponentsInChildren<Renderer>(true);
                foreach (var r in rends) { r.sortingLayerName = "VFX"; r.sortingOrder = 200; }

                // Cleanup
                float maxLife = 0.6f;
                foreach (var ps in systems)
                {
                    var m = ps.main;
                    float dur = m.duration;
                    float life = (m.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
                        ? m.startLifetime.constantMax
                        : m.startLifetime.constant;
                    maxLife = Mathf.Max(maxLife, dur + life);
                }
                Destroy(fx, maxLife);
            }



            cooldownReadyPlayed = true;
        }


        if (KeyBindings.GetKeyDown(ActionKey.ToggleGhost))
        {
            if (teleportGhost != null)
            {
                SetGhostVisibility(!teleportGhost.activeSelf);
            }
        }

        // Attempt teleport/dash (shared cooldown)
        if (Time.time >= lastTeleportTime + teleportCooldown && KeyBindings.GetKeyDown(ActionKey.Teleport))
        {
            if (powerUps.hasDash)
            {
                powerUps.PerformDash();
                StartCooldown(); // dash uses the same lockout ping
            }
            else
            {
                PerformTeleport();
                StartCooldown(); // we’ll remove the old lastTeleportTime set inside PerformTeleport
            }
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

            ApplyGhostColorByDifficulty(targetPosition);


            // Safety check
            /* bool isSafe = IsTeleportTargetSafe(teleportGhost.transform.position);

             Color ghostColor = GhostSettings.ghostColor;

             if (IsApprenticeDifficulty() && GhostSettings.enableTinting)
             {
                 ghostColor = isSafe ? GhostSettings.safeColor : GhostSettings.unsafeColor;
             }

             ghostColor.a = GhostSettings.ghostAlpha;
             foreach (var sr in teleportGhost.GetComponentsInChildren<SpriteRenderer>())
             {
                 sr.color = ghostColor;
             }*/

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

        // Define which tags are allowed to be killed
        string[] killableTags = { "Enemy", "KeyMimic", "CoinMimic"};

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

    private void StartCooldown()
    {
        lastTeleportTime = Time.time;
        cooldownReadyPlayed = false;
    }


    public bool IsApprenticeDifficulty()
    {
        return SceneManager.GetActiveScene().name.Contains("Apprentice");
    }

    public bool IsAdeptDifficulty()
    {
        return SceneManager.GetActiveScene().name.Contains("Adept");
    }
    public bool IsWizardDifficulty()
    {
        return SceneManager.GetActiveScene().name.Contains("Wizard");
    }

    public bool IsArchmageDifficulty()
    {
        return SceneManager.GetActiveScene().name.Contains("Archmage");
    }

    public void SetPauseState(bool pause)
    {
        isPaused = pause;
    }



    public bool IsTeleportTargetSafe(Vector3 destination)
    {
        // First check for dangerous tagged colliders
        Collider2D[] hits = Physics2D.OverlapCircleAll(destination, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("ObbyCourse") || hit.CompareTag("Door") ||
                hit.CompareTag("DashWall") || hit.CompareTag("Spike") ||
                hit.CompareTag("Laser") || hit.CompareTag("MovingPlatform"))
            {
                return false;
            }
        }

        // Then check if any laser beams pass near the destination
        foreach (LaserTurret turret in FindObjectsOfType<LaserTurret>())
        {
            if (turret == null || turret.firePoint == null) continue;

            Vector2 origin = turret.firePoint.position;
            Vector2 direction = turret.firePoint.right.normalized;
            float maxDistance = 100f;

            RaycastHit2D hitInfo = Physics2D.Raycast(origin, direction, maxDistance);
            Vector2 laserEnd = hitInfo.collider != null ? hitInfo.point : origin + direction * maxDistance;

            // Find the closest point on the laser beam line to the teleport destination
            Vector2 closestPoint = ClosestPointOnLine(origin, laserEnd, destination);

            float proximity = Vector2.Distance(destination, closestPoint);
            if (proximity <= 0.5f) // adjust as needed
            {
                return false;
            }
        }

        return true;
    }

    // Helper function: closest point on line segment from A to B
    private Vector2 ClosestPointOnLine(Vector2 a, Vector2 b, Vector2 point)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(point - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return a + t * ab;
    }

    public void SetGhostVisibility(bool isVisible)
    {
        if (teleportGhost == null) 
            return;

        teleportGhost.SetActive(isVisible);
        GhostSettings.enableGhost = isVisible;
        GhostSettings.SaveSettings();

        // Enable line if Apprentice and ghost visible
        if (teleportGuide != null && TryGetComponent(out LineRenderer line))
        {
            line.enabled = isVisible && IsApprenticeDifficulty();
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
    }

    public void SetGhostVisibilityFromUI(bool isVisible)
    {
        updatingFromUI = true;
        SetGhostVisibility(isVisible);
        updatingFromUI = false;
    }

    public void ApplyGhostColorByDifficulty(Vector3 targetPosition)
    {
        if (teleportGhost == null)
            return;

        foreach (var sr in teleportGhost.GetComponentsInChildren<SpriteRenderer>())
        {
            Color ghostColor;

            if (IsWizardDifficulty() || IsArchmageDifficulty())
            {
                float alpha = GhostSettings.shadowUsesOpacity
                    ? GhostSettings.ghostAlpha * shadowAlphaMax
                    : shadowAlphaMax;


                ghostColor = new Color(0f, 0f, 0f, alpha);
            }

            else
            {
                ghostColor = GhostSettings.ghostColor;

                if ((IsApprenticeDifficulty() || IsAdeptDifficulty()) && GhostSettings.enableTinting)
                {
                    bool isSafe = IsTeleportTargetSafe(targetPosition);
                    ghostColor = isSafe ? GhostSettings.safeColor : GhostSettings.unsafeColor;
                }

                ghostColor.a = GhostSettings.ghostAlpha;
            }

            sr.color = ghostColor;
        }
    }

}