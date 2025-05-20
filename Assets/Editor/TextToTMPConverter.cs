using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextToTMPConverter : EditorWindow
{
    [MenuItem("Tools/Convert Text to TMP")]
    public static void ConvertAllTextToTMP()
    {
        Text[] texts = FindObjectsOfType<Text>();

        foreach (Text text in texts)
        {
            GameObject go = text.gameObject;

            // Copy settings
            string content = text.text;
            Color color = text.color;
            int fontSize = text.fontSize;
            TextAnchor alignment = text.alignment;

            // Remove old Text component
            DestroyImmediate(text, true);

            // Add TMP
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.color = color;
            tmp.fontSize = fontSize;
            tmp.alignment = ConvertAlignment(alignment);
            tmp.enableAutoSizing = false;

            Debug.Log($"Converted: {go.name}");
        }
    }

    private static TextAlignmentOptions ConvertAlignment(TextAnchor anchor)
    {
        return anchor switch
        {
            TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
            TextAnchor.UpperCenter => TextAlignmentOptions.Top,
            TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
            TextAnchor.MiddleLeft => TextAlignmentOptions.Left,
            TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
            TextAnchor.MiddleRight => TextAlignmentOptions.Right,
            TextAnchor.LowerLeft => TextAlignmentOptions.BottomLeft,
            TextAnchor.LowerCenter => TextAlignmentOptions.Bottom,
            TextAnchor.LowerRight => TextAlignmentOptions.BottomRight,
            _ => TextAlignmentOptions.Center,
        };
    }
}

