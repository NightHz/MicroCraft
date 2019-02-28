using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise
{
    int seed;
    Dictionary<Vector2Int, Vector2> grads;

    public PerlinNoise(int seed)
    {
        this.seed = seed;
        grads = new Dictionary<Vector2Int, Vector2>();
    }

    // 计算格点梯度
    Vector2 CalculateGradient(Vector2Int p)
    {
        System.Random r = new System.Random(seed ^ p.GetHashCode());
        while (true)
        {
            float gx = (float)r.NextDouble() * 2 - 1;
            float gy = (float)r.NextDouble() * 2 - 1;
            Vector2 g = new Vector2(gx, gy);
            float length = g.magnitude;
            if (length > 1 || length < 0.1f)
                continue;
            else
                return g / length;
        }
    }

    // 获取格点梯度
    Vector2 GetGradient(Vector2Int p)
    {
        if (grads.ContainsKey(p))
            return grads[p];
        else
        {
            Vector2 g = CalculateGradient(p);
            grads.Add(p, g);
            return g;
        }
    }

    // 计算距离向量与格点梯度的点积
    float DotGridGradient(int ix, int iy, float x, float y)
    {
        float dx = x - ix, dy = y - iy;
        Vector2 g = GetGradient(new Vector2Int(ix, iy));
        return (dx * g.x + dy * g.y);
    }

    // 缓动函数
    static float Fade(float t)
    {
        return t * t * t * (10 + t * (-15 + t * 6)); // 6*t^5 - 15*t^4 + 10*t^3
    }
    
    /// <summary>
    /// 返回值在 -1.0 到 1.0 之间
    /// </summary>
    public float GetNoise(float x, float y)
    {
        int xi = Mathf.FloorToInt(x);
        int yi = Mathf.FloorToInt(y);
        float xf = Fade(x - xi);
        float yf = Fade(y - yi);
        float n0, n1, n2, n3;
        n0 = DotGridGradient(xi, yi, x, y);
        n1 = DotGridGradient(xi + 1, yi, x, y);
        n2 = Mathf.Lerp(n0, n1, xf);
        n0 = DotGridGradient(xi, yi + 1, x, y);
        n1 = DotGridGradient(xi + 1, yi + 1, x, y);
        n3 = Mathf.Lerp(n0, n1, xf);
        return Mathf.Lerp(n2, n3, yf);
    }

    /// <summary>
    /// 返回值在 -1.0 到 1.0 之间
    /// </summary>
    public float GetNoiseSum(float x, float y)
    {
        float max = 0;
        float s = 0;
        s += GetNoise(x, y); max += 1;
        x *= 2; y *= 2; s += 0.5000f * GetNoise(x, y); max += 0.5000f;
        x *= 2; y *= 2; s += 0.2500f * GetNoise(x, y); max += 0.2500f;
        x *= 2; y *= 2; s += 0.1250f * GetNoise(x, y); max += 0.1250f;
        x *= 2; y *= 2; s += 0.0625f * GetNoise(x, y); max += 0.0625f;
        return s / max;
    }

    /// <summary>
    /// 返回值在 0 到 1.0 之间
    /// </summary>
    public float GetNoiseSumAbs(float x, float y)
    {
        float max = 0;
        float s = 0;
        s += Mathf.Abs(GetNoise(x, y)); max += 1;
        x *= 2; y *= 2; s += 0.5000f * Mathf.Abs(GetNoise(x, y)); max += 0.5000f;
        x *= 2; y *= 2; s += 0.2500f * Mathf.Abs(GetNoise(x, y)); max += 0.2500f;
        x *= 2; y *= 2; s += 0.1250f * Mathf.Abs(GetNoise(x, y)); max += 0.1250f;
        x *= 2; y *= 2; s += 0.0625f * Mathf.Abs(GetNoise(x, y)); max += 0.0625f;
        return s / max;
    }

    /// <summary>
    /// 返回值在 -1.0 到 1.0 之间
    /// </summary>
    public float GetNoiseSumAbsSin(float x, float y)
    {
        float s = 0;
        float x0 = x;
        s += Mathf.Abs(GetNoise(x, y));
        x *= 2; y *= 2; s += 0.5000f * Mathf.Abs(GetNoise(x, y));
        x *= 2; y *= 2; s += 0.2500f * Mathf.Abs(GetNoise(x, y));
        x *= 2; y *= 2; s += 0.1250f * Mathf.Abs(GetNoise(x, y));
        x *= 2; y *= 2; s += 0.0625f * Mathf.Abs(GetNoise(x, y));
        s = Mathf.Sin(s + x0);
        return s;
    }
}

public class PerlinNoise3
{
    int seed;
    Dictionary<Vector3Int, Vector3> grads;

    public PerlinNoise3(int seed)
    {
        this.seed = seed;
        grads = new Dictionary<Vector3Int, Vector3>();
    }

    // 计算格点梯度
    Vector3 CalculateGradient(Vector3Int p)
    {
        System.Random r = new System.Random(seed ^ p.GetHashCode());
        while (true)
        {
            float gx = (float)r.NextDouble() * 2 - 1;
            float gy = (float)r.NextDouble() * 2 - 1;
            float gz = (float)r.NextDouble() * 2 - 1;
            Vector3 g = new Vector3(gx, gy, gz);
            float length = g.magnitude;
            if (length > 1 || length < 0.1f)
                continue;
            else
                return g / length;
        }
    }

