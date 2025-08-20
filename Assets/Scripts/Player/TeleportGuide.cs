using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TeleportGuide : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject teleportGhost;

    [Header("Settings")]
    private LineRenderer lineRenderer;
    private TeleportControl teleportControl;

    [SerializeField] private bool matchGhostOpacity = true;
    private readonly float shadowAlphaMax = GhostSettings.shadowAlpha; //  editable in Inspector
    private bool isVisible = true;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false; // Start off
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (player != null)
        {
            teleportControl = player.GetComponent<TeleportControl>();
        }
        GhostSettings.shadowAlpha = shadowAlphaMax;
    }

    void Update()
    {
        //if (player == null || (teleportGhost != null && !teleportGhost.activeSelf))
        if (player == null || teleportGhost == null || !teleportGhost.activeSelf || teleportControl == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        //lineRenderer.enabled = isVisible;

        bool isEasy = teleportControl.IsEasyDifficulty();
        bool isNormal = teleportControl.IsNormalDifficulty();
        bool isHard = teleportControl.IsHardDifficulty();
        bool isExtreme = teleportControl.IsExtremeDifficulty();

        lineRenderer.enabled = isEasy;
        isVisible = teleportGhost.activeSelf;

        Vector3 direction;
        Vector3 targetPosition;

        PlayerPowerUps powerUps = player.GetComponent<PlayerPowerUps>();
        teleportControl = player.GetComponent<TeleportControl>();
        float teleportDistance = teleportControl.teleportDistance;


        if (powerUps != null && powerUps.hasDash)
        {
            direction = player.localScale.x > 0 ? Vector3.right : Vector3.left;
            targetPosition = player.position + direction * powerUps.dashDistance;
        }

        else
        {
            direction = VerticalModeManager.IsVertical
                ? (player.position.x < teleportControl.verticalXThreshold ? Vector3.right : Vector3.left)
                : (player.position.y < -3f ? Vector3.up : Vector3.down);

            targetPosition = player.position + direction * teleportDistance;
        }

        lineRenderer.SetPosition(0, player.position);
        lineRenderer.SetPosition(1, targetPosition);

        // Determine the base guide color
        Color guideColor = GhostSettings.ghostColor;

        if ((isEasy || isNormal) && GhostSettings.enableTinting && PauseManager.Instance != null)
        {
            bool isSafe = teleportControl.IsTeleportTargetSafe(targetPosition);
            guideColor = isSafe ? GhostSettings.safeColor : GhostSettings.unsafeColor;
        }
        else if (isHard || isExtreme)
        {
            float alpha = matchGhostOpacity ? GhostSettings.ghostAlpha : shadowAlphaMax;
            guideColor = new Color(0f, 0f, 0f, alpha);
        }


        if (isHard || isExtreme)
        {
            guideColor.a = GhostSettings.shadowUsesOpacity
                ? GhostSettings.ghostAlpha * shadowAlphaMax
                : shadowAlphaMax;
        }
        else if (matchGhostOpacity)
        {
            guideColor.a = GhostSettings.ghostAlpha;
        }


        lineRenderer.startColor = guideColor;
        lineRenderer.endColor = guideColor;

        /*// === Match ghost tinting ===
        Color guideColor = GhostSettings.ghostColor;

        if (GhostSettings.enableTinting && PauseManager.Instance != null && teleportControl.IsEasyDifficulty())
        {
            bool isSafe = teleportControl.IsTeleportTargetSafe(targetPosition);
            guideColor = isSafe ? GhostSettings.safeColor : GhostSettings.unsafeColor;
        }


        guideColor.a = GhostSettings.applyOpacityToGuide ? GhostSettings.ghostAlpha : 1f;
        lineRenderer.startColor = guideColor;
        lineRenderer.endColor = guideColor;
        */
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        lineRenderer.enabled = isVisible;
    }

    public void SetVisible(bool show)
    {
        isVisible = show;
        lineRenderer.enabled = show && teleportControl != null && teleportControl.IsEasyDifficulty();
        /*isVisible = value;
        lineRenderer.enabled = value;*/
    }

}
