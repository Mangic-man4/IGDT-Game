using System;
using System.Reflection;
using UnityEngine;
/*
public class BossHealthObserver : MonoBehaviour, BossHealthBarHUD.IHealth
{
    [Header("Source (your real boss health component)")]
    [SerializeField] private MonoBehaviour healthComponent;

    [Tooltip("Field or property name for current HP on the source (e.g., 'currentHealth' or 'CurrentHealth')")]
    [SerializeField] private string currentMember = "currentHealth";

    [Tooltip("Field or property name for max HP on the source (e.g., 'maxHealth' or 'MaxHealth')")]
    [SerializeField] private string maxMember = "maxHealth";

    [Header("Display")]
    [SerializeField] private string displayName = "Mimic Boss";

    public string DisplayName => displayName;

    // IHealth: events the HUD listens to
    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    // IHealth: values the HUD may read
    public float Current => _current;
    public float Max => _max;

    float _current, _max;
    bool _deadNotified;

    Func<float> _getCurrent;
    Func<float> _getMax;

    void Awake()
    {
        if (healthComponent == null)
            healthComponent = GetComponent<MonoBehaviour>(); // best-effort fallback

        _getCurrent = BuildGetter(healthComponent, currentMember);
        _getMax = BuildGetter(healthComponent, maxMember);

        // Prime values so HUD starts correct
        ReadAndNotify(force: true);
    }

    void Update()
    {
        ReadAndNotify();
    }

    void ReadAndNotify(bool force = false)
    {
        if (_getCurrent == null || _getMax == null) return;

        float cur = _getCurrent();
        float max = Mathf.Max(1f, _getMax()); // guard

        bool changed = force || !Mathf.Approximately(cur, _current) || !Mathf.Approximately(max, _max);
        _current = cur;
        _max = max;

        if (changed)
            OnHealthChanged?.Invoke(_current, _max);

        if (!_deadNotified && _current <= 0f)
        {
            _deadNotified = true;
            OnDied?.Invoke();
        }
    }

    // Builds a fast float getter for a field or property on the component
    static Func<float> BuildGetter(MonoBehaviour comp, string member)
    {
        if (comp == null || string.IsNullOrEmpty(member)) return null;

        var t = comp.GetType();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // Property first
        var p = t.GetProperty(member, flags);
        if (p != null && p.CanRead && p.PropertyType == typeof(float))
            return () => (float)p.GetValue(comp, null);

        // Then field
        var f = t.GetField(member, flags);
        if (f != null && f.FieldType == typeof(float))
            return () => (float)f.GetValue(comp);

        Debug.LogError($"BossHealthObserver: Could not find float member '{member}' on {t.Name}.");
        return null;
    }
}*/
