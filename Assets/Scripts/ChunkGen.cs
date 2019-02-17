using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 区块生成器，只能被实例化一次
/// 提交到此处的区块会分配到多帧生成
/// </summary>
public class ChunkGen : MonoBehaviour
{
    public static int width = 16;
    public static int height = 32;

    static ChunkGen chunkGen = null;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;
    
    Queue<Chunk> chunks = new Queue<Chunk>();
    Chunk chunk;

    private void Start()
    {
        if (chunkGen == null)
            chunkGen = this;
        else
            Debug.LogError("ChunkGen同一时间被加载了两次");
        StartCoroutine(Generate());
    }

    private void OnDestroy()
    {
        chunkGen = null;
    }

    public static void StartGenerate(Chunk chunk)
    {
        chunkGen.chunks.Enqueue(chunk);
    }

    public static int AlignChunkXZ(int xz) { return xz / width * width; }
    public static int AlignChunkXZ(float xz) { return Mathf.FloorToInt(xz) / width * width; }
    public static int AlignChunkY(int y) { return y / height * height; }
    public static int AlignChunkY(float y) { return Mathf.FloorToInt(y) / height * height; }
    public static Vector3Int AlignChunk(Vector3Int p) { return new Vector3Int(AlignChunkXZ(p.x), AlignChunkY(p.y), AlignChunkXZ(p.z)); }
    public static Vector3Int AlignChunk(Vector3 p) { return new Vector3Int(AlignChunkXZ(p.x), AlignChunkY(p.y), AlignChunkXZ(p.z)); }

