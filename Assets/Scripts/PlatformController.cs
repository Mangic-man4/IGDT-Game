using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformController : MonoBehaviour
{
    
    public Tilemap platformTilemap;

    // Example method to check if a tile is present at a specific position
    public bool IsTileAtPosition(Vector3Int position)
    {
        return platformTilemap.HasTile(position);
    }

    // Example method to get the tile at a specific position
    public TileBase GetTileAtPosition(Vector3Int position)
    {
        return platformTilemap.GetTile(position);
    }
    /*
    
        public Transform startPosition;
        public Transform endPosition;
        public float speed = 2f;

        private Vector2 targetPosition;

        void Start()
        {
            targetPosition = endPosition.position;
        }

        void Update()
        {
            // Move the platform towards the target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Check if the platform has reached the target position
            if (Vector2.Distance((Vector2)transform.position, targetPosition) < 0.01f)
            {
                // Swap the target position to move the platform in the opposite direction
                targetPosition = (targetPosition == (Vector2)startPosition.position) ? (Vector2)endPosition.position : (Vector2)startPosition.position;
            }
        }*/
    }

