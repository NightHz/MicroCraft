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

/// <summary>
/// 区块
/// </summary>
public class Chunk : MonoBehaviour
{
    [HideInInspector]
    public bool isFinished = false;
    //bool isDirty = false;
    [HideInInspector]
    public Vector3Int position;
    [HideInInspector]
    public BlockID[,,] blocks;
    //public BlockDirection[,,] directions;

    GameObject player;

    private void Start()
    {
        position = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));
        name = "(" + position.x + "," + position.y + "," + position.z + ")";

        ChunkGen.StartGenerate(this);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // 未完成生成直接返回
        if (!isFinished)
            return;

        // 距离足够大，进行销毁
        float disX = position.x + ChunkGen.width / 2 - player.transform.position.x;
        float disZ = position.z + ChunkGen.width / 2 - player.transform.position.z;
        if (disX * disX + disZ * disZ > World.minDistanceDes * World.minDistanceDes)
        {
            Debug.Log("销毁区块:(" + position.x + ", " + position.y + ", " + position.z + ")");
            World.DestroyChunk(position);
            Destroy(gameObject);
        }
    }
}
