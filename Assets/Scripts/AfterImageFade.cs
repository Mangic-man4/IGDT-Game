using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AfterImageFade : MonoBehaviour
{
    [SerializeField] private float life = 0.3f;
    private SpriteRenderer sr;
    private float t;

    void Awake() => sr = GetComponent<SpriteRenderer>();

    void Update()
    {
        t += Time.deltaTime;
        float alpha = Mathf.Lerp(0.5f, 0f, t / life);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
        if (t >= life) Destroy(gameObject);
    }
}
