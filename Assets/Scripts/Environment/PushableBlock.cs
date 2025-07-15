using UnityEngine;

public class PushableBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isBeingPushed = false;
    private Vector2 pushDirection;

    [SerializeField] private float pushForce = 2.0f;
    [SerializeField] private float stopThreshold = 0.1f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void ResetBlock()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        isBeingPushed = false;

        transform.SetPositionAndRotation(startPosition, startRotation);

        gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if (isBeingPushed)
        {
            rb.velocity = pushDirection * pushForce;
            if (rb.velocity.magnitude < stopThreshold)
            {
                rb.velocity = Vector2.zero;
                isBeingPushed = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ObbyCourse") || collision.gameObject.CompareTag("Trap"))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            isBeingPushed = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isBeingPushed && collision.gameObject.CompareTag("Player"))
        {
            pushDirection = (transform.position - collision.transform.position).normalized;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isBeingPushed = true;
            pushDirection = (transform.position - collision.transform.position).normalized;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isBeingPushed = false;
            rb.velocity = Vector2.zero;
        }
    }
}
