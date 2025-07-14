using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Transform cam;
    private Vector3 previousCamPos;

    [SerializeField] private float parallaxFactor = 0.5f; // 0 = static, 1 = full camera speed

    void Start()
    {
        cam = Camera.main.transform;
        previousCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - previousCamPos;

        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0f);
        previousCamPos = cam.position;
    }
}
