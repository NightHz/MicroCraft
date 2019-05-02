using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 生成区块，管理已有区块，设置方块
/// </summary>
public class ChunkManager
{
    // 上层
    World world;

    Transform playerTransform;

    Dictionary<Vector3Int, Chunk> chunks;
    List<Chunk> chunkWaitUpdateList;
    public int ChunkCount
    {
        get { return chunks.Count; }
    }
    
    public int ChunkWaitUpdateCount
    {
        get { return chunkWaitUpdateList.Count; }
    }

    int workNSpeed;
    int workYSpeed;
    
    public ChunkManager(World world)
    {
        this.world = world;
        playerTransform = world.player.transform;
        chunks = new Dictionary<Vector3Int, Chunk>();
        chunkWaitUpdateList = new List<Chunk>();
        workNSpeed = 1;
        workYSpeed = 16;
    }

    public void UpdateChunkUpdateList()
    {
        chunkWaitUpdateList.Clear();
        foreach (KeyValuePair<Vector3Int, Chunk> pair in chunks)
        {
            Chunk chunk = pair.Value;
            if (chunk.State < ChunkState.Working)
            {
                chunkWaitUpdateList.Add(chunk);
            }
        }
    }

    void UpdateChunk(Chunk chunk)
    {
        if (chunk.Update(workYSpeed))
            chunkWaitUpdateList.Remove(chunk);
        //Debug.Log("Update Chunk: " + chunk.Position);
    }

    public void UpdateChunk(bool distanceFirst = true)
    {
        // 检查位置
        Vector3 playerPos = world.player.transform.position;
        List<Vector3Int> removeList = new List<Vector3Int>();
        foreach (KeyValuePair<Vector3Int, Chunk> pair in chunks)
        {
            Chunk chunk = pair.Value;
            float d = (chunk.Position + Chunk.centerPosOffset - playerPos).magnitude;
            if (d > world.minDistanceDes)
            {
                chunk.Destroy();
                removeList.Add(pair.Key);
            }
            else if (d > world.minDistanceDea)
            {
                if (chunk.Active != false)
                    chunk.Active = false;
            }
            else
            {
                if (chunk.Active != true)
                    chunk.Active = true;
            }
        }
        foreach(Vector3Int pos in removeList)
        {
            chunks.Remove(pos);
        }
        // 生成方块或网格
        if (distanceFirst)
        {
            for (int n = workNSpeed; n > 0; n--)
            {
                Chunk chunk = null;
                // 找到最近的待更新区块
                float distance = float.MaxValue;
                foreach (Chunk chunk2 in chunkWaitUpdateList)
                {
                    float d = (chunk2.Position + Chunk.centerPosOffset - playerTransform.position).magnitude;
                    if (d < distance)
                    {
                        chunk = chunk2;
                        distance = d;
                    }
                }
                if (chunk != null)
                {
                    UpdateChunk(chunk);
                }
            }
            if (ChunkWaitUpdateCount == 0)
                workNSpeed = 1;
            // 50帧限定
            else if (Time.deltaTime < 0.02f)
                workNSpeed++;
            else
                workNSpeed--;
            workNSpeed = Mathf.Clamp(workNSpeed, 1, 16);
        }
        else
        {
            for (int n = workNSpeed; n > 0; n--)
            {
                // 找到任意一个待更新区块
                foreach (Chunk chunk in chunkWaitUpdateList)
                {
                    UpdateChunk(chunk);
                    break;
                }
            }
            if (ChunkWaitUpdateCount == 0)
                workNSpeed = 1;
            // 20帧限定
            else if (Time.deltaTime < 0.05f)
                workNSpeed++;
            else
                workNSpeed--;
            workNSpeed = Mathf.Clamp(workNSpeed, 1, 16);
        }
    }

    public void ActivateChunk(Vector3Int chunkPosition)
    {
        if (chunks.ContainsKey(chunkPosition))
        {
            Chunk chunk = chunks[chunkPosition];
            if (!chunk.Active)
                chunk.Active = true;
        }
        else
        {
            CreateChunk(chunkPosition);
        }
    }

    void CreateChunk(Vector3Int chunkPosition)
    {
        Chunk newChunk = new Chunk(world, chunkPosition);
        chunks.Add(chunkPosition, newChunk);
        chunkWaitUpdateList.Add(newChunk);
    }

    public void DeactivateChunk(Vector3Int chunkPosition)
    {
        Chunk chunk;
        if (chunks.ContainsKey(chunkPosition) && (chunk = chunks[chunkPosition]).Active)
        {
            chunk.Active = false;
        }
    }

    public void DestroyChunk(Vector3Int chunkPosition)
    {
        if (chunks.ContainsKey(chunkPosition))
        {
            Chunk chunk = chunks[chunkPosition];
            chunk.Destroy();
            chunks.Remove(chunkPosition);
        }
    }

    public void CheckChunk(Vector3Int chunkPosition)
    {
        if (chunks.ContainsKey(chunkPosition))
        {
            Chunk chunk = chunks[chunkPosition];
            if (chunk.State < ChunkState.Working && !chunkWaitUpdateList.Contains(chunk))
                chunkWaitUpdateList.Add(chunk);
        }
    }
    
    public void SetBlock(Vector3Int blockPosition, BlockID id)
    {
        Vector3Int chunkPosition = Chunk.AlignChunk(blockPosition);
        if(chunks.ContainsKey(chunkPosition))
        {
            Chunk chunk = chunks[chunkPosition];
            chunk.SetBlock(blockPosition, id);
            chunkWaitUpdateList.Add(chunk);
        }
    }

    public void SetBlock(int x, int y, int z, BlockID id)
    {
        SetBlock(new Vector3Int(x, y, z), id);
    }
}

