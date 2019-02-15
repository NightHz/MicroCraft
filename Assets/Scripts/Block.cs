using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 方块
/// </summary>
public class Block
{
    static float uvOffset = 1f / 16;
    static float uvShrink = uvOffset / 32;

    public BlockID id;
    public string name;

    public float frontU, frontV;
    public float backU, backV;
    public float leftU, leftV;
    public float rightU, rightV;
    public float topU, topV;
    public float bottomU, bottomV;

    // 上面为材质左上角坐标，下面为右下角坐标
    public float frontU2, frontV2;
    public float backU2, backV2;
    public float leftU2, leftV2;
    public float rightU2, rightV2;
    public float topU2, topV2;
    public float bottomU2, bottomV2;

    public override string ToString()
    {
        return name + "(" + id + ")";
    }

    /// <summary>
    /// 一般的构造器
    /// </summary>
    public Block(BlockID id, string name, byte frontX, byte frontY, byte backX, byte backY,
        byte leftX, byte leftY, byte rightX, byte rightY, byte topX, byte topY, byte bottomX, byte bottomY)
    {
        this.id = id;
        this.name = name;

        // uv坐标原点为左下角 函数的输入xy原点为左上角 y->v需要进行转换
        frontU = frontX * uvOffset;   frontV = 1 - frontY * uvOffset;
        backU = backX * uvOffset;     backV = 1 - backY * uvOffset;
        leftU = leftX * uvOffset;     leftV = 1 - leftY * uvOffset;
        rightU = rightX * uvOffset;   rightV = 1 - rightY * uvOffset;
        topU = topX * uvOffset;       topV = 1 - topY * uvOffset;
        bottomU = bottomX * uvOffset; bottomV = 1 - bottomY * uvOffset;

        frontU2 = frontU + uvOffset;   frontV2 = frontV - uvOffset;
        backU2 = backU + uvOffset;     backV2 = backV - uvOffset;
        leftU2 = leftU + uvOffset;     leftV2 = leftV - uvOffset;
        rightU2 = rightU + uvOffset;   rightV2 = rightV - uvOffset;
        topU2 = topU + uvOffset;       topV2 = topV - uvOffset;
        bottomU2 = bottomU + uvOffset; bottomV2 = bottomV - uvOffset;

        // 微微缩小防止边缘显示不正确
        frontU += uvShrink;  frontU2 -= uvShrink;  frontV -= uvShrink;  frontV2 += uvShrink;
        backU += uvShrink;   backU2 -= uvShrink;   backV -= uvShrink;   backV2 += uvShrink;
        leftU += uvShrink;   leftU2 -= uvShrink;   leftV -= uvShrink;   leftV2 += uvShrink;
        rightU += uvShrink;  rightU2 -= uvShrink;  rightV -= uvShrink;  rightV2 += uvShrink;
        topU += uvShrink;    topU2 -= uvShrink;    topV -= uvShrink;    topV2 += uvShrink;
        bottomU += uvShrink; bottomU2 -= uvShrink; bottomV -= uvShrink; bottomV2 += uvShrink;
    }

    /// <summary>
    /// 6面相同
    /// </summary>
    public Block(BlockID id, string name, byte x, byte y)
        : this(id, name, x, y, x, y, x, y, x, y, x, y, x, y)
    { }

    /// <summary>
    /// 上下相同(0) 四周相同(1)
    /// </summary>
    public Block(BlockID id, string name, byte x0, byte y0, byte x1, byte y1)
        : this(id, name, x1, y1, x1, y1, x1, y1, x1, y1, x0, y0, x0, y0)
    { }

    /// <summary>
    /// 四周相同(2) 上面(0) 下面(1)
    /// </summary>
    public Block(BlockID id, string name, byte x0, byte y0, byte x1, byte y1, byte x2, byte y2)
        : this(id, name, x2, y2, x2, y2, x2, y2, x2, y2, x0, y0, x1, y1)
    { }
}
