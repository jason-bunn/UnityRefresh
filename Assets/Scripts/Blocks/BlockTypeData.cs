
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class BlockTypeData
{
    public BlockType type;
    public Dictionary<int, Vector2Int> staticFaceUVs = new Dictionary<int, Vector2Int>();
    public Dictionary<int, AnimatedTile> animatedFaceUVs = new Dictionary<int, AnimatedTile>();

    public BlockTypeData(BlockType type)
    {
        this.type = type;
    }

    public void SetFaceUV(int faceIndex, int tileX, int tileY)
    {
        staticFaceUVs[faceIndex] = new Vector2Int(tileX, tileY);
    }

    public void SetAnimatedFaceUV(int faceIndex, List<Vector2Int> frames, float frameDuration)
    {
        if (frames == null || frames.Count == 0)
        {
            Debug.LogWarning($"No frames provided for animated face UVs for block type {type} on face {faceIndex}. Animation will not be set.");
            return;
        }

        animatedFaceUVs[faceIndex] = new AnimatedTile(frames, frameDuration);
    }

    public Vector2[] GetUVs(int faceIndex, int AtlasSizeInTiles, int atlasPixelSize)
    {
        Vector2Int tile = staticFaceUVs.ContainsKey(faceIndex) ? staticFaceUVs[faceIndex] : new Vector2Int(0, 0);

        if (animatedFaceUVs.TryGetValue(faceIndex, out AnimatedTile animated))
        {
            int frameIndex = Mathf.FloorToInt(Time.time / animated.frameDuration) % animated.frames.Count;
            tile = animated.frames[frameIndex];
        }
        else if (staticFaceUVs.TryGetValue(faceIndex, out tile))
        {
            // Already assigned
        }
        else
        {
            tile = new Vector2Int(0, 0); // fallback
        }

        tile.y = AtlasSizeInTiles - 1 - tile.y; // Invert Y coordinate for correct UV mapping
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
    public class AnimatedTile
    {
        public List<Vector2Int> frames;
        public float frameDuration;

        public AnimatedTile(List<Vector2Int> frames, float frameDuration)
        {
            this.frames = frames;
            this.frameDuration = frameDuration;
        }

    }
}


