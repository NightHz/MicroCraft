using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界地形
/// </summary>
public class WorldTerrain
{
    public static readonly int chunkWidth = Chunk.width;
    public static readonly int heightMax = Chunk.height;

    static readonly int seaLevel = 31;

    int seed;
    int oceanSeed;
    PerlinNoise oceanNoise;
    int densitySeed;
    PerlinNoise3 densityNoise;

    public WorldTerrain(int seed)
    {
        this.seed = seed;
        System.Random random = new System.Random(seed);
        
        oceanSeed = random.Next();
        oceanNoise = new PerlinNoise(oceanSeed);
        densitySeed = random.Next();
        densityNoise = new PerlinNoise3(densitySeed);
        
        Debug.Log("seed=" + seed);
    }

    public BlockID GetBaseBlock(int x, int y, int z)
    {
        float ocean = oceanNoise.GetNoise(x / 500f, z / 500f);
        float density = densityNoise.GetNoise(x / 50f, y / 50f, z / 50f);

        float heightOffset = 5f * ((float)-y / heightMax + .5f);
        float oceanOffset = Mathf.Clamp(ocean * -8, -0.4f, 0.4f);

        density = density + heightOffset + oceanOffset;

        if (density > 0f)
            return BlockID.Stone;
        else if (y < seaLevel)
            return BlockID.Ice;
        else
            return BlockID.Air;
    }

    public void DecorateChunk(BlockID[,,] blocks)
    {
        for (int x = 0; x < chunkWidth; x++)
            for (int z = 0; z < chunkWidth; z++)
            {
                int height = -1;
                bool isWater = false;
                for (int y = heightMax - 1; y >= 0; y--)
                {
                    if (blocks[x, y, z] == BlockID.Ice)
                        isWater = true;
                    else if (blocks[x, y, z] != BlockID.Air)
                    {
                        height = y;
                        break;
                    }
                }
                if (isWater)
                {
                    for (int i = 0; i < 5; i++)
                        blocks[x, height - i, z] = BlockID.Sand;
                }
                else
                {
                    blocks[x, height, z] = BlockID.Grass;
                    for (int i = 1; i < 5; i++)
                        blocks[x, height - i, z] = BlockID.Dirt;
                }
                for (int y = 0; y < 3; y++)
                    blocks[x, y, z] = BlockID.Bedrock;
            }
    }
}