    IEnumerator Generate()
    {
        while (true)
        {
            if (chunks.Count == 0)
                yield return null;
            else
            {
                chunk = chunks.Dequeue();

                // GenerateChunk
                chunk.blocks = new BlockID[width, height, width];
                for (int x = 0; x < width; x++)
                    for (int z = 0; z < width; z++)
                        for (int y = 0; y < height; y++)
                            chunk.blocks[x, y, z] = WorldTerrain.GetBlock(x + chunk.position.x, y + chunk.position.y, z + chunk.position.z);
                yield return null;

                // GenerateMesh
                mesh = new Mesh();
                vertices = new List<Vector3>();
                triangles = new List<int>();
                uvs = new List<Vector2>();
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                        for (int z = 0; z < width; z++)
                        {
                            // 如果此方块透明，不会提供面
                            if (IsTransparent(x, y, z))
                                continue;
                            // 判断相邻6个方块是否透明，若透明需要产生面
                            if (IsTransparent(x, y, z - 1)) AddFrontFace(x, y, z, BlockList.GetBlock(chunk.blocks[x, y, z]));
                            if (IsTransparent(x, y, z + 1)) AddBackFace(x, y, z, BlockList.GetBlock(chunk.blocks[x, y, z]));
                            if (IsTransparent(x - 1, y, z)) AddLeftFace(x, y, z, BlockList.GetBlock(chunk.blocks[x, y, z]));
                            if (IsTransparent(x + 1, y, z)) AddRightFace(x, y, z, BlockList.GetBlock(chunk.blocks[x, y, z]));
                            if (IsTransparent(x, y - 1, z)) AddBottomFace(x, y, z, BlockList.GetBlock(chunk.blocks[x, y, z]));
                            if (IsTransparent(x, y + 1, z)) AddTopFace(x, y, z, BlockList.GetBlock(chunk.blocks[x, y, z]));
                        }
                    if (x % 3 == 2)
                        yield return null;
                }
                mesh.name = "ChunkMesh " + vertices.Count + " verts";
                chunk.name += " " + mesh.name;
                mesh.vertices = vertices.ToArray();
                mesh.triangles = triangles.ToArray();
                mesh.uv = uvs.ToArray();
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                chunk.GetComponent<MeshFilter>().mesh = mesh;
                chunk.GetComponent<MeshCollider>().sharedMesh = mesh;
                mesh = null;
                vertices = null;
                triangles = null;
                uvs = null;

                Debug.Log("完成生成:" + chunk.name);
                chunk.isFinished = true;
                chunk = null;
                yield return null;
            }
        }
    }

    bool IsTransparent(int x, int y, int z)
    {
        // 不在区块内
        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= width)
            return true;

        // 判断透明方块
        if (chunk.blocks[x, y, z] == BlockID.Air)
            return true;
        else
            return false;
    }


    void AddFrontFace(int x, int y, int z, Block block)
    {
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 1); triangles.Add(vertices.Count + 2);
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 2); triangles.Add(vertices.Count + 3);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0));
        vertices.Add(new Vector3(x + 0, y + 1, z + 0));
        vertices.Add(new Vector3(x + 1, y + 1, z + 0));
        vertices.Add(new Vector3(x + 1, y + 0, z + 0));
        uvs.Add(new Vector2(block.frontU, block.frontV2));
        uvs.Add(new Vector2(block.frontU, block.frontV));
        uvs.Add(new Vector2(block.frontU2, block.frontV));
        uvs.Add(new Vector2(block.frontU2, block.frontV2));
    }

    void AddBackFace(int x, int y, int z, Block block)
    {
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 1); triangles.Add(vertices.Count + 2);
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 2); triangles.Add(vertices.Count + 3);
        vertices.Add(new Vector3(x + 0, y + 0, z + 1));
        vertices.Add(new Vector3(x + 1, y + 0, z + 1));
        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        vertices.Add(new Vector3(x + 0, y + 1, z + 1));
        uvs.Add(new Vector2(block.backU, block.backV2));
        uvs.Add(new Vector2(block.backU2, block.backV2));
        uvs.Add(new Vector2(block.backU2, block.backV));
        uvs.Add(new Vector2(block.backU, block.backV));
    }

    void AddLeftFace(int x, int y, int z, Block block)
    {
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 1); triangles.Add(vertices.Count + 2);
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 2); triangles.Add(vertices.Count + 3);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0));
        vertices.Add(new Vector3(x + 0, y + 0, z + 1));
        vertices.Add(new Vector3(x + 0, y + 1, z + 1));
        vertices.Add(new Vector3(x + 0, y + 1, z + 0));
        uvs.Add(new Vector2(block.leftU, block.leftV2));
        uvs.Add(new Vector2(block.leftU2, block.leftV2));
        uvs.Add(new Vector2(block.leftU2, block.leftV));
        uvs.Add(new Vector2(block.leftU, block.leftV));
    }

    void AddRightFace(int x, int y, int z, Block block)
    {
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 1); triangles.Add(vertices.Count + 2);
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 2); triangles.Add(vertices.Count + 3);
        vertices.Add(new Vector3(x + 1, y + 0, z + 0));
        vertices.Add(new Vector3(x + 1, y + 1, z + 0));
        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        vertices.Add(new Vector3(x + 1, y + 0, z + 1));
        uvs.Add(new Vector2(block.rightU, block.rightV2));
        uvs.Add(new Vector2(block.rightU, block.rightV));
        uvs.Add(new Vector2(block.rightU2, block.rightV));
        uvs.Add(new Vector2(block.rightU2, block.rightV2));
    }

    void AddTopFace(int x, int y, int z, Block block)
    {
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 1); triangles.Add(vertices.Count + 2);
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 2); triangles.Add(vertices.Count + 3);
        vertices.Add(new Vector3(x + 0, y + 1, z + 0));
        vertices.Add(new Vector3(x + 0, y + 1, z + 1));
        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        vertices.Add(new Vector3(x + 1, y + 1, z + 0));
        uvs.Add(new Vector2(block.topU, block.topV2));
        uvs.Add(new Vector2(block.topU, block.topV));
        uvs.Add(new Vector2(block.topU2, block.topV));
        uvs.Add(new Vector2(block.topU2, block.topV2));
    }

    void AddBottomFace(int x, int y, int z, Block block)
    {
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 1); triangles.Add(vertices.Count + 2);
        triangles.Add(vertices.Count + 0); triangles.Add(vertices.Count + 2); triangles.Add(vertices.Count + 3);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0));
        vertices.Add(new Vector3(x + 1, y + 0, z + 0));
        vertices.Add(new Vector3(x + 1, y + 0, z + 1));
        vertices.Add(new Vector3(x + 0, y + 0, z + 1));
        uvs.Add(new Vector2(block.bottomU2, block.bottomV2));
        uvs.Add(new Vector2(block.bottomU2, block.bottomV));
        uvs.Add(new Vector2(block.bottomU, block.bottomV));
        uvs.Add(new Vector2(block.bottomU, block.bottomV2));
    }
}

