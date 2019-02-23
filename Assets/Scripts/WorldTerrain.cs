using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界地形
/// 这是一个静态类
/// </summary>
public static class WorldTerrain
{
    static readonly int version = 1;
    public static readonly int heightMax = 128;
    static readonly int seaLevel = 63;

    static int seed;
    static int elevationSeed, roughnessSeed, detailSeed;
    static PerlinNoise elevationNoise;
    static PerlinNoise roughnessNoise, detailNoise;
    static int densitySeed;
    static PerlinNoise3 densityNoise;

    public static void Awake(int seed)
    {
        WorldTerrain.seed = seed;
        System.Random random = new System.Random(seed);
        Debug.Log("seed=" + seed);
        if (version == 1)
        {
            elevationSeed = random.Next();
            roughnessSeed = random.Next();
            detailSeed = random.Next();
            elevationNoise = new PerlinNoise(elevationSeed);
            roughnessNoise = new PerlinNoise(roughnessSeed);
            detailNoise = new PerlinNoise(detailSeed);
            Debug.Log("elevationSeed=" + elevationSeed);
            Debug.Log("roughnessSeed=" + roughnessSeed);
            Debug.Log("detailSeed=" + detailSeed);
        }
        else
        {
            densitySeed = random.Next();
            densityNoise = new PerlinNoise3(densitySeed);
            Debug.Log("densitySeed=" + densitySeed);
        }
    }

    public static BlockID GetBlock(int x, int y, int z)
    {
        if (version == 1)
        {
            float elevation = elevationNoise.GetNoise(x / 100f, z / 100f) / 2;
            float roughness = roughnessNoise.GetNoise(x / 100f, z / 100f) / 4;
            float detail = detailNoise.GetNoise(x / 10f, z / 10f);
            int height = Mathf.FloorToInt((elevation + (roughness * detail)) * 64 + 64);

            if (y <= height)
                return BlockID.Stone;
            else
                return BlockID.Air;
        }
        else
        {
            float density = densityNoise.GetNoise(x / 50f, y / 50f, z / 50f);
            float heightOffset = -2f * y / heightMax + 1f;
            density += heightOffset;

            if (density > 0.0)
                return BlockID.Stone;
            else if (y < seaLevel)
                return BlockID.Ice;
            else
                return BlockID.Air;
        }
    }
}
