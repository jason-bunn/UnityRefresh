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
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                int height = Mathf.FloorToInt(Mathf.PerlinNoise((position.x + x) * 0.1f, (position.z + y) * 0.1f) * ChunkSize);
                for (int z = 0; z < ChunkSize; z++)
                {
                    blocks[x, y, z] = y < height ? BlockType.Dirt : BlockType.Air; 
                }
            }
        }
    }
}
