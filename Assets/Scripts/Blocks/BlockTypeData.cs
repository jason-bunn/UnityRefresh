
using System.Collections.Generic;
using UnityEngine;
public class BlockTypeData
{
    public BlockType type;
    public Dictionary<int, Vector2Int> faceUVs = new Dictionary<int, Vector2Int>();

    public BlockTypeData(BlockType type)
    {
        this.type = type;
    }

    public void SetFaceUV(int faceIndex, int tileX, int tileY)
    {
        faceUVs[faceIndex] = new Vector2Int(tileX, tileY);
    }

    public Vector2[] GetUVs(int faceIndex, int AtlasSizeInTiles, int atlasPixelSize)
    {
        Vector2Int tile = faceUVs.ContainsKey(faceIndex) ? faceUVs[faceIndex] : new Vector2Int(0, 0);
        tile.y = AtlasSizeInTiles -1 - tile.y; // Invert Y coordinate for correct UV mapping
        float tileSize = 1f / AtlasSizeInTiles;
        float padding = 1f / atlasPixelSize * 0.5f; // Padding for pixel-perfect rendering
        Vector2 uvOffset = new Vector2(tile.x, tile.y) * tileSize;

        return new Vector2[]
        {
            uvOffset + new Vector2(padding, padding),
            uvOffset + new Vector2(padding, tileSize - padding),
            uvOffset + new Vector2(tileSize - padding, tileSize - padding),
            uvOffset + new Vector2(tileSize - padding, padding)
        };
    }
}
