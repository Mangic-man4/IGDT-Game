using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageBoxUI : MonoBehaviour
{
    public GameObject messagePanel;
    public TMP_Text messageText;
    public Button okButton;

    public bool IsShowing => messagePanel.activeSelf;

    void Start()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
            okButton.onClick.AddListener(HideMessage);
        }
    }

    public void ShowMessage(string message)
    {
        if (messagePanel != null && messageText != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
        }
    }

    public void HideMessage()
    {
        messagePanel.SetActive(false);
    }
}

