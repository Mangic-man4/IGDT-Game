using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleChildrenOnTrigger : MonoBehaviour
{
    [SerializeField] private bool onlyOnce = false;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (onlyOnce && hasTriggered) return;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }

        hasTriggered = true;
    }
}

