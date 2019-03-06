using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockDirection : byte
{
    UP = 0,
    DOWN = 1,
    NORTH = 2,
    SOUTH = 3,
    EAST = 4,
    WEST = 5
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
/// </summary>
public class Chunk : MonoBehaviour
{
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
}
