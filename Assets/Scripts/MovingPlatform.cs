using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Path")]
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float speed = 1.0f;

    [Header("Behaviour")]
    [Tooltip("If ON, the platform stays still until the player touches it once.")]
    public bool startOnFirstTouch = false;      

    private bool movingToEnd = true;
    private bool activated = false;        // set true after first player touch


    void Update()
    {
        //  Pause until activated (if toggle is on)
        if (startOnFirstTouch && !activated)
        {
            return;
        }

        float step = speed * Time.deltaTime;

        if (movingToEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, step);

            if (transform.position == endPosition)
            {
                movingToEnd = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, step);

            if (transform.position == startPosition)
            {
                movingToEnd = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Activate movement if required
            if (startOnFirstTouch && !activated)
            {
                activated = true;
            }

            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerPowerUps>();

            if (player != null && !player.isDashing && collision.transform.parent != transform)
            {
                collision.transform.SetParent(transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("UnparentFromPlatform", SendMessageOptions.DontRequireReceiver);
        }
    }   
}