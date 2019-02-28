using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界地形
/// 这是一个静态类
/// </summary>
public static class WorldTerrain
{
    static readonly bool use3D = false;
    public static readonly int heightMax = 128;
    static readonly int seaLevel = 63;

    static int seed;
    // 2D
    static int elevationSeed, roughnessSeed, detailSeed;
    static PerlinNoiseBounded elevationNoise;
    static PerlinNoiseBounded roughnessNoise, detailNoise;
    // 3D
    static int densitySeed;
    static PerlinNoise3 densityNoise;

    public static void Awake(int seed)
    {
        WorldTerrain.seed = seed;
        System.Random random = new System.Random(seed);
        Debug.Log("seed=" + seed);
        if (!use3D)
        {
            elevationSeed = random.Next();
            roughnessSeed = random.Next();
            detailSeed = random.Next();
            elevationNoise = new PerlinNoiseBounded(elevationSeed, 100,100);
            roughnessNoise = new PerlinNoiseBounded(roughnessSeed, 100, 100);
            detailNoise = new PerlinNoiseBounded(detailSeed, 1000, 1000);
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
        if (!use3D)
        {
            float elevation = elevationNoise.GetNoise(x / 100f + 50, z / 100f + 50) / 2;
            float roughness = roughnessNoise.GetNoise(x / 100f + 50, z / 100f + 50) / 4;
            float detail = detailNoise.GetNoise(x / 20f + 500, z / 20f + 500);
            int height = Mathf.FloorToInt((elevation + (roughness * detail)) * 64 + 64);

            if (y > height)
                return BlockID.Air;
            else if (y == height)
                return BlockID.Grass;
            else if (y + 5 > height)
                return BlockID.Dirt;
            else
                return BlockID.Stone;
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
