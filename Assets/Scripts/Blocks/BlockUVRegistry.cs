using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public static class BlockUVRegistry
{
    public static readonly Dictionary<BlockType, BlockTypeData> BlockTypes = new Dictionary<BlockType, BlockTypeData>();
    public static int AtlasSizeInTiles = 16; // Assuming a 16x16 texture atlas
    public static int AtlasPixelSize = 256; // Assuming the atlas is 256x256 pixels

    static BlockUVRegistry()
    {
        RegisterBlockType(BlockType.Grass, data =>
        {
            data.SetFaceUV(0, 1, 0); // Back
            data.SetFaceUV(1, 1, 0); // Front
            data.SetFaceUV(2, 1, 0); // Left
            data.SetFaceUV(3, 1, 0); // Right
            data.SetFaceUV(4, 0, 0); // Top
            data.SetFaceUV(5, 2, 0); // Bottom
        });
        RegisterBlockType(BlockType.Dirt, data =>
        {
            for (int i = 0; i < 6; i++)
                data.SetFaceUV(i, 2, 0);
        });

        RegisterBlockType(BlockType.Stone, data =>
        {
            for (int i = 0; i < 6; i++)
                data.SetFaceUV(i, 3, 0);
        });

        RegisterBlockType(BlockType.Water, data =>
        {
            data.SetAnimatedFaceUV(0, new List<Vector2Int> { new Vector2Int(4, 0), new Vector2Int(5, 0) }, 0.5f); // Back
            data.SetAnimatedFaceUV(1, new List<Vector2Int> { new Vector2Int(4, 0), new Vector2Int(5, 0) }, 0.5f); // Front

            // Animate all 6 faces
            for (int face = 0; face < 6; face++)
            {
                data.SetAnimatedFaceUV(face, new List<Vector2Int> { new Vector2Int(4, 0), new Vector2Int(5, 0) }, 0.5f);
            }
        });
        // Add more block types as needed
    }
    public static void RegisterBlockType(BlockType type, System.Action<BlockTypeData> configure)
    {
        if (!BlockTypes.ContainsKey(type))
        {
            var data = new BlockTypeData(type);
            configure(data);
            BlockTypes[type] = data;
        }
    }

    public static Vector2[] GetUVs(BlockType type, int faceIndex)
    {
        if (!BlockTypes.ContainsKey(type))
        {
            Debug.LogWarning($"Block type {type} not registered. Defaulting to (0, 0).");
            return BlockTypes[BlockType.Grass].GetUVs(faceIndex, AtlasSizeInTiles, AtlasPixelSize); // fallback
        }
        return BlockTypes[type].GetUVs(faceIndex, AtlasSizeInTiles, AtlasPixelSize);
    }
}
