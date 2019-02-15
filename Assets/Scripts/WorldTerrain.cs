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
    static int x0, z0;
    static PerlinNoise noise;

    public static void Awake(int seed)
    {
        WorldTerrain.seed = seed;
        noise = new PerlinNoise(seed);
        System.Random random = new System.Random(seed);
        x0 = random.Next(2000, 8000);
        z0 = random.Next(2000, 8000);
        Debug.Log("seed=" + seed + " x0=" + x0 + " z0=" + z0);
    }

    public static BlockID GetBlock(int x, int y, int z)
    {
        //x += x0;
        //z += z0;
        //int y2 = Mathf.FloorToInt(Mathf.PerlinNoise(x / 20f, z / 20f) * 15 + 10);
        int y2 = Mathf.FloorToInt(noise.GetNoise(x / 20f, z / 20f) * 10 + 10);

        //if (y > y2)
        //    return BlockID.Air;
        //else if (y < 3)
        //    return BlockID.Bedrock;
        //else if (y < 8)
        //    return BlockID.Stone;
        //else if (y2 - y < 3)
        //    return BlockID.Sand;
        //else
        //    return BlockID.Sandstone;

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
