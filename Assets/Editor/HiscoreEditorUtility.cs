using UnityEditor;
using UnityEngine;

public static class HiscoreEditorUtility
{
    [MenuItem("Tools/Reset Player Highscores")]
    public static void ResetPlayerHighscores()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Guest");

        string[] levels = { "Level 0", "Level 1", "Level 2", "Level 3" }; // Add all your levels
        string[] difficulties = { "Easy", "Normal", "Hard", "Extreme" };

        foreach (string level in levels)
        {
            foreach (string difficulty in difficulties)
            {
                string key = $"Hiscore_{playerName}_{level} {difficulty}";
                PlayerPrefs.DeleteKey(key);
            }
        }

        PlayerPrefs.Save();

        Debug.Log($"All hiscores for player '{playerName}' have been reset.");
    }

    [MenuItem("Tools/Delete ALL PlayerPrefs")]
    public static void DeleteAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Delete All PlayerPrefs",
            "Are you sure you want to delete all PlayerPrefs?", "Yes", "Cancel"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("All PlayerPrefs have been deleted.");
        }
    }
}
