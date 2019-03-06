using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界地形
/// 这是一个静态类
/// </summary>
public static class WorldTerrain
{
    public static readonly int chunkWidth = 16;
    public static readonly int heightMax = 128;
    static readonly int seaLevel = 63;

    static int seed;
    static int densitySeed;
    static PerlinNoise3 densityNoise;

    public static void Awake(int seed)
    {
        WorldTerrain.seed = seed;
        System.Random random = new System.Random(seed);
        densitySeed = random.Next();
        densityNoise = new PerlinNoise3(densitySeed);

        Debug.Log("seed=" + seed);
        Debug.Log("densitySeed=" + densitySeed);
    }

    public static BlockID GetBlock(int x, int y, int z)
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
