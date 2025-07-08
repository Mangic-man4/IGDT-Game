using TMPro;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class TextToTMPConverter : EditorWindow
{
    [MenuItem("Tools/Convert Text + Dropdown + InputField to TMP")]
    public static void ConvertAllTextDropdownsAndInputsToTMP()
    {
        // Convert all Text components to TMP
        Text[] texts = FindObjectsOfType<Text>(true);
        foreach (Text text in texts)
        {
            GameObject go = text.gameObject;
            string content = text.text;
            Color color = text.color;
            int fontSize = text.fontSize;
            TextAnchor alignment = text.alignment;

            DestroyImmediate(text, true);

            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.color = color;
            tmp.fontSize = fontSize;
            tmp.alignment = ConvertAlignment(alignment);
            tmp.enableAutoSizing = false;

            Debug.Log($"Converted Text to TMP: {go.name}");
        }

        // Convert all Dropdowns to TMP_Dropdown
        Dropdown[] dropdowns = FindObjectsOfType<Dropdown>(true);
        foreach (Dropdown dd in dropdowns)
        {
            GameObject go = dd.gameObject;

            // Cache values
            var options = dd.options;
            int value = dd.value;
            bool interactable = dd.interactable;
            UnityEvent<int> onValueChanged = dd.onValueChanged;

            // Destroy old dropdown
            DestroyImmediate(dd, true);

            // Add TMP Dropdown
            TMP_Dropdown tmpDD = go.AddComponent<TMP_Dropdown>();
            tmpDD.options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();
            foreach (var option in options)
            {
                tmpDD.options.Add(new TMP_Dropdown.OptionData(option.text));
            }

            tmpDD.value = value;
            tmpDD.interactable = interactable;

            // Attempt to find and assign template, label, and item text
            Transform template = go.transform.Find("Template");
            Transform label = go.transform.Find("Label");
            Transform itemText = template?.Find("Viewport/Content/Item/Item Label");

            if (template != null && label != null && itemText != null)
            {
                tmpDD.template = template.GetComponent<RectTransform>();
                tmpDD.captionText = label.GetComponent<TextMeshProUGUI>();
                tmpDD.itemText = itemText.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning($"Missing template structure in: {go.name}. Please verify Template, Label, and Item Text assignments.");
            }

            // Copy over listeners
            for (int i = 0; i < onValueChanged.GetPersistentEventCount(); i++)
            {
                UnityEngine.Object target = onValueChanged.GetPersistentTarget(i);
                string methodName = onValueChanged.GetPersistentMethodName(i);
                if (target != null && !string.IsNullOrEmpty(methodName))
                {
                    UnityAction<int> action = (UnityAction<int>)System.Delegate.CreateDelegate(typeof(UnityAction<int>), target, methodName);
                    tmpDD.onValueChanged.AddListener(action);
                }
            }

            Debug.Log($"Converted Dropdown to TMP_Dropdown: {go.name}");
        }

        // Convert all InputFields to TMP_InputField
        InputField[] inputFields = FindObjectsOfType<InputField>(true);
        foreach (InputField input in inputFields)
        {
            GameObject go = input.gameObject;
            string text = input.text;
            bool interactable = input.interactable;
            UnityEvent<string> onValueChanged = input.onValueChanged;

            DestroyImmediate(input, true);

            TMP_InputField tmpInput = go.AddComponent<TMP_InputField>();
            tmpInput.text = text;
            tmpInput.interactable = interactable;

            // Try to find the Text and Placeholder components
            Transform textComp = go.transform.Find("Text");
            Transform placeholderComp = go.transform.Find("Placeholder");

            if (textComp != null && textComp.TryGetComponent(out TextMeshProUGUI textTMP))
                tmpInput.textComponent = textTMP;

            if (placeholderComp != null && placeholderComp.TryGetComponent(out TextMeshProUGUI placeholderTMP))
                tmpInput.placeholder = placeholderTMP;

            // Copy event listeners
            for (int i = 0; i < onValueChanged.GetPersistentEventCount(); i++)
            {
                UnityEngine.Object target = onValueChanged.GetPersistentTarget(i);
                string methodName = onValueChanged.GetPersistentMethodName(i);
                if (target != null && !string.IsNullOrEmpty(methodName))
                {
                    UnityAction<string> action = (UnityAction<string>)System.Delegate.CreateDelegate(typeof(UnityAction<string>), target, methodName);
                    tmpInput.onValueChanged.AddListener(action);
                }
            }

            Debug.Log($"Converted InputField to TMP_InputField: {go.name}");
        }

        Debug.Log("Conversion complete.");
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