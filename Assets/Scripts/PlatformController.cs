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
}

