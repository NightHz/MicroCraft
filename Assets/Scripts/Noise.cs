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
        System.Random r = new System.Random(seed.GetHashCode() ^ p.GetHashCode());
        while (true)
        {
            float gx = (float)r.NextDouble() * 2 - 1;
            float gy = (float)r.NextDouble() * 2 - 1;
            Vector2 g = new Vector2(gx, gy);
            float length = g.magnitude;
            if (length > 1 || length == 0)
                continue;
            else
                return g / length;
        }
    }

    // 获取格点梯度
    Vector2 GetGradient(Vector2Int p)
    {
        // 是否缓存此格点梯度
        if (grads.ContainsKey(p))
            return grads[p];
        else
        {
            Vector2 g = CalculateGradient(p);
            grads.Add(p, g);
            Debug.Log("格点梯度:p" + p + ",g" + g);
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
}
