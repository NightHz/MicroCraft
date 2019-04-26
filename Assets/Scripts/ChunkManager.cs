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
    public int ChunkCount
    {
        get { return chunks.Count; }
    }

    int chunkWaitUpdateCount;
    public int ChunkWaitUpdateCount
    {
        get
        {
            if (chunkWaitUpdateCount == -1)
            {
                chunkWaitUpdateCount = 0;
                foreach (KeyValuePair<Vector3Int, Chunk> pair in chunks)
                {
                    Chunk chunk2 = pair.Value;
                    if (chunk2.State < ChunkState.Working)
                        chunkWaitUpdateCount++;
                }
            }
            return chunkWaitUpdateCount;
        }
    }

    int workSpeed;
    
    public ChunkManager(World world)
    {
        this.world = world;
        playerTransform = world.player.transform;
        chunks = new Dictionary<Vector3Int, Chunk>();
        chunkWaitUpdateCount = -1;
        workSpeed = 16;
    }

    void UpdateChunk(Chunk chunk)
    {
        chunk.Update(workSpeed);
        //Debug.Log("Update Chunk: " + chunk.Position);
    }

    public void UpdateChunk(bool distanceFirst = true)
    {
        if (distanceFirst)
        {
            Chunk chunk = null;
            // 检查玩家所在区块
            Vector3Int playerChunkPosition = Chunk.AlignChunk(playerTransform.position);
            if (chunks.ContainsKey(playerChunkPosition) && (chunk = chunks[playerChunkPosition]).State < ChunkState.Working)
            {
                UpdateChunk(chunk);
                chunkWaitUpdateCount = -1;
                return;
            }

            // 找到最近的待更新区块
            float distance = float.MaxValue;
            chunkWaitUpdateCount = 0;
            foreach (KeyValuePair<Vector3Int, Chunk> pair in chunks)
            {
                Chunk chunk2 = pair.Value;
                if (chunk2.State < ChunkState.Working)
                {
                    chunkWaitUpdateCount++;
                    float d = (chunk2.Position + Chunk.centerPosOffset - playerTransform.position).magnitude;
                    if (d < distance)
                    {
                        chunk = chunk2;
                        distance = d;
                    }
                }
            }
            if (distance != float.MaxValue)
            {
                UpdateChunk(chunk);
                return;
            }
        }
        else
        {
            // 找到任意一个待更新区块
            foreach (KeyValuePair<Vector3Int, Chunk> pair in chunks)
            {
                Chunk chunk = pair.Value;
                if (chunk.State < ChunkState.Working)
                {
                    UpdateChunk(chunk);
                    return;
                }
            }
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
    
    public void SetBlock(Vector3Int blockPosition, BlockID id)
    {
        Vector3Int chunkPosition = Chunk.AlignChunk(blockPosition);
        if(chunks.ContainsKey(chunkPosition))
        {
            chunks[chunkPosition].SetBlock(blockPosition, id);
        }
    }

    public void SetBlock(int x, int y, int z, BlockID id)
    {
        SetBlock(new Vector3Int(x, y, z), id);
    }
}

