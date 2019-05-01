using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldState : byte
{
    Init,
    Working
}

/// <summary>
/// 世界
/// </summary>
public class World : MonoBehaviour
{
    WorldState state;
    public WorldState State { get { return state; } }

    public string seed;

    public int maxDistanceGen = 150;
    public int minDistanceDea = 100;
    public int minDistanceDes = 300;

    public GameObject player;
    public Material chunkMaterial;

    [HideInInspector] public WorldTerrain worldTerrain;
    [HideInInspector] public ChunkManager chunkManager;

    private void Start()
    {
        state = WorldState.Init;
        worldTerrain = new WorldTerrain(seed.GetHashCode());
        chunkManager = new ChunkManager(this);

        GenerateWorld();
    }

    void GenerateWorld()
    {
        for (int x = -10 * Chunk.width; x < 10 * Chunk.width; x += Chunk.width)
        {
            for (int z = -10 * Chunk.width; z < 10 * Chunk.width; z += Chunk.width)
            {
                Vector3Int pos = new Vector3Int(x, 0, z);
                if ((pos + Chunk.centerPosOffset).magnitude < maxDistanceGen)
                    chunkManager.ActivateChunk(pos);
            }
        }
    }

    private void Update()
    {
        switch(state)
        {
            case WorldState.Init:
                chunkManager.UpdateChunk(false);
                if (chunkManager.ChunkWaitUpdateCount == 0)
                    state = WorldState.Working;
                break;

            case WorldState.Working:
                Work();
                chunkManager.UpdateChunk();
                break;

            default:
                break;
        }
    }

    void Work()
    {
        Vector3 playerPos = player.transform.position;
        for (int x = Chunk.AlignChunkXZ(-10 * Chunk.width + playerPos.x); x < 10 * Chunk.width + playerPos.x; x += Chunk.width)
        {
            for (int z = Chunk.AlignChunkXZ(-10 * Chunk.width + playerPos.z); z < 10 * Chunk.width + playerPos.z; z += Chunk.width)
            {
                Vector3Int pos = new Vector3Int(x, 0, z);
                if ((pos + Chunk.centerPosOffset - playerPos).magnitude < maxDistanceGen)
                    chunkManager.ActivateChunk(pos);
            }
        }
    }
    
}
