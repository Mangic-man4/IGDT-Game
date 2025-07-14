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
        float x = Mathf.Clamp(player.transform.position.x, xMin, xMax);
        float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
        if (VerticalModeManager.IsVertical)
        {
            // Let the camera follow the player upward in vertical mode
            transform.position = new Vector3(xMin, y, transform.position.z); // Lock X if needed
        }
        else
        {
            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}
