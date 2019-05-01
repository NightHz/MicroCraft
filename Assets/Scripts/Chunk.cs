using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChunkState : byte
{
    WaitGenerate,
    GenerateIsWorking,
    WaitUpdate,
    UpdateIsWorking,
    Working,
    Destroy
}

public enum DisplayUnitState : byte
{
    WaitUpdate,
    UpdateIsWorking,
    Working,
    Destroy
}

/// <summary>
/// 显示单元，16*16*16大小
/// </summary>
public struct DisplayUnit
{
    // 显示单元属性
    public DisplayUnitState state;
    public int yOffset;
    public GameObject obj;
    public MeshFilter meshFilter;
    public Mesh mesh;

    public void Init(Transform parent, Material material, Chunk chunk, int yOffset)
    {
        state = DisplayUnitState.WaitUpdate;
        this.yOffset = yOffset;
        obj = new GameObject("(" + chunk.Position.x + "," + yOffset + "," + chunk.Position.z + ")");
        obj.transform.parent = parent;
        obj.transform.position = chunk.Position;
        meshFilter = obj.AddComponent<MeshFilter>();
        obj.AddComponent<MeshRenderer>().material = material;
        mesh = null;
    }

    public void Destroy()
    {
        state = DisplayUnitState.Destroy;
        Object.Destroy(obj);
        obj = null;
        mesh = null;
    }
}

/// <summary>
/// 区块，16*64*16大小
/// </summary>
public class Chunk
{
    public static readonly int width = 16;
    public static readonly int aspect = 4;
    public static readonly int height = width * aspect; // 64
    public static readonly Vector3Int centerPosOffset = new Vector3Int(width / 2, 0, width / 2);


    public static int AlignChunkXZ(int xz) { if (xz >= 0) return xz / width * width; else return (xz + 1) / width * width - width; }
    public static int AlignChunkXZ(float xz) { return AlignChunkXZ(Mathf.FloorToInt(xz)); }
    public static int AlignChunkY(int y) { return 0; }
    public static int AlignChunkY(float y) { return 0; }
    public static Vector3Int AlignChunk(Vector3Int p) { return new Vector3Int(AlignChunkXZ(p.x), AlignChunkY(p.y), AlignChunkXZ(p.z)); }
    public static Vector3Int AlignChunk(Vector3 p) { return new Vector3Int(AlignChunkXZ(p.x), AlignChunkY(p.y), AlignChunkXZ(p.z)); }


    // 上层
    World world;

    // 区块属性
    bool active;
    public bool Active
    {
        get { return active; }
        set
        {
            if (value)
            {
                active = true;
                if (displayUnits != null)
                    for (int i = 0; i < aspect; i++)
                        displayUnits[i].meshFilter.mesh = displayUnits[i].mesh;
            }
            else
            {
                active = false;
                if (displayUnits != null)
                    for (int i = 0; i < aspect; i++)
                        displayUnits[i].meshFilter.mesh = null;
            }
        }
    }
    ChunkState state;
    public ChunkState State
    {
        get { return state; }
    }
    Vector3Int position;
    public Vector3Int Position
    {
        get { return position; }
    }

    // 方块属性
    BlockID[,,] blocks;

    // 渲染属性
    DisplayUnit[] displayUnits;

    public Chunk(World world, Vector3Int chunkPosition, bool active = true)
    {
        this.world = world;
        this.active = active;
        state = ChunkState.WaitGenerate;
        position = chunkPosition;
        blocks = null;
        displayUnits = null;
    }

