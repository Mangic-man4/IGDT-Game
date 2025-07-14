using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsMenu : MonoBehaviour
{
    [System.Serializable]
    public struct RebindItem
    {
        public ActionKey action;
        public TMP_Text labelText;
        public TMP_Text keyText;
        public GameObject buttonObj; // UI button that starts rebinding
    }

    public RebindItem[] items;

    [Header("Axis Rebinding")]
    public TMP_Text leftKeyText;
    public TMP_Text rightKeyText;
    public GameObject leftKeyButton;
    public GameObject rightKeyButton;

    private bool waitingForKey = false;
    private ActionKey pendingAction;

    private bool waitingForAxisKey = false;
    private bool rebindingLeft = false;

    public MessageBoxUI messageBox;

    public bool IsWaitingForKey => waitingForKey || waitingForAxisKey;


    void Start()
    {
        RefreshDisplay();

        foreach (var it in items)
        {
            it.buttonObj.GetComponent<Button>()
                        .onClick.AddListener(() => BeginRebind(it.action));
        }

        if (leftKeyButton != null)
        {
            leftKeyButton.GetComponent<Button>().onClick.AddListener(() => BeginRebindAxis(true));
        }

        if (rightKeyButton != null)
        {
            rightKeyButton.GetComponent<Button>().onClick.AddListener(() => BeginRebindAxis(false));
        }
    }

    void Update()
    {
        if (!waitingForKey && !waitingForAxisKey)
        {
            return;
        }

        foreach (KeyCode kc in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kc))
            {
                if (waitingForKey)
                {
                    if (KeyBindings.Instance.IsKeyInUseAnywhere(kc, exceptKey: pendingAction))
                    {
                        if (messageBox != null)
                        {
                            messageBox.ShowMessage($"Key '{kc}' is already in use! Please choose another key instead!");
                        }
                        waitingForKey = false;
                        return;
                    }

                    KeyBindings.Instance.RebindKey(pendingAction, kc);
                    waitingForKey = false;
                }
                else if (waitingForAxisKey)
                {
                    var (pos, neg) = KeyBindings.Instance.GetAxisKeys(ActionAxis.Horizontal);

                    bool inUse = KeyBindings.Instance.IsKeyInUseAnywhere(
                        kc,
                        exceptAxis: ActionAxis.Horizontal,
                        isNegative: rebindingLeft
                    );

                    if (inUse)
                    {
                        if (messageBox != null)
                        {
                            messageBox.ShowMessage($"Key '{kc}' is already in use! Please choose another key instead!");
                        }

                        waitingForAxisKey = false;
                        return;
                    }

                    if (rebindingLeft)
                    {
                        KeyBindings.Instance.RebindAxis(ActionAxis.Horizontal, pos, kc);
                    }
                    else
                    {
                        KeyBindings.Instance.RebindAxis(ActionAxis.Horizontal, kc, neg);
                    }

                    waitingForAxisKey = false;
                }

                RefreshDisplay();
                break;
            }
        }
    }



    private void BeginRebind(ActionKey action)
    {
        waitingForKey = true;
        pendingAction = action;
    }

    private void BeginRebindAxis(bool isLeft)
    {
        waitingForAxisKey = true;
        rebindingLeft = isLeft;
    }

    private void RefreshDisplay()
    {
        foreach (var it in items)
        {
            it.keyText.text = KeyBindings.Instance.GetBoundKey(it.action).ToString(); 
        }

        RefreshAxisDisplay();
    }

    private void RefreshAxisDisplay()
    {
        var (pos, neg) = KeyBindings.Instance.GetAxisKeys(ActionAxis.Horizontal);

        if (leftKeyText != null)
        {
            leftKeyText.text = neg.ToString();
        }

        if (rightKeyText != null)
        {
            rightKeyText.text = pos.ToString();
        }
    }

    public void RefreshUI()
    {
        RefreshDisplay();
    }

}