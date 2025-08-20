using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DripSpawner : MonoBehaviour
{
    public GameObject dripPrefab;

    [Header("Fixed Interval")]
    public float interval = 1.5f;

    [Header("Random Interval")]
    public bool useRandomInterval = false;
    public float minInterval = 1f;
    public float maxInterval = 3f;

    private void Start()
    {
        if (useRandomInterval)
        {
            StartCoroutine(SpawnWithRandomInterval());
        }
        else
        {
            InvokeRepeating(nameof(SpawnDrop), 0f, interval);
        }
    }

    void SpawnDrop()
    {
        // Rotate so Z+ points down in 2D (i.e., -Y)
        Quaternion dripRotation = Quaternion.Euler(90f, 0f, 0f);

        Instantiate(dripPrefab, transform.position, dripRotation);
    }
    private IEnumerator SpawnWithRandomInterval()
    {
        while (true)
        {
            SpawnDrop();
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
