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

public class SimplexNoise
{
    int seed;
    Dictionary<Vector2Int, Vector2> grads;

    public SimplexNoise(int seed)
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
    float DotGridGradient(int ix, int iy, float dx, float dy)
    {
        Vector2 g = GetGradient(new Vector2Int(ix, iy));
        return (dx * g.x + dy * g.y);
    }

    /// <summary>
    /// 返回值在 -1.0 到 1.0 之间
    /// </summary>
    public float GetNoise(float x, float y)
    {
        const float F = 0.366025404f; // (sqrt(3)-1)/2  (sqrt(n+1)-1)/n
        const float G = 0.211324865f; // (3-sqrt(3))/6  ((n+1)-sqrt(n+1))/(n*(n+1))
        // 坐标偏斜后的格点
        int xi = Mathf.FloorToInt(x + (x + y) * F);
        int yi = Mathf.FloorToInt(y + (x + y) * F);
        // 距离向量
        float ax = x - (xi - (xi + yi) * G);
        float ay = y - (yi - (xi + yi) * G);
        float bx = ax + G; // x - (xi + 1 - (xi + yi + 1) * G); if(x>y)
        float by = ay + G; // y - (yi - (xi + yi + 1) * G);
        if (x >= y)
            bx -= 1;
        else
            by -= 1;
        float cx = ax + G + G - 1; // x - (xi + 1 - (xi + yi + 2) * G);
        float cy = ay + G + G - 1; // y - (xi + 1 - (xi + yi + 2) * G);
        // 计算点积
        float n0, n1, n2;
        n0 = DotGridGradient(xi, yi, ax, ay);
        if (x >= y)
            n1 = DotGridGradient(xi + 1, yi, bx, by);
        else
            n1 = DotGridGradient(xi, yi + 1, bx, by);
        n2 = DotGridGradient(xi + 1, yi + 1, cx, cy);
        // 计算权重
        const float r2 = 0.5f;
        const float S = 99.2043345f; // 3^4*sqrt(6)/2
        float h0, h1, h2;
        h0 = Mathf.Max(0, r2 - ax * ax + ay * ay);
        h0 = h0 * h0 * h0 * h0;
        h1 = Mathf.Max(0, r2 - bx * bx + by * by);
        h1 = h1 * h1 * h1 * h1;
        h2 = Mathf.Max(0, r2 - cx * cx + cy * cy);
        h2 = h2 * h2 * h2 * h2;

        return (h0 * n0 + h1 * n1 + h2 * n2) * S;
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

public class SimplexNoise3
{
    int seed;
    Dictionary<Vector3Int, Vector3> grads;

    public SimplexNoise3(int seed)
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
    float DotGridGradient(int ix, int iy, int iz, float dx, float dy, float dz)
    {
        Vector3 g = GetGradient(new Vector3Int(ix, iy, iz));
        return (dx * g.x + dy * g.y + dz * g.z);
    }

    /// <summary>
    /// 返回值在 -1.0 到 1.0 之间
    /// </summary>
    public float GetNoise(float x, float y, float z)
    {
        const float F = 0.333333333f; // (sqrt(4)-1)/3  (sqrt(n+1)-1)/n
        const float G = 0.166666667f; // (4-sqrt(4))/12  ((n+1)-sqrt(n+1))/(n*(n+1))
        // 坐标偏斜后的格点
        int xi = Mathf.FloorToInt(x + (x + y + z) * F);
        int yi = Mathf.FloorToInt(y + (x + y + z) * F);
        int zi = Mathf.FloorToInt(z + (x + y + z) * F);
        // 距离向量
        float ax = x - (xi - (xi + yi + zi) * G);
        float ay = y - (yi - (xi + yi + zi) * G);
        float az = z - (zi - (xi + yi + zi) * G);
        float bx = ax + G;
        float by = ay + G;
        float bz = az + G;
        if (x >= y && x >= z)
            bx -= 1;
        else if (y >= z)
            by -= 1;
        else
            bz -= 1;
        float dx = ax + G + G + G - 1;
        float dy = ay + G + G + G - 1;
        float dz = az + G + G + G - 1;
        float cx = dx - G;
        float cy = dy - G;
        float cz = dz - G;
        if (x <= y && x <= z)
            cx += 1;
        else if (y <= z)
            cy += 1;
        else
            cz += 1;
        // 计算点积
        float n0, n1, n2, n3;
        n0 = DotGridGradient(xi, yi, zi, ax, ay, az);
        if (x >= y && x >= z)
            n1 = DotGridGradient(xi + 1, yi, zi, bx, by, bz);
        else if (y >= z)
            n1 = DotGridGradient(xi, yi + 1, zi, bx, by, bz);
        else
            n1 = DotGridGradient(xi, yi, zi + 1, bx, by, bz);
        if (x <= y && x <= z)
            n2 = DotGridGradient(xi, yi + 1, zi + 1, bx, by, bz);
        else if (y <= z)
            n2 = DotGridGradient(xi + 1, yi, zi + 1, bx, by, bz);
        else
            n2 = DotGridGradient(xi + 1, yi + 1, zi + 1, bx, by, bz);
        n3 = DotGridGradient(xi + 1, yi + 1, zi + 1, dx, dy, dz);
        // 计算权重
        const float r2 = 0.5f;
        const float S = 170.666666f; // 4^4*2/3
        float h0, h1, h2, h3;
        h0 = Mathf.Max(0, r2 - ax * ax - ay * ay - az * az);
        h0 = h0 * h0 * h0 * h0;
        h1 = Mathf.Max(0, r2 - bx * bx - by * by - bz * bz);
        h1 = h1 * h1 * h1 * h1;
        h2 = Mathf.Max(0, r2 - cx * cx - cy * cy - cz * cz);
        h2 = h2 * h2 * h2 * h2;
        h3 = Mathf.Max(0, r2 - dx * dx - dy * dy - dz * dz);
        h3 = h3 * h3 * h3 * h3;

        return (h0 * n0 + h1 * n1 + h2 * n2 + h3 * n3) * S;
    }
}

public class PerlinNoiseBounded
{
    int seed;
    Vector2[,] grads;

    public PerlinNoiseBounded(int seed, int xSize, int ySize)
    {
        this.seed = seed;
        grads = new Vector2[xSize, ySize];
        Vector2Int p = new Vector2Int();
        for (p.x = 0; p.x < xSize; p.x++)
            for (p.y = 0; p.y < ySize; p.y++)
                grads[p.x, p.y] = CalculateGradient(p);
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