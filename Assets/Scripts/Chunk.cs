using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// default direction is up
public enum BlockDirection : byte
{
    UP = 0,
    DOWN = 1,
    NORTH = 2,
    SOUTH = 3,
    EAST = 4,
    WEST = 5
}

public class Chunk : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;

    public static int width = 16;
    public static int height = 32;

    [HideInInspector]
    public Vector3Int position;
    [HideInInspector]
    public byte[,,] blocks;
    //public BlockDirection[,,] directions;

    GameObject player;

    private void Start()
    {
        position = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));
        name = "(" + position.x + "," + position.y + "," + position.z + ")";
        GenerateChunk();
        GenerateMesh();
        Debug.Log("完成生成:" + name);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // 距离足够大，进行销毁
        float disX = player.transform.position.x - position.x;
        float disZ = player.transform.position.z - position.z;
        if (disX * disX + disZ * disZ > 10000)
        {
            Debug.Log("销毁区块:(" + position.x + ", " + position.y + ", " + position.z + ")");
            Destroy(gameObject);
        }
    }

    void GenerateChunk()
    {
        blocks = new byte[width, height, width];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < width; z++)
                    if (Random.Range(1, 100) <= 50)
                        blocks[x, y, z] = 0;
                    else
                        blocks[x, y, z] = 2;
    }

    void GenerateMesh()
    {
        mesh = new Mesh();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < width; z++)
                {
                    // 如果此方块透明，不会提供面
                    if (IsTransparent(x, y, z))
                        continue;
                    // 判断相邻6个方块是否透明，若透明需要产生面
                    if (IsTransparent(x, y, z - 1)) AddFrontFace(x, y, z, BlockList.GetBlock(blocks[x, y, z]));
                    if (IsTransparent(x, y, z + 1)) AddBackFace(x, y, z, BlockList.GetBlock(blocks[x, y, z]));
                    if (IsTransparent(x - 1, y, z)) AddLeftFace(x, y, z, BlockList.GetBlock(blocks[x, y, z]));
                    if (IsTransparent(x + 1, y, z)) AddRightFace(x, y, z, BlockList.GetBlock(blocks[x, y, z]));
                    if (IsTransparent(x, y - 1, z)) AddBottomFace(x, y, z, BlockList.GetBlock(blocks[x, y, z]));
                    if (IsTransparent(x, y + 1, z)) AddTopFace(x, y, z, BlockList.GetBlock(blocks[x, y, z]));
                }

        mesh.name = "ChunkMesh " + vertices.Count + " verts";
        name += " " + mesh.name;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        mesh = null;
        vertices = null;
        triangles = null;
        uvs = null;
    }

    bool IsTransparent(int x, int y, int z)
    {
        // 不在区块内
        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= width)
            return true;

        // 判断透明方块
        if (blocks[x, y, z] == 0)
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
