using System;
using System.Reflection;
using UnityEngine;
/*
public class BossHealthBridge : MonoBehaviour, BossHealthBarHUD.IHealth
{
    [Header("Source (read-only)")]
    [Tooltip("Component that holds the real boss health fields/properties.")]
    [SerializeField] private Component healthSource;

    [Tooltip("Name of the CURRENT health field or property on the source (e.g., currentHealth, Health, HP).")]
    [SerializeField] private string currentFieldName = "currentHealth";

    [Tooltip("Name of the MAX health field or property on the source (e.g., maxHealth, MaxHealth, MaxHP).")]
    [SerializeField] private string maxFieldName = "maxHealth";

    [Header("Display")]
    [SerializeField] private string displayName = "Mimic Boss";
    public string DisplayName => displayName;

    public float Current => _current;
    public float Max => _max;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    float _current, _max;
    bool _died;

    FieldInfo curField, maxField;
    PropertyInfo curProp, maxProp;

    void Awake()
    {
        // If you forgot to assign, try to find a component with those members on the same GO.
        if (healthSource == null) healthSource = GetComponent<Component>();
        ResolveMembers();
        ForceRefresh(); // prime HUD with correct values
    }

    void Update()
    {
        if (healthSource == null) return;

        float cur = ReadFloat(healthSource, curField, curProp);
        float max = Mathf.Max(0.0001f, ReadFloat(healthSource, maxField, maxProp));

        if (!Mathf.Approximately(cur, _current) || !Mathf.Approximately(max, _max))
        {
            _current = cur;
            _max = max;
            OnHealthChanged?.Invoke(_current, _max);
        }

        if (!_died && _current <= 0f)
        {
            _died = true;
            OnDied?.Invoke();
        }
    }

    void ResolveMembers()
    {
        if (healthSource == null) return;
        var t = healthSource.GetType();

        // current
        curField = t.GetField(currentFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (curField == null)
            curProp = t.GetProperty(currentFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // max
        maxField = t.GetField(maxFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (maxField == null)
            maxProp = t.GetProperty(maxFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (curField == null && curProp == null)
            Debug.LogWarning($"BossHealthBridge: '{currentFieldName}' not found on {t.Name}.");
        if (maxField == null && maxProp == null)
            Debug.LogWarning($"BossHealthBridge: '{maxFieldName}' not found on {t.Name}.");
    }

    float ReadFloat(object src, FieldInfo f, PropertyInfo p)
    {
        object val = null;
        if (f != null) val = f.GetValue(src);
        else if (p != null) val = p.GetValue(src, null);

        if (val == null) return 0f;
        try { return Convert.ToSingle(val); } catch { return 0f; }
    }

    void ForceRefresh()
    {
        if (healthSource == null) return;
        _current = ReadFloat(healthSource, curField, curProp);
        _max = Mathf.Max(0.0001f, ReadFloat(healthSource, maxField, maxProp));
        OnHealthChanged?.Invoke(_current, _max);
        if (_current <= 0f) OnDied?.Invoke();
    }
}
*/