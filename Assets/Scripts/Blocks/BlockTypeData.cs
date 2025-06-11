using System.Collections.Generic;
using UnityEngine;

public class BlockTypeData
{
    public BlockType type;

    public Dictionary<int, Vector2Int> staticFaceUVs = new();
    public Dictionary<int, AnimatedTile> animatedFaceUVs = new();

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

    public Vector2[] GetUVs(int faceIndex, int atlasSizeInTiles, int atlasPixelSize)
    {
        Vector2Int tile = staticFaceUVs.ContainsKey(faceIndex) ? staticFaceUVs[faceIndex] : new Vector2Int(0, 0);

        if (animatedFaceUVs.TryGetValue(faceIndex, out AnimatedTile animated))
        {
            int frameIndex = Mathf.FloorToInt(Time.time / animated.frameDuration) % animated.frames.Count;
            tile = animated.frames[frameIndex];
        }

        // Flip Y to match UV layout
        tile.y = atlasSizeInTiles - 1 - tile.y;

        float tileSize = 1f / atlasSizeInTiles;
        float padding = 1f / atlasPixelSize * 0.5f;
        Vector2 uvOffset = new Vector2(tile.x, tile.y) * tileSize;

        //Debug.Log($"Block: {type}, Face: {faceIndex}, Tile: {tile}, UVs: {string.Join(", ", uvOffset)}");

        return new Vector2[]
        {
            uvOffset + new Vector2(padding, padding),
            uvOffset + new Vector2(padding, tileSize - padding),
            uvOffset + new Vector2(tileSize - padding, tileSize - padding),
            uvOffset + new Vector2(tileSize - padding, padding)
        };
    }

    /// <summary>
    /// Returns animation data per face in a Vector4:
    /// x = start U, y = start V (normalized), z = frame count, w = frame duration
    /// </summary>
    public Vector4 GetAnimationData(int faceIndex, int atlasSizeInTiles)
    {
        if (animatedFaceUVs.TryGetValue(faceIndex, out AnimatedTile anim) && anim.frames.Count > 0)
        {
            Vector2Int startTile = anim.frames[0];
            startTile.y = atlasSizeInTiles - 1 - startTile.y; // flip Y
            Vector2 uv = new Vector2(startTile.x, startTile.y) / atlasSizeInTiles;

            return new Vector4(uv.x, uv.y, anim.frames.Count, anim.frameDuration);
        }

        return Vector4.zero;
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
