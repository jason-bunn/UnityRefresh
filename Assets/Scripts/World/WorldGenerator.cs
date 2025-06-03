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
                // Here you would typically instantiate a GameObject to represent the chunk in the scene
                // For example: Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
                GameObject chunkGO = new GameObject($"Chunk_{x}_{z}");
                chunkGO.transform.position = new Vector3(x * Chunk.ChunkSize, 0, z * Chunk.ChunkSize);
                chunkGO.AddComponent<ChunkRenderer>().Initialize(chunk); // Assuming you have a ChunkRenderer script to handle rendering
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
