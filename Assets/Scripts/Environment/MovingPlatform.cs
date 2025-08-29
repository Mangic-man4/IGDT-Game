using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Path")]
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float speed = 1.0f;

    private Quaternion startRotation;

    [Header("Behaviour")]
    [Tooltip("If ON, the platform stays still until the player touches it once.")]
    [SerializeField] private bool startOnFirstTouch = false;
    [Tooltip("If ON, the platform can be started by triggers in addition to collision.")]
    [SerializeField] private bool triggerCheck = false;
    [Tooltip("If ON, the platform stops when it reaches the End Position    .")]
    [SerializeField] private bool stopAtEnd = false;
    [Tooltip("If ON, use the Gameobject's position as the start position")]
    [SerializeField] private bool useObjectPosAsStartPos = false;

    private bool movingToEnd = true;
    private bool activated = false;        // set true after first player touch
    private bool hasStopped = false;


    void Start()
    {
        if (useObjectPosAsStartPos)
        {
            startPosition = transform.position;
            //startRotation = transform.rotation;
        }
    }

    void Update()
    {
        //  Pause until activated (if toggle is on)
        if (startOnFirstTouch && !activated || hasStopped)
        {
            return;
        }

        float step = speed * Time.deltaTime;

        if (movingToEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, step);

            if (transform.position == endPosition)
            {
                if (stopAtEnd)
                {
                    hasStopped = true;
                }
                else
                {
                    movingToEnd = false;
                }
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

    public void ResetMovingPlatform()
    {
        //rb.velocity = Vector2.zero;
        //rb.angularVelocity = 0f;

        transform.SetPositionAndRotation(startPosition, startRotation);

        movingToEnd = true;
        activated = false;
        hasStopped = false;

        gameObject.SetActive(true);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (startOnFirstTouch && triggerCheck && !activated)
            {
                activated = true;
            }
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