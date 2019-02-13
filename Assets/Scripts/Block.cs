using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public static float uvOffset = 1f / 16;
    public static float uvShrink = uvOffset / 32;

    public byte id;
    public string name;

    public float frontU, frontV;
    public float backU, backV;
    public float leftU, leftV;
    public float rightU, rightV;
    public float topU, topV;
    public float bottomU, bottomV;

    public float frontU2, frontV2;
    public float backU2, backV2;
    public float leftU2, leftV2;
    public float rightU2, rightV2;
    public float topU2, topV2;
    public float bottomU2, bottomV2;

    /// <summary>
    /// 一般的构造器
    /// </summary>
    public Block(byte id, string name, byte frontX, byte frontY, byte backX, byte backY,
        byte leftX, byte leftY, byte rightX, byte rightY, byte topX, byte topY, byte bottomX, byte bottomY)
    {
        this.id = id;
        this.name = name;
        frontU = frontX * uvOffset;   frontV2 = frontY * uvOffset;
        backU = backX * uvOffset;     backV2 = backY * uvOffset;
        leftU = leftX * uvOffset;     leftV2 = leftY * uvOffset;
        rightU = rightX * uvOffset;   rightV2 = rightY * uvOffset;
        topU = topX * uvOffset;       topV2 = topY * uvOffset;
        bottomU = bottomX * uvOffset; bottomV2 = bottomY * uvOffset;

        frontU2 = frontU + uvOffset;   frontV = frontV2 + uvOffset;
        backU2 = backU + uvOffset;     backV = backV2 + uvOffset;
        leftU2 = leftU + uvOffset;     leftV = leftV2 + uvOffset;
        rightU2 = rightU + uvOffset;   rightV = rightV2 + uvOffset;
        topU2 = topU + uvOffset;       topV = topV2 + uvOffset;
        bottomU2 = bottomU + uvOffset; bottomV = bottomV2 + uvOffset;

        frontU += uvShrink; frontV2 += uvShrink; frontU2 -= uvShrink; frontV -= uvShrink;
        backU += uvShrink; backV2 += uvShrink; backU2 -= uvShrink; backV -= uvShrink;
        leftU += uvShrink; leftV2 += uvShrink; leftU2 -= uvShrink; leftV -= uvShrink;
        rightU += uvShrink; rightV2 += uvShrink; rightU2 -= uvShrink; rightV -= uvShrink;
        topU += uvShrink; topV2 += uvShrink; topU2 -= uvShrink; topV -= uvShrink;
        bottomU += uvShrink; bottomV2 += uvShrink; bottomU2 -= uvShrink; bottomV -= uvShrink;
    }

    /// <summary>
    /// 6面相同
    /// </summary>
    public Block(byte id, string name, byte x, byte y)
        : this(id, name, x, y, x, y, x, y, x, y, x, y, x, y)
    { }

    /// <summary>
    /// 上下相同(0) 四周相同(1)
    /// </summary>
    public Block(byte id, string name, byte x0, byte y0, byte x1, byte y1)
        : this(id, name, x1, y1, x1, y1, x1, y1, x1, y1, x0, y0, x0, y0)
    { }

    /// <summary>
    /// 四周相同(2) 上面(0) 下面(1)
    /// </summary>
    public Block(byte id, string name, byte x0, byte y0, byte x1, byte y1, byte x2, byte y2)
        : this(id, name, x2, y2, x2, y2, x2, y2, x2, y2, x0, y0, x1, y1)
    { }
}

public class BlockList
{
    public static Dictionary<byte, Block> blocks = null;

    public static void Awake()
    {
        if (blocks == null)
        {
            blocks = new Dictionary<byte, Block>();
            blocks.Add(1, new Block(1, "dirt", 2, 15));
            blocks.Add(2, new Block(2, "grass", 0, 15, 2, 15, 3, 15));
        }
    }

    public static Block GetBlock(byte id)
    {
        if (blocks == null)
            Awake();
        return blocks[id];
    }
}