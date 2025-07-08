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

    private bool isVisible = true;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false; // Start off
    }

    void Update()
    {
        if (player == null || (teleportGhost != null && !teleportGhost.activeSelf))
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = isVisible;

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

        // === Match ghost tinting ===
        Color guideColor = GhostSettings.ghostColor;

        if (GhostSettings.enableTinting && PauseManager.Instance != null && teleportControl.IsEasyDifficulty()
)
        {
            bool isSafe = teleportControl.IsTeleportTargetSafe(targetPosition);
            guideColor = isSafe ? Color.green : Color.red;
        }

        guideColor.a = 1f; // Keep line fully visible
        lineRenderer.startColor = guideColor;
        lineRenderer.endColor = guideColor;

    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        lineRenderer.enabled = isVisible;
    }

    public void SetVisible(bool value)
    {
        isVisible = value;
        lineRenderer.enabled = value;
    }
}