    List<Vector3> _update_vertices;
    List<int> _update_triangles;
    List<Vector2> _update_uvs;
    int _update_i;
    int _update_y;
    /// <summary>
    /// 返回是否完成更新
    /// </summary>
    public bool Update(int workY)
    {
        switch (state)
        {
            case ChunkState.WaitGenerate:
                blocks = new BlockID[width, height, width];
                displayUnits = new DisplayUnit[aspect];
                for (int i = 0; i < aspect; i++)
                {
                    displayUnits[i].Init(world.transform, world.chunkMaterial, this, i * aspect);
                }
                _update_y = 0;
                state = ChunkState.GenerateIsWorking;
                goto case ChunkState.GenerateIsWorking;

            case ChunkState.GenerateIsWorking:
                int y2 = _update_y;
                //Debug.Log("生成方块，开始y:" + y2);
                for (int i = 0; i < workY && y2 < height; i++, y2++)
                {
                    for (int x = 0; x < width; x++)
                        for (int z = 0; z < width; z++)
                            blocks[x, y2, z] = world.worldTerrain.GetBaseBlock(x + position.x, y2 + position.y, z + position.z);
                }
                //Debug.Log("生成方块，结束y:" + y2);
                if (y2 == height)
                {
                    // 完成生成
                    //Debug.Log("方块生成完成，pos:" + position);
                    world.worldTerrain.DecorateChunk(blocks);
                    state = ChunkState.WaitUpdate;
                    return false;
                }
                else
                {
                    _update_y = y2;
                    return false;
                }

            case ChunkState.WaitUpdate:
                _update_i = aspect;
                for (int i = 0; i < aspect; i++)
                {
                    if (displayUnits[i].state == DisplayUnitState.WaitUpdate)
                    {
                        _update_i = i;
                        break;
                    }
                }
                if(_update_i == aspect)
                {
                    // 找不到待更新的显示单元
                    //Debug.Log("更新完成，pos:" + position);
                    state = ChunkState.Working;
                    Active = active;
                    return true;
                }
                _update_vertices = new List<Vector3>();
                _update_triangles = new List<int>();
                _update_uvs = new List<Vector2>();
                _update_y = _update_i * width;
                state = ChunkState.UpdateIsWorking;
                displayUnits[_update_i].state = DisplayUnitState.UpdateIsWorking;
                goto case ChunkState.UpdateIsWorking;

            case ChunkState.UpdateIsWorking:
                int y = _update_y;
                //Debug.Log("生成方块网格，i:" + _update_i + "，开始y:" + y);
                for (int j = 0; j < workY && y < _update_i * width + width; j++, y++)
                {
                    for (int x = 0; x < width; x++)
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
                }
                //Debug.Log("生成方块网格，结束y:" + y);
                if (y == _update_i * width + width)
                {
                    // 完成更新
                    //Debug.Log("完成方块网格，i:" + _update_i);
                    Mesh mesh = new Mesh();
                    mesh.name = "ChunkMesh " + _update_vertices.Count + " verts";
                    mesh.vertices = _update_vertices.ToArray();
                    mesh.triangles = _update_triangles.ToArray();
                    mesh.uv = _update_uvs.ToArray();
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    displayUnits[_update_i].mesh = mesh;
                    _update_vertices = null;
                    _update_triangles = null;
                    _update_uvs = null;
                    // 检查是否还有显示单元需要更新
                    displayUnits[_update_i].state = DisplayUnitState.Working;
                    state = ChunkState.WaitUpdate;
                    return false;
                }
                else
                {
                    _update_y = y;
                    return false;
                }

            default:
                return true;
        }
    }

    public void Destroy()
    {
        world = null;
        state = ChunkState.Destroy;
        blocks = null;
        if (displayUnits != null)
        {
            for (int i = 0; i < aspect; i++)
            {
                displayUnits[i].Destroy();
            }
            displayUnits = null;
        }
    }
    
    /// <summary>
    /// 返回是否设置成功
    /// </summary>
    public bool SetBlock(Vector3Int blockPosition, BlockID id)
    {
        return SetBlock(blockPosition.x, blockPosition.y, blockPosition.z, id);
    }

    /// <summary>
    /// 返回是否设置成功
    /// </summary>
    public bool SetBlock(int blockX, int blockY, int blockZ, BlockID id)
    {
        if (state == ChunkState.Working)
        {
            int x = blockX - position.x;
            int y = blockY - position.y;
            int z = blockZ - position.z;
            if (y < 0 || y >= height)
                return false;
            else if (x < 0 || z < 0 || x >= width || z >= width)
                return false;
            else
            {
                blocks[x, y, z] = id;
                state = ChunkState.WaitUpdate;
                displayUnits[y / width].state = DisplayUnitState.WaitUpdate;
                return true;
            }
        }
        else
            return false;
    }


    bool IsTransparent(int x, int y, int z)
    {
        // 不在区块内
        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= width)
            return true;

