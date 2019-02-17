using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界地形
/// 这是一个静态类
/// </summary>
public static class WorldTerrain
{
    public static int height = 32;

    static int seed;
    static PerlinNoise noise;

    public static void Awake(int seed)
    {
        WorldTerrain.seed = seed;
        noise = new PerlinNoise(seed);
        Debug.Log("seed=" + seed);
    }

    public static BlockID GetBlock(int x, int y, int z)
    {
        //int y2 = Mathf.FloorToInt(Mathf.PerlinNoise(x / 20f, z / 20f) * 15 + 10);
        int y2 = Mathf.FloorToInt(noise.GetNoise(x / 50f, z / 50f) * 12 + 18);

        if (y == y2)
            return BlockID.Grass;
        else if (y > y2)
            return BlockID.Air;
        else if (y < 3)
            return BlockID.Bedrock;
        else
            return BlockID.Dirt;
    }
}
