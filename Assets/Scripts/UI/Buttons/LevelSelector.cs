using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    [Header("Config")]
    public int level;

    [Header("References")]
    public DifficultySelection difficultySelection;   // preferred
    public TextMeshProUGUI difficultyText;            // fallback if no DifficultySelection

    public void OpenScene()
    {
        // Use the visible difficulty (Apprentice/Adept/Wizard/Archmage)
        string difficulty = null;

        if (difficultySelection != null)
        {
            difficulty = difficultySelection.CurrentDifficulty; // new property
        }
        else if (difficultyText != null)
        {
            difficulty = difficultyText.text; // assume UI already shows the new label
        }
        else
        {
            Debug.LogError("[LevelSelector] No DifficultySelection or difficultyText assigned.");
            return;
        }

        if (string.IsNullOrWhiteSpace(difficulty))
        {
            Debug.LogError("[LevelSelector] Difficulty is empty/null.");
            return;
        }

        string sceneName = $"Level {level} {difficulty}";

        if (!CanLoadScene(sceneName))
        {
            Debug.LogError($"[LevelSelector] Scene not found in Build Settings: \"{sceneName}\"");
            return;
        }

        Debug.Log($"[LevelSelector] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    private bool CanLoadScene(string name)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneFile = System.IO.Path.GetFileNameWithoutExtension(path);
            if (sceneFile == name) return true;
        }
        return false;
    }
}
