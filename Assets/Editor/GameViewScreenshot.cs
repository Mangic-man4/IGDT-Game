using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;

public class GameViewScreenshot
{
    [MenuItem("Tools/Take Screenshot From Fixed Position %#k")] // Ctrl+Shift+K
    public static void TakeScreenshotFromFixedPosition()
    {
        int width = 1920;
        int height = 1080;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("No Main Camera found.");
            return;
        }

        // Save original camera transform
        Vector3 originalPos = cam.transform.position;
        Quaternion originalRot = cam.transform.rotation;

        // Set custom position and rotation
        Vector3 screenshotPos = new(8.7f, -2.5f, -10f); // <- Horizontal = (8.7f, -2.5f, -10f); Vertical = (144f, 112f, -10f)
        Quaternion screenshotRot = Quaternion.Euler(0f, 0f, 0f); // In general keep at (0f, 0f, 0f)
        cam.transform.SetPositionAndRotation(screenshotPos, screenshotRot);

        // Render to texture
        RenderTexture rt = new(width, height, 24);
        Texture2D screenshot = new(width, height, TextureFormat.RGB24, false);

        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // Reset camera
        cam.targetTexture = null;
        RenderTexture.active = null;
        Object.DestroyImmediate(rt);
        cam.transform.SetPositionAndRotation(originalPos, originalRot);

        // Save screenshot
        string folder = "Assets/Screenshots";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string sceneName = SceneManager.GetActiveScene().name;
        string fileName = $"{sceneName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string path = Path.Combine(folder, fileName);
        File.WriteAllBytes(path, screenshot.EncodeToPNG());
        AssetDatabase.Refresh();

        Debug.Log($"Screenshot taken from fixed position: {path}");
    }
}
