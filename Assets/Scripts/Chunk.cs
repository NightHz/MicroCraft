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
    [HideInInspector]
    public bool isFinished = false;
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
        float disX = player.transform.position.x - position.x;
        float disZ = player.transform.position.z - position.z;
        if (disX * disX + disZ * disZ > 10000)
        {
            Debug.Log("销毁区块:(" + position.x + ", " + position.y + ", " + position.z + ")");
            WorldGen.DestroyChunk(position);
            Destroy(gameObject);
        }
    }
}
