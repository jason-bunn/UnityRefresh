using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public Material debugChunkMaterial;
    private Chunk chunk;

    private static readonly Vector3Int[] faceChecks = {
        new Vector3Int(0, 0, -1), new Vector3Int(0, 0, 1),
        new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0)
    };

    public void Initialize(Chunk chunk)
    {
        BlockUVRegistry.RegisterDefaultBlocks();
        if (debugChunkMaterial == null)
        {
            debugChunkMaterial = Resources.Load<Material>("Materials/WorldShader");
            if (debugChunkMaterial == null)
            {
                Debug.LogError("Debug Chunk Material not found! Please assign a material in the inspector or ensure it exists in Resources/Materials.");
                return;
            }
        }

        this.chunk = chunk;
        Debug.Log($"Initializing Chunk at position: {chunk.position}");

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector4> uv2s = new List<Vector4>();

        for (int x = 0; x < Chunk.ChunkSize; x++)
        {
            for (int y = 0; y < Chunk.ChunkSize; y++)
            {
                for (int z = 0; z < Chunk.ChunkSize; z++)
                {
                    var block = chunk.blocks[x, y, z];
                    if (block == BlockType.Air) continue;

                    var blockPos = new Vector3(x, y, z);

                    for (int i = 0; i < faceChecks.Length; i++)
                    {
                        var check = faceChecks[i];
                        int nx = x + check.x;
                        int ny = y + check.y;
                        int nz = z + check.z;

                        if (InBounds(nx, ny, nz) && chunk.blocks[nx, ny, nz] != BlockType.Air)
                            continue;

                        AddFace(vertices, triangles, uvs, uv2s, blockPos, i, block);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.SetUVs(1, uv2s); // assign TEXCOORD1
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = debugChunkMaterial;
    }

    private void AddFace(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector4> uv2s, Vector3 pos, int dir, BlockType block)
    {
        int vertStart = vertices.Count;

        Vector3[] faceVerts = GetFaceVertices(dir);
        foreach (var v in faceVerts)
            vertices.Add(pos + v);

        triangles.AddRange(new int[] {
            vertStart, vertStart + 1, vertStart + 2,
            vertStart, vertStart + 2, vertStart + 3
        });

        var unityUVs = BlockUVRegistry.GetUVs(block, dir);
        Debug.Log($"AddFace: Block={block}, Dir={dir}, UV0={unityUVs[0]}");
        uvs.AddRange(unityUVs);

        Vector4 animationData = BlockUVRegistry.GetAnimatedUVData(block, dir);
        Debug.Log($"AddFace: UV2 = {animationData}");
        for (int i = 0; i < 4; i++)
            uv2s.Add(animationData);
    }

    private bool InBounds(int x, int y, int z)
    {
        return x >= 0 && x < Chunk.ChunkSize &&
               y >= 0 && y < Chunk.ChunkSize &&
               z >= 0 && z < Chunk.ChunkSize;
    }

    private Vector3[] GetFaceVertices(int direction)
    {
        switch (direction)
        {
            case 0: return new[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0) }; // Back
            case 1: return new[] { new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1), new Vector3(0, 0, 1) }; // Front
            case 2: return new[] { new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 0), new Vector3(0, 0, 0) }; // Left
            case 3: return new[] { new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(1, 0, 1) }; // Right
            case 4: return new[] { new Vector3(0, 1, 0), new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0) }; // Top
            case 5: return new[] { new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1) }; // Bottom
            default: return new Vector3[0];
        }
    }
}
