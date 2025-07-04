using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSafeArea : MonoBehaviour
{
    [Tooltip("Orthographic size used at 16:9 (e.g. 5)")]
    public float referenceOrthographicSize = 5f;

    [Tooltip("The reference aspect ratio to base scaling on (e.g. 16:9)")]
    public Vector2 referenceAspectRatio = new Vector2(16f, 9f);

    private Camera cam;
    private float lastScreenWidth;
    private float lastScreenHeight;

    void Start()
    {
        cam = GetComponent<Camera>();
        UpdateCameraSize(force: true); // Initial setup
    }

    void Update()
    {
        // Only update if resolution has changed
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateCameraSize();
        }
    }

    private void UpdateCameraSize(bool force = false)
    {
        float currentAspect = (float)Screen.width / Screen.height;
        float referenceAspect = referenceAspectRatio.x / referenceAspectRatio.y;

        cam.orthographicSize = referenceOrthographicSize * (referenceAspect / currentAspect);

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        if (force)
        {
            Debug.Log($"[CameraSafeArea] Adjusted camera for aspect {currentAspect:F2} with ortho size {cam.orthographicSize:F2}");
        }
    }
}
