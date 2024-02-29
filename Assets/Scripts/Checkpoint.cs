using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isChecked = false;

    public bool IsChecked
    {
        get { return isChecked; }
        set { isChecked = value; }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isChecked)
        {
            isChecked = true;
            GetComponent<SpriteRenderer>().color = Color.green;

            // Update the player's respawn point
            other.GetComponent<PlayerController>().SetRespawnPoint(transform.position);
        }
    }
}
