using System.Collections;
using UnityEngine;

public class CrumblePlatform : MonoBehaviour
{
    [Header("Core Settings")]
    public float crumbleDelay = 1.5f;
    public float destroyDelay = 0.5f;

    [Header("Behavior Toggles")]
    public bool enableRegrowth = false;
    public float regrowDelay = 3f;

    public bool randomizeCrumbleDelay = false;
    public float randomRange = 0.5f;

    public bool cancelOnStepOff = true;

    private Coroutine crumbleCoroutine;
    private bool isPlayerOnPlatform = false;
    private bool isCrumbled = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Collider2D col;
    private SpriteRenderer sr;

    void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCrumbled || !collision.gameObject.CompareTag("Player")) return;

        PlayerPowerUps playerPowerUps = collision.gameObject.GetComponent<PlayerPowerUps>();

        foreach (ContactPoint2D contact in collision.contacts)
        {
            bool validLanding = true;

            if (playerPowerUps != null)
            {
                validLanding = playerPowerUps.gravityFlipped
                    ? contact.normal.y > 0.5f
                    : contact.normal.y < -0.5f;
            }

            if (validLanding)
            {
                isPlayerOnPlatform = true;

                if (crumbleCoroutine == null)
                {
                    float actualDelay = crumbleDelay;
                    if (randomizeCrumbleDelay)
                        actualDelay += Random.Range(-randomRange, randomRange);

                    crumbleCoroutine = StartCoroutine(CrumbleWithColorTransition(actualDelay));
                }

                break;
            }
        }
    }

    public void ResetPlatform()
    {
        StopAllCoroutines();
        crumbleCoroutine = null;
        isCrumbled = false;
        isPlayerOnPlatform = false;

        transform.SetPositionAndRotation(startPosition, startRotation);

        col.enabled = true;
        sr.enabled = true;
        sr.color = Color.white;

        gameObject.SetActive(true);
    }


    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        isPlayerOnPlatform = false;

        if (cancelOnStepOff && !isCrumbled && crumbleCoroutine != null)
        {
            StopCoroutine(crumbleCoroutine);
            crumbleCoroutine = null;
            sr.color = Color.white;

            // Add animation trigger here for crumble cancel/reset
            // Example: animator.SetTrigger("ResetCrumble");
        }
    }


    private IEnumerator CrumbleWithColorTransition(float delay)
    {
        float elapsed = 0f;

        Color startColor = new(1f, 0.65f, 0f);
        Color endColor = Color.red;

        sr.color = startColor;

        // Optional: Trigger initial "start crumbling" animation here
        // Example: animator.SetTrigger("StartCrumble");

        while (elapsed < delay)
        {
            if (cancelOnStepOff && !isPlayerOnPlatform)
            {
                sr.color = Color.white;
                crumbleCoroutine = null;

                // Add animation trigger here for crumble interruption
                // Example: animator.SetTrigger("CancelCrumble");

                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / delay);
            sr.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        isCrumbled = true;
        crumbleCoroutine = null;

        // Add animation trigger here for platform break
        // Example: animator.SetTrigger("Break");

        yield return new WaitForSeconds(destroyDelay);

        col.enabled = false;
        sr.enabled = false;

        if (enableRegrowth)
        {
            yield return new WaitForSeconds(regrowDelay);
            col.enabled = true;
            sr.enabled = true;
            sr.color = Color.white;
            isCrumbled = false;

            // Add animation trigger here for regrowth
            // Example: animator.SetTrigger("Regrow");

        }
    }
}
