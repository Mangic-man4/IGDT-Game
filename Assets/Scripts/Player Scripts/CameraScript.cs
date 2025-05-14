using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    private GameObject player;
    public float xMin;
    public float yMin;
    public float xMax;
    public float yMax;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float x = player.transform.position.x;
        float y = player.transform.position.y;

        // Clamp based on whether vertical mode is on
        if (VerticalModeManager.IsVertical)
        {
            // In vertical mode, clamp only Y (camera moves up/down freely, X can be fixed)
            y = Mathf.Clamp(y, yMin, yMax);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        else
        {
            // In horizontal mode, clamp both X and Y
            x = Mathf.Clamp(x, xMin, xMax);
            y = Mathf.Clamp(y, yMin, yMax);
            transform.position = new Vector3(x, y, transform.position.z);
        }
    }

}
