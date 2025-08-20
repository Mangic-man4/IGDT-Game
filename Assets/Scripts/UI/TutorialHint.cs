using UnityEngine;
using TMPro;

public class TutorialHint : MonoBehaviour
{
    [Header("UI Text Component")]
    [SerializeField] private TextMeshProUGUI textField;

    [Header("Tutorial Message Settings")]
    [TextArea]
    [SerializeField] private string actionDescription = "Teleport between the worlds";
    [SerializeField] private ActionKey actionKey = ActionKey.Teleport;
    [SerializeField] private ActionAxis axisKey;
    [SerializeField] private bool useAxis = false;
    [SerializeField] private bool usePositiveAxis = true;

    [Header("Optional Formatting")]
    [SerializeField] private bool useRichText = true;

    private string lastText = "";

    void Start()
    {
        UpdateText();
    }

    void Update()
    {
        if (KeyBindings.Instance != null)
        {
            string currentText = BuildMessage();
            if (currentText != lastText)
            {
                textField.text = currentText;
                lastText = currentText;
            }
        }
    }

    private void UpdateText()
    {
        if (textField == null)
        {
            Debug.LogWarning("TextMeshProUGUI not assigned in TutorialHint.");
            return;
        }

        string fullMessage = BuildMessage();
        textField.text = fullMessage;
        lastText = fullMessage;
    }

    private string BuildMessage()
    {
        string keyText;

        if (useAxis)
        {
            var (pos, neg) = KeyBindings.Instance.GetAxisKeys(axisKey);
            KeyCode key = usePositiveAxis ? pos : neg;
            keyText = FormatKeyCode(key);
        }
        else
        {
            KeyCode boundKey = KeyBindings.Instance.GetBoundKey(actionKey);
            keyText = FormatKeyCode(boundKey);
        }

        if (useRichText)
        {
            keyText = $"<b><color=#FFD700>{keyText}</color></b>";  // bold and gold
        }

        if (!useAxis)
        {
            return $"{actionDescription.ToUpper()} BY PRESSING \"{keyText}\"";
        }
        else
        {
            return $"{actionDescription.ToUpper()} BY HOLDING \"{keyText}\"";
        }
    }

    private string FormatKeyCode(KeyCode key) => key switch
    {
        KeyCode.Space => "SPACE",
        KeyCode.LeftArrow => "←",
        KeyCode.RightArrow => "→",
        KeyCode.UpArrow => "↑",
        KeyCode.DownArrow => "↓",
        KeyCode.LeftShift => "L-SHIFT",
        KeyCode.RightShift => "R-SHIFT",
        KeyCode.LeftAlt => "L-ALT",
        KeyCode.RightAlt => "R-ALT",
        KeyCode.LeftControl => "L-CTRL",
        KeyCode.RightControl => "R-CTRL",
        KeyCode.Escape => "ESC",
        KeyCode.Return => "ENTER",
        KeyCode.Backspace => "BACKSPACE",
        KeyCode.Tab => "TAB",
        KeyCode.Mouse0 => "L-MOUSE",
        KeyCode.Mouse1 => "R-MOUSE",
        KeyCode.Mouse2 => "M-MOUSE",
        KeyCode.Delete => "DEL",
        KeyCode.LeftApple => "WIN KEY",
        KeyCode.None => "",
        _ => key.ToString().ToUpper()
    };
    
}


