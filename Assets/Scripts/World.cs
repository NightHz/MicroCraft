using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界
/// </summary>
public class World : MonoBehaviour
{
    public string seed;

    public static readonly int maxDistanceGen = 100;
    public static readonly int minDistanceDes = 200*100;

    public GameObject player;
    public Material chunkMaterial;

    [HideInInspector] public WorldTerrain worldTerrain;
    [HideInInspector] public ChunkManager chunkManager;

    private void Start()
    {
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
        chunkManager.UpdateChunk();
    }
    
}
