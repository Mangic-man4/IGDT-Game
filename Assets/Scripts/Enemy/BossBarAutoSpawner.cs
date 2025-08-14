using UnityEngine;
using UnityEngine.UI;

public class BossBarAutoSpawner : MonoBehaviour
{
    [SerializeField] private BossHealthBarHUD bossBarPrefab;
    [SerializeField] private Canvas targetCanvas;

    private BossHealthBarHUD instance;

    void Start()
    {
        if (bossBarPrefab == null) return;

        Canvas canvas = EnsureCanvas(targetCanvas);
        instance = Instantiate(bossBarPrefab, canvas.transform);
        instance.name = "BossBar (HUD)";
    }

    private Canvas EnsureCanvas(Canvas existing)
    {
        if (existing != null) return existing;

        foreach (var c in Resources.FindObjectsOfTypeAll<Canvas>())
            if (c && c.isActiveAndEnabled && c.renderMode == RenderMode.ScreenSpaceOverlay)
                return c;

        var go = new GameObject("UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        return canvas;
    }
}
