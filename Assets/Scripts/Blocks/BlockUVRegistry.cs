using System.Collections.Generic;
using UnityEngine;

public static class BlockUVRegistry
{
    public static readonly Dictionary<BlockType, BlockTypeData> BlockTypes = new();
    public static int AtlasSizeInTiles = 16;
    public static int AtlasPixelSize = 256;

    static BlockUVRegistry()
    {

    }

    public static void RegisterDefaultBlocks()
    {
        Debug.Log("Initializing Block UV Registry...");
        RegisterBlockType(BlockType.Grass, data =>
        {
            data.SetFaceUV(0, 1, 0);
            data.SetFaceUV(1, 1, 0);
            data.SetFaceUV(2, 1, 0); // North
            data.SetFaceUV(3, 1, 0); // South   
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
            var animFrames = new List<Vector2Int> { new Vector2Int(3, 0), new Vector2Int(4, 0) };
            for (int i = 0; i < 6; i++)
                data.SetAnimatedFaceUV(i, animFrames, 0.5f); // 0.5s per frame
        });
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
        if (!BlockTypes.TryGetValue(type, out var data))
        {
            Debug.LogWarning($"Block type {type} not registered. Defaulting to Grass.");
            data = BlockTypes[BlockType.Grass];
        }
        return data.GetUVs(faceIndex, AtlasSizeInTiles, AtlasPixelSize);
    }

    public static Vector4 GetAnimatedUVData(BlockType type, int faceIndex)
    {
        if (!BlockTypes.TryGetValue(type, out var data))
        {
            data = BlockTypes[BlockType.Grass]; // Fallback
        }
        return data.GetAnimationData(faceIndex, AtlasSizeInTiles);
    }
}
