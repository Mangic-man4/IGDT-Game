using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DripSpawner : MonoBehaviour
{
    public GameObject dripPrefab;
    public float interval = 1.5f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnDrop), 0f, interval);
    }

    void SpawnDrop()
    {
        // Rotate so Z+ points down in 2D (i.e., -Y)
        Quaternion dripRotation = Quaternion.Euler(90f, 0f, 0f);

        Instantiate(dripPrefab, transform.position, dripRotation);
    }

}
