using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTerrain
{
    public static int yMin = 0;
    public static int yMax = 31;

    public static int seed;

    public static BlockID GetBlock(int x, int y, int z)
    {
        float y2 = Mathf.PerlinNoise(x / 20f, z / 20f) * 10 + 10;
        if (y == Mathf.FloorToInt(y2))
            return BlockID.Grass;
        else if (y < y2)
            return BlockID.Dirt;
        else
            return BlockID.Air;
    }
}
