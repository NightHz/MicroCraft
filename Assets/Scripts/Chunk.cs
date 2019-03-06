using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockDirection : byte
{
    UP = 0,    // 默认顶面
    DOWN = 1,  // 默认底面
    NORTH = 2, // 默认后面
    SOUTH = 3, // 默认前面
    EAST = 4,  // 默认右面
    WEST = 5   // 默认左面
}

public struct ChunkData
{
    public BlockID[,,] blocks;
    // public BlockDirection[,,] up; // default is up
    // public BlockDirection[,,] front; // default is south
}

public enum ChunkState : byte
{
    WaitGenerate = 0,
    GenerateIsWorking = 1,
    WaitUpdate = 2,
    UpdateIsWorking = 3,
    Working = 4,
    Destroy = 5
}

/// <summary>
/// 区块
/// 上层ChunkManager
/// 1.被实例化后将自身递交给上层生成数据
/// 2.距离过大摧毁自身并报告上层
/// 3.可以设置方块
/// </summary>
public class Chunk : MonoBehaviour
{
    static readonly int width = ChunkManager.width;
    static readonly int height = ChunkManager.height;

    [HideInInspector] public ChunkState state;
    //bool isDirty = false;

    [HideInInspector] public Vector3Int position;
    [HideInInspector] public BlockID[,,] blocks;
    //public BlockDirection[,,] up; // default is up
    //public BlockDirection[,,] front; // default is south

    [HideInInspector] public GameObject player;

    private void Start()
    {
        position = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));
        name = "(" + position.x + "," + position.y + "," + position.z + ")";

        state = ChunkState.WaitGenerate;
        ChunkManager.StartGenerate(this);
    }

    private void Update()
    {
        if (state == ChunkState.GenerateIsWorking || state == ChunkState.UpdateIsWorking)
            return;

        // 距离足够大，进行销毁
        float disX = position.x + ChunkManager.width / 2 - player.transform.position.x;
        float disZ = position.z + ChunkManager.width / 2 - player.transform.position.z;
        if (disX * disX + disZ * disZ > World.minDistanceDes * World.minDistanceDes)
        {
            Destroy(gameObject);
            state = ChunkState.Destroy;
            ChunkManager.DestroyChunk(position);
        }
    }

    public void SetBlock(Vector3Int pos, BlockID id)
    {
        SetBlock(pos.x, pos.y, pos.z, id);
    }

    public void SetBlock(int x, int y, int z, BlockID id)
    {
        if (state == ChunkState.Working)
        {
            if (y < 0 || y >= height)
                // 超出界限
                return;
            else if (x < 0 || z < 0 || x >= width || z >= width)
                // 不在此区块内，递交给上层处理
                ChunkManager.SetBlock(new Vector3Int(x, y, z) + position, id);
            else
            {
                blocks[x, y, z] = id;
                ChunkUpdater.UpdateChunk(this);
            }
        }
    }
}
