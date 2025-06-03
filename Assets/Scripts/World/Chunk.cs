using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public const int ChunkSize = 16;
    public BlockType[,,] blocks = new BlockType[ChunkSize, ChunkSize, ChunkSize];

    public Vector3Int position;

    public Chunk(Vector3Int position)
    {
        this.position = position;
        InitializeBlocks();
    }

    void InitializeBlocks()
    {
        float scale = 0.1f;
        float heightMultiplier = 8f;
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {

                for (int z = 0; z < ChunkSize; z++)
                {
                    int height = Mathf.FloorToInt(Mathf.PerlinNoise((position.x + x) * scale, (position.z + z) * scale) * heightMultiplier);
                    //blocks[x, y, z] = y < height ? BlockType.Dirt : BlockType.Air;
                    if (y == height)
                    {
                        blocks[x, y, z] = BlockType.Grass; // Top layer
                    }
                    else if (y == 0)
                    {
                        blocks[x, y, z] = BlockType.Stone; // Bedrock layer
                    }
                    else if (y < height && y > 0)
                    {
                        blocks[x, y, z] = BlockType.Dirt; // Below the grass
                    }
                    else
                    {
                        blocks[x, y, z] = BlockType.Air; // Above the ground
                    }
                }
            }
        }
    }
    public void GenerateChunkData()
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                for (int z = 0; z < ChunkSize; z++)
                {
                    if (y == 0)
                        blocks[x, y, z] = BlockType.Stone;
                    else if (y == 1)
                        blocks[x, y, z] = BlockType.Dirt;
                    else if (y == 2)
                        blocks[x, y, z] = BlockType.Grass;
                    else
                        blocks[x, y, z] = BlockType.Air;
                }
            }
        }
    }
}