    // 获取格点梯度
    Vector3 GetGradient(Vector3Int p)
    {
        if (grads.ContainsKey(p))
            return grads[p];
        else
        {
            Vector3 g = CalculateGradient(p);
            grads.Add(p, g);
            return g;
        }
    }

    // 计算距离向量与格点梯度的点积
    float DotGridGradient(int ix, int iy, int iz, float x, float y,float z)
    {
        float dx = x - ix, dy = y - iy, dz = z - iz;
        Vector3 g = GetGradient(new Vector3Int(ix, iy, iz));
        return (dx * g.x + dy * g.y + dz * g.z);
    }

    // 缓动函数
    static float Fade(float t)
    {
        return t * t * t * (10 + t * (-15 + t * 6)); // 6*t^5 - 15*t^4 + 10*t^3
    }

    /// <summary>
    /// 返回值在 -1.0 到 1.0 之间
    /// </summary>
    public float GetNoise(float x, float y, float z)
    {
        int xi = Mathf.FloorToInt(x);
        int yi = Mathf.FloorToInt(y);
        int zi = Mathf.FloorToInt(z);
        float xf = Fade(x - xi);
        float yf = Fade(y - yi);
        float zf = Fade(z - zi);
        float n0, n1, n2, n3, n4, n5;
        n0 = DotGridGradient(xi, yi, zi, x, y, z);
        n1 = DotGridGradient(xi + 1, yi, zi, x, y, z);
        n2 = Mathf.Lerp(n0, n1, xf);
        n0 = DotGridGradient(xi, yi + 1, zi, x, y, z);
        n1 = DotGridGradient(xi + 1, yi + 1, zi, x, y, z);
        n3 = Mathf.Lerp(n0, n1, xf);
        n4 = Mathf.Lerp(n2, n3, yf);
        n0 = DotGridGradient(xi, yi, zi + 1, x, y, z);
        n1 = DotGridGradient(xi + 1, yi, zi + 1, x, y, z);
        n2 = Mathf.Lerp(n0, n1, xf);
        n0 = DotGridGradient(xi, yi + 1, zi + 1, x, y, z);
        n1 = DotGridGradient(xi + 1, yi + 1, zi + 1, x, y, z);
        n3 = Mathf.Lerp(n0, n1, xf);
        n5 = Mathf.Lerp(n2, n3, yf);
        return Mathf.Lerp(n4, n5, zf);
    }

    /// <summary>
    /// 返回值在 -1.0 到 1.0 之间
    /// </summary>
    public float GetNoiseSum(float x, float y, float z)
    {
        float max = 0;
        float s = 0;
        s += GetNoise(x, y, z); max += 1;
        x *= 2; y *= 2; s += 0.5000f * GetNoise(x, y, z); max += 0.5000f;
        x *= 2; y *= 2; s += 0.2500f * GetNoise(x, y, z); max += 0.2500f;
        x *= 2; y *= 2; s += 0.1250f * GetNoise(x, y, z); max += 0.1250f;
        x *= 2; y *= 2; s += 0.0625f * GetNoise(x, y, z); max += 0.0625f;
        return s / max;
    }
}

public class PerlinNoiseBounded
{
    int seed;
    System.Random random;
    Vector2[,] grads;

    public PerlinNoiseBounded(int seed, int xSize, int ySize)
    {
        this.seed = seed;
        random = new System.Random(seed);
        grads = new Vector2[xSize, ySize];
        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++)
                grads[x, y] = RandomGradient();
    }

    // 计算格点梯度
    Vector2 RandomGradient()
    {
        while (true)
        {
            float gx = (float)random.NextDouble() * 2 - 1;
            float gy = (float)random.NextDouble() * 2 - 1;
            Vector2 g = new Vector2(gx, gy);
            float length = g.magnitude;
            if (length > 1 || length < 0.1f)
                continue;
            else
                return g / length;
        }
    }

    // 计算距离向量与格点梯度的点积
    float DotGridGradient(int ix, int iy, float x, float y)
    {
        float dx = x - ix, dy = y - iy;
        Vector2 g = grads[ix, iy];
        return (dx * g.x + dy * g.y);
    }

    // 缓动函数
    static float Fade(float t)
    {
        return t * t * t * (10 + t * (-15 + t * 6)); // 6*t^5 - 15*t^4 + 10*t^3
    }

    /// <summary>
    /// 返回值在 -1.0 到 1.0 之间
    /// </summary>
    public float GetNoise(float x, float y)
    {
        int xi = Mathf.FloorToInt(x);
        int yi = Mathf.FloorToInt(y);
        float xf = Fade(x - xi);
        float yf = Fade(y - yi);
        float n0, n1, n2, n3;
        n0 = DotGridGradient(xi, yi, x, y);
        n1 = DotGridGradient(xi + 1, yi, x, y);
        n2 = Mathf.Lerp(n0, n1, xf);
        n0 = DotGridGradient(xi, yi + 1, x, y);
        n1 = DotGridGradient(xi + 1, yi + 1, x, y);
        n3 = Mathf.Lerp(n0, n1, xf);
        return Mathf.Lerp(n2, n3, yf);
    }
}

