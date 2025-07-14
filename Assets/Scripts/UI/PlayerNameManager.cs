using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    private void Start()
    {
        // Load existing name (or default)
        string savedName = PlayerPrefs.GetString("PlayerName", "Guest");
        nameInputField.text = savedName;
    }

    public void OnNameChanged(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            newName = "Guest";

        Debug.Log("New name;" + newName);

        PlayerPrefs.SetString("PlayerName", newName);
        PlayerPrefs.Save();
    }
}