        // 判断透明方块
        if (blocks[x, y, z] == BlockID.Air)
            return true;
        else
            return false;
    }
    
    void AddFrontFace(int x, int y, int z, Block block)
    {
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 1); _update_triangles.Add(_update_vertices.Count + 2);
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 2); _update_triangles.Add(_update_vertices.Count + 3);
        _update_vertices.Add(new Vector3(x + 0, y + 0, z + 0));
        _update_vertices.Add(new Vector3(x + 0, y + 1, z + 0));
        _update_vertices.Add(new Vector3(x + 1, y + 1, z + 0));
        _update_vertices.Add(new Vector3(x + 1, y + 0, z + 0));
        _update_uvs.Add(new Vector2(block.frontU, block.frontV2));
        _update_uvs.Add(new Vector2(block.frontU, block.frontV));
        _update_uvs.Add(new Vector2(block.frontU2, block.frontV));
        _update_uvs.Add(new Vector2(block.frontU2, block.frontV2));
    }

    void AddBackFace(int x, int y, int z, Block block)
    {
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 1); _update_triangles.Add(_update_vertices.Count + 2);
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 2); _update_triangles.Add(_update_vertices.Count + 3);
        _update_vertices.Add(new Vector3(x + 0, y + 0, z + 1));
        _update_vertices.Add(new Vector3(x + 1, y + 0, z + 1));
        _update_vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        _update_vertices.Add(new Vector3(x + 0, y + 1, z + 1));
        _update_uvs.Add(new Vector2(block.backU, block.backV2));
        _update_uvs.Add(new Vector2(block.backU2, block.backV2));
        _update_uvs.Add(new Vector2(block.backU2, block.backV));
        _update_uvs.Add(new Vector2(block.backU, block.backV));
    }

    void AddLeftFace(int x, int y, int z, Block block)
    {
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 1); _update_triangles.Add(_update_vertices.Count + 2);
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 2); _update_triangles.Add(_update_vertices.Count + 3);
        _update_vertices.Add(new Vector3(x + 0, y + 0, z + 0));
        _update_vertices.Add(new Vector3(x + 0, y + 0, z + 1));
        _update_vertices.Add(new Vector3(x + 0, y + 1, z + 1));
        _update_vertices.Add(new Vector3(x + 0, y + 1, z + 0));
        _update_uvs.Add(new Vector2(block.leftU, block.leftV2));
        _update_uvs.Add(new Vector2(block.leftU2, block.leftV2));
        _update_uvs.Add(new Vector2(block.leftU2, block.leftV));
        _update_uvs.Add(new Vector2(block.leftU, block.leftV));
    }

    void AddRightFace(int x, int y, int z, Block block)
    {
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 1); _update_triangles.Add(_update_vertices.Count + 2);
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 2); _update_triangles.Add(_update_vertices.Count + 3);
        _update_vertices.Add(new Vector3(x + 1, y + 0, z + 0));
        _update_vertices.Add(new Vector3(x + 1, y + 1, z + 0));
        _update_vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        _update_vertices.Add(new Vector3(x + 1, y + 0, z + 1));
        _update_uvs.Add(new Vector2(block.rightU, block.rightV2));
        _update_uvs.Add(new Vector2(block.rightU, block.rightV));
        _update_uvs.Add(new Vector2(block.rightU2, block.rightV));
        _update_uvs.Add(new Vector2(block.rightU2, block.rightV2));
    }

    void AddTopFace(int x, int y, int z, Block block)
    {
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 1); _update_triangles.Add(_update_vertices.Count + 2);
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 2); _update_triangles.Add(_update_vertices.Count + 3);
        _update_vertices.Add(new Vector3(x + 0, y + 1, z + 0));
        _update_vertices.Add(new Vector3(x + 0, y + 1, z + 1));
        _update_vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        _update_vertices.Add(new Vector3(x + 1, y + 1, z + 0));
        _update_uvs.Add(new Vector2(block.topU, block.topV2));
        _update_uvs.Add(new Vector2(block.topU, block.topV));
        _update_uvs.Add(new Vector2(block.topU2, block.topV));
        _update_uvs.Add(new Vector2(block.topU2, block.topV2));
    }

    void AddBottomFace(int x, int y, int z, Block block)
    {
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 1); _update_triangles.Add(_update_vertices.Count + 2);
        _update_triangles.Add(_update_vertices.Count + 0); _update_triangles.Add(_update_vertices.Count + 2); _update_triangles.Add(_update_vertices.Count + 3);
        _update_vertices.Add(new Vector3(x + 0, y + 0, z + 0));
        _update_vertices.Add(new Vector3(x + 1, y + 0, z + 0));
        _update_vertices.Add(new Vector3(x + 1, y + 0, z + 1));
        _update_vertices.Add(new Vector3(x + 0, y + 0, z + 1));
        _update_uvs.Add(new Vector2(block.bottomU2, block.bottomV2));
        _update_uvs.Add(new Vector2(block.bottomU2, block.bottomV));
        _update_uvs.Add(new Vector2(block.bottomU, block.bottomV));
        _update_uvs.Add(new Vector2(block.bottomU, block.bottomV2));
    }
}
