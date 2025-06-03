using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public Material debugChunkMaterial;
    private Chunk chunk;
    private static readonly Vector3[] faceVertices = {
        new Vector3(0, 0, 0), new Vector3(0, 1, 0),
        new Vector3(1, 1, 0), new Vector3(1, 0, 0)  // Z- face
    };

    private static readonly int[][] faceTriangles = {
        new int[] { 0, 1, 2, 0, 2, 3 }
    };

    private static readonly Vector3[] faceNormals = {
        Vector3.back, Vector3.forward, Vector3.left,
        Vector3.right, Vector3.up, Vector3.down
    };

    private static readonly Vector3Int[] faceChecks = {
        new Vector3Int(0, 0, -1), new Vector3Int(0, 0, 1),
        new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0)
    };

    public void Initialize(Chunk chunk)
    {
        if (debugChunkMaterial == null)
        {
            debugChunkMaterial = Resources.Load<Material>("Materials/DebugChunkMaterial");
            if (debugChunkMaterial == null)
            {
                Debug.LogError("Debug Chunk Material not found! Please assign a material in the inspector or ensure it exists in Resources/Materials.");
                return;
            }
        }
        this.chunk = chunk;
        // Here you would typically create a mesh for the chunk based on the blocks in it
        // For simplicity, we will just log the chunk position
        Debug.Log($"Initializing Chunk at position: {chunk.position}");

        // Example of how you might start creating a mesh
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // Populate vertices and triangles based on the chunk's blocks
        // This is where you would implement your mesh generation logic
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

                        AddFace(vertices, triangles, uvs, blockPos, i);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        // Assign the mesh to a MeshFilter component
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Optionally add a MeshRenderer with a material
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = debugChunkMaterial;


    }

    private void AddFace(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, Vector3 pos, int dir)
    {
        int vertStart = vertices.Count;

        Vector3[] faceVerts = GetFaceVertices(dir);
        foreach (var v in faceVerts)
            vertices.Add(pos + v);

        triangles.AddRange(new int[] {
            vertStart, vertStart + 1, vertStart + 2,
            vertStart, vertStart + 2, vertStart + 3
        });
        var block = chunk.blocks[(int)pos.x, (int)pos.y, (int)pos.z];
        var numericsUVs = BlockUVs.GetUVs(block, dir);
        var unityUVs = new Vector2[numericsUVs.Length];
        for (int i = 0; i < numericsUVs.Length; i++)
        {
            unityUVs[i] = new Vector2(numericsUVs[i].X, numericsUVs[i].Y);
        }
        uvs.AddRange(unityUVs);
    }

    private bool InBounds(int x, int y, int z)
    {
        return x >= 0 && x < Chunk.ChunkSize &&
               y >= 0 && y < Chunk.ChunkSize &&
               z >= 0 && z < Chunk.ChunkSize;
    }

    private Vector3[] GetFaceVertices(int direction)
    {
        // Z- face, Z+ face, X- face, X+ face, Y+ face, Y- face
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
