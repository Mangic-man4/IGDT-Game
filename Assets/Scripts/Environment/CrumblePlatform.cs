using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;


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
    private Material tilemapMaterial;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Collider2D col;
    private SpriteRenderer sr;
    private TilemapRenderer tr;

    void Start()
    {
        col = GetComponent<Collider2D>();
        TryGetComponent(out sr);
        TryGetComponent(out tr);

        if (tr != null)
        {
            tilemapMaterial = tr.material;
        }

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

        if (col != null) col.enabled = true;
        if (sr != null)
        {
            sr.enabled = true;
            sr.color = Color.white;
        }
        if (tr != null) tr.enabled = true;
        if (tilemapMaterial != null)
        {
            tilemapMaterial.color = Color.white;
        }

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
            if (sr != null)
                sr.color = Color.white;

            if (tilemapMaterial != null)
            {
                tilemapMaterial.color = Color.white;
            }
            // Add animation trigger here for crumble cancel/reset
            // Example: animator.SetTrigger("ResetCrumble");
        }
    }


    private IEnumerator CrumbleWithColorTransition(float delay)
    {
        float elapsed = 0f;

        Color startColor = new(1f, 0.65f, 0f);
        Color endColor = Color.red;

        if (sr != null)
            sr.color = startColor;

        if (tilemapMaterial != null)
        {
            tilemapMaterial.color = startColor;
        }

        // Optional: Trigger initial "start crumbling" animation here
        // Example: animator.SetTrigger("StartCrumble");

        while (elapsed < delay)
        {
            if (cancelOnStepOff && !isPlayerOnPlatform)
            {
                if (sr != null)
                    sr.color = Color.white;

                if (tilemapMaterial != null)
                {
                    tilemapMaterial.color = Color.white;
                }

                crumbleCoroutine = null;

                // Add animation trigger here for crumble interruption
                // Example: animator.SetTrigger("CancelCrumble");

                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / delay);
            if (sr != null)
                sr.color = Color.Lerp(startColor, endColor, t);

            if (tilemapMaterial != null)
            {
                tilemapMaterial.color = Color.Lerp(startColor, endColor, t);
            }

            yield return null;
        }

        isCrumbled = true;
        crumbleCoroutine = null;

        // Add animation trigger here for platform break
        // Example: animator.SetTrigger("Break");

        yield return new WaitForSeconds(destroyDelay);

        if (col != null) col.enabled = false;
        if (sr != null) sr.enabled = false;
        if (tr != null) tr.enabled = false;

        if (enableRegrowth)
        {
            yield return new WaitForSeconds(regrowDelay);

            if (col != null) col.enabled = true;
            if (sr != null)
            {
                sr.enabled = true;
                sr.color = Color.white;
            }
            if (tr != null) tr.enabled = true;
            if (tilemapMaterial != null)
            {
                tilemapMaterial.color = Color.white;
            }

            isCrumbled = false;
        }
    }
}
