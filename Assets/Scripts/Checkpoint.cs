using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer sR;
    public Sprite activated;

    private bool isChecked = false;

    private void Start() //Gets the sprite rendere component for sprite change
    {
        sR = GetComponent<SpriteRenderer>();
    }

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
            sR.sprite = activated; //Changes the sprite

            // Update the player's respawn point
            other.GetComponent<PlayerController>().SetRespawnPoint(transform.position);
        }
    }
}
