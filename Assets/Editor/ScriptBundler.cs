using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public class ScriptBundler
{
    // Add any folder names (relative to Assets/) that you want to ignore here
    private static readonly string[] foldersToIgnore = new string[]
    {
        "/Assets/TextMesh Pro",
        "/Assets/FlexibleColorPicker"
    };

    [MenuItem("Tools/Bundle All Scripts")]
    public static void BundleScripts()
    {
        string[] scriptPaths = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        string outputPath = EditorUtility.SaveFilePanel("Save Bundled Script File", "", "AllScriptsBundle.txt", "txt");

        if (string.IsNullOrEmpty(outputPath))
        {
            Debug.Log("Save cancelled.");
            return;
        }

        StringBuilder sb = new();

        foreach (string path in scriptPaths)
        {
            string normalizedPath = path.Replace("\\", "/");

            // Skip ignored folders
            bool skip = false;
            foreach (string ignore in foldersToIgnore)
            {
                if (normalizedPath.StartsWith(ignore))
                {
                    skip = true;
                    break;
                }
            }

            if (skip)
                continue;

            string relativePath = "Assets" + normalizedPath.Replace(Application.dataPath, "");
            string fileContent = File.ReadAllText(path);

            sb.AppendLine($"// ==================================================");
            sb.AppendLine($"// File: {relativePath}");
            sb.AppendLine($"// ==================================================\n");
            sb.AppendLine(fileContent);
            sb.AppendLine("\n\n");
        }

        File.WriteAllText(outputPath, sb.ToString());

        Debug.Log($"All scripts bundled and saved to: {outputPath}");
        EditorUtility.RevealInFinder(outputPath);
    }
}
