using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public int worldSize = 4;
    private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateWorld();
    }


    void GenerateWorld()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int z = 0; z < worldSize; z++)
            {
                Vector3Int chunkPosition = new Vector3Int(x * Chunk.ChunkSize, 0, z * Chunk.ChunkSize);
                Chunk chunk = new Chunk(chunkPosition);
                chunks[chunkPosition] = chunk;

            }
        }

        AddTestPond();


        foreach (var kvp in chunks)
        {
            Vector3Int chunkPosition = kvp.Key;
            Chunk chunk = kvp.Value;

            GameObject chunkGO = new GameObject($"Chunk_{chunkPosition.x}_{chunkPosition.z}");
            chunkGO.transform.position = chunkPosition;
            chunkGO.AddComponent<ChunkRenderer>().Initialize(chunk);
        }
    }

    private Chunk GetChunkAtPosition(Vector3Int position)
    {
        Vector3Int chunkPosition = new Vector3Int(
            Mathf.FloorToInt(position.x / Chunk.ChunkSize) * Chunk.ChunkSize,
            0,
            Mathf.FloorToInt(position.z / Chunk.ChunkSize) * Chunk.ChunkSize
        );

        if (chunks.TryGetValue(chunkPosition, out Chunk chunk))
        {
            return chunk;
        }
        return null;
    }

    private void AddTestPond()
    {
        Debug.Log("Adding Test Pond...");
        int pondCenterX = worldSize * Chunk.ChunkSize / 2;
        int pondCenterZ = worldSize * Chunk.ChunkSize / 2;
        int pondRadius = 5;
        int baseHeight = 4; // Flat base terrain height

        for (int x = -pondRadius; x <= pondRadius; x++)
        {
            for (int z = -pondRadius; z <= pondRadius; z++)
            {
                int distanceSquared = x * x + z * z;
                if (distanceSquared <= pondRadius * pondRadius)
                {
                    int worldX = pondCenterX + x;
                    int worldZ = pondCenterZ + z;

                    Chunk chunk = GetChunkAtPosition(new Vector3Int(worldX, 0, worldZ));
                    if (chunk != null)
                    {
                        int localX = ((worldX % Chunk.ChunkSize) + Chunk.ChunkSize) % Chunk.ChunkSize;
                        int localZ = ((worldZ % Chunk.ChunkSize) + Chunk.ChunkSize) % Chunk.ChunkSize;

                        // Compute distance-based depth variation (deeper near center)
                        float normalizedDistance = Mathf.Sqrt(distanceSquared) / pondRadius;
                        int depth = Mathf.RoundToInt(Mathf.Lerp(3, 1, normalizedDistance)); // 3 blocks deep at center, 1 at edge

                        // Optional: small noise for organic variation
                        depth += Random.Range(0, 2); // add 0 or 1

                        // Clear above pond
                        for (int y = baseHeight + 1; y < Chunk.ChunkSize; y++)
                            chunk.blocks[localX, y, localZ] = BlockType.Air;

                        // Dirt base under water
                        for (int y = 0; y < baseHeight - depth; y++)
                            chunk.blocks[localX, y, localZ] = BlockType.Dirt;

                        // Fill with water up to baseHeight
                        for (int y = baseHeight - depth; y <= baseHeight; y++)
                            chunk.blocks[localX, y, localZ] = BlockType.Water;
                    }
                }
            }
        }
    }
}
