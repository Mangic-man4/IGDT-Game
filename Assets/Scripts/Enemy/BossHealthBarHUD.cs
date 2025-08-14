using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossHealthBarHUD : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text nameText;
    [SerializeField, Tooltip("Speed at which the health bar fades in/out (CanvasGroup alpha). Higher = faster fade, lower = smoother/slower fade.")]
    private float fadeSpeed = 8f;
    [SerializeField, Tooltip("Speed at which the health bar fill visually updates toward the target health. Higher = faster update, lower = slower/smoother fill.")]
    private float lerpSpeed = 10f;
    [SerializeField] private bool healthLog = false;

    private float target01 = 0f;
    private float current01 = 0f;
    private bool visible;

    private MimicBossController boss;

    public float FadeSpeed => fadeSpeed;
    public float CanvasAlpha => group != null ? group.alpha : 0f;

    void Awake()
    {
        if (group) { group.alpha = 0f; group.interactable = false; group.blocksRaycasts = false; }
        if (fill) fill.fillAmount = 0f;
    }

    void Start()
    {
        boss = FindObjectOfType<MimicBossController>();
        if (boss != null)
        {
            Show();
            if (nameText) nameText.text = "Mimic Boss"; // optional override
        }
        else
        {
            Debug.LogWarning("[BossHealthBarHUD] No MimicBossController found.");
            Hide();
        }
    }

    void Update()
    {
        if (boss == null) return;

        float max = boss.MaxHealth;
        float cur = boss.CurrentHealth;
        target01 = (max <= 0f) ? 0f : Mathf.Clamp01(cur / max);

        current01 = Mathf.MoveTowards(current01, target01, lerpSpeed * Time.deltaTime);
        if (fill) fill.fillAmount = current01;

        float targetAlpha = visible ? 1f : 0f;
        if (group) group.alpha = Mathf.MoveTowards(group.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        if (healthLog) 
        {
            Debug.Log($"HUD: Boss HP {boss.CurrentHealth} / {boss.MaxHealth} -> {target01} ({fill.fillAmount})");
        }

    }

    public void Show() => visible = true;
    public void Hide(bool autoDestroy = false)
    {
        visible = false;
            
        if (autoDestroy && group != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutAndDestroy(group, fadeSpeed));
        }
    }

    private IEnumerator FadeOutAndDestroy(CanvasGroup cg, float speed)
    {
        while (cg.alpha > 0f)
        {
            cg.alpha -= Time.unscaledDeltaTime * speed;
            yield return null;
        }

        Destroy(gameObject);
    }
    public void ForceEmpty()
    {
        target01 = 0f;
        current01 = 0f;
        if (fill) fill.fillAmount = 0f;
    }
}
