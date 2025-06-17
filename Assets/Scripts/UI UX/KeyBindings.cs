using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ActionKey 
{ 
    Jump, 
    Teleport, 
    ToggleGhost, 
    Respawn,
    Pause,
    FireballAttack
}

public enum ActionAxis 
{ 
    Horizontal 
}

public class KeyBindings : MonoBehaviour
{
    public static KeyBindings Instance { get; private set; }

    /* default bindings */
    private readonly Dictionary<ActionKey, KeyCode> keyMap = new()
    {
        { ActionKey.Jump,           KeyCode.Space },
        { ActionKey.Teleport,       KeyCode.F     },
        { ActionKey.ToggleGhost,    KeyCode.G     },
        { ActionKey.Respawn,        KeyCode.R     },
        { ActionKey.Pause,          KeyCode.Escape},
        { ActionKey.FireballAttack, KeyCode.E     },



    };

    /* axis keys: positive / negative */
    private readonly Dictionary<ActionAxis, (KeyCode pos, KeyCode neg)> axisMap =
        new()
    {
        { ActionAxis.Horizontal, (KeyCode.D, KeyCode.A) }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadBindings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool GetKey(ActionKey action)
    {
        return Input.GetKey(Instance.keyMap[action]);
    }

    public static bool GetKeyDown(ActionKey action)
    {
        return Input.GetKeyDown(Instance.keyMap[action]);
    }

    public static float GetAxisRaw(ActionAxis axis)
    {
        var (pos, neg) = Instance.axisMap[axis];
        float value = 0f;

        if (Input.GetKey(pos)) value += 1f;
        if (Input.GetKey(neg)) value -= 1f;

        return value;
    }

    public (KeyCode pos, KeyCode neg) GetAxisKeys(ActionAxis axis)
    {
        return axisMap[axis];
    }


    public void RebindKey(ActionKey action, KeyCode newKey)
    {
        if (keyMap.ContainsValue(newKey))
        {
            Debug.LogWarning($"Key '{newKey}' is already assigned to another action.");
            return;
        }

        keyMap[action] = newKey;
        PlayerPrefs.SetInt(action.ToString(), (int)newKey);
        PlayerPrefs.Save();
    }


    public void RebindAxis(ActionAxis axis, KeyCode positive, KeyCode negative)
    {
        // Prevent using the same key for both directions
        if (positive == negative)
        {
            Debug.LogWarning("Cannot assign the same key for both directions of the axis.");
            return;
        }

        // Check if either key is already in use in keyMap
        if (keyMap.ContainsValue(positive) || keyMap.ContainsValue(negative))
        {
            Debug.LogWarning("One or both axis keys are already assigned to another action.");
            return;
        }

        axisMap[axis] = (positive, negative);
        PlayerPrefs.SetInt(axis.ToString() + "_Pos", (int)positive);
        PlayerPrefs.SetInt(axis.ToString() + "_Neg", (int)negative);
        PlayerPrefs.Save();
    }


    private void LoadBindings()
    {
        foreach (ActionKey k in System.Enum.GetValues(typeof(ActionKey)))
        {
            string pKey = k.ToString();
            if (PlayerPrefs.HasKey(pKey))
            {
                keyMap[k] = (KeyCode)PlayerPrefs.GetInt(pKey);
            }
        }

        foreach (ActionAxis ax in System.Enum.GetValues(typeof(ActionAxis)))
        {
            string posKey = ax.ToString() + "_Pos";
            string negKey = ax.ToString() + "_Neg";

            if (PlayerPrefs.HasKey(posKey) && PlayerPrefs.HasKey(negKey))
            {
                KeyCode pos = (KeyCode)PlayerPrefs.GetInt(posKey);
                KeyCode neg = (KeyCode)PlayerPrefs.GetInt(negKey);
                axisMap[ax] = (pos, neg);
            }
        }
    }

    public KeyCode GetBoundKey(ActionKey action)
    {
        return keyMap[action];
    }

    public bool IsKeyInUseAnywhere(KeyCode key, ActionKey? exceptKey = null, ActionAxis? exceptAxis = null, bool? isNegative = null)
    {
        // Check ActionKeys
        foreach (var pair in keyMap)
        {
            if (exceptKey != null && pair.Key == exceptKey.Value)
                continue;

            if (pair.Value == key)
                return true;
        }

        // Check Axis
        foreach (var pair in axisMap)
        {
            if (exceptAxis != null && pair.Key == exceptAxis.Value)
            {
                if (isNegative == true && pair.Value.neg == key)
                    continue;

                if (isNegative == false && pair.Value.pos == key)
                    continue;
            }

            if (pair.Value.pos == key || pair.Value.neg == key)
                return true;
        }

        return false;
    }

}
