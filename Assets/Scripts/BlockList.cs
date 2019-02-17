using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockID : byte
{
    Air = 0,
    Stone = 1,
    Grass = 2,
    Dirt = 3,
    Cobblestone = 4,
    WoodPlank = 5,
    Bedrock = 7,
    //Water = 8,
    //Lava = 10,
    Sand = 12,
    Wood = 17,
    Leaves = 18,
    //Glass = 20,
    Sandstone = 24
}

/// <summary>
/// 方块列表，可由此获得各种方块的实例
/// 这是一个静态类
/// </summary>
public static class BlockList
{
    static Dictionary<BlockID, Block> blocks = null;

    static void Awake()
    {
        if (blocks == null)
        {
            blocks = new Dictionary<BlockID, Block>();
            blocks.Add(BlockID.Stone, new Block(BlockID.Stone, "Stone", 1, 0));
            blocks.Add(BlockID.Grass, new Block(BlockID.Grass, "Grass", 0, 0, 2, 0, 3, 0));
            blocks.Add(BlockID.Dirt, new Block(BlockID.Dirt, "Dirt", 2, 0));
            blocks.Add(BlockID.Cobblestone, new Block(BlockID.Cobblestone, "Cobbblestone", 0, 1));
            blocks.Add(BlockID.WoodPlank, new Block(BlockID.WoodPlank, "WoodPlank", 4, 0));
            blocks.Add(BlockID.Bedrock, new Block(BlockID.Bedrock, "Bedrock", 1, 1));
            blocks.Add(BlockID.Sand, new Block(BlockID.Sand, "Sand", 2, 1));
            blocks.Add(BlockID.Wood, new Block(BlockID.Wood, "Wood", 5, 1, 4, 1));
            blocks.Add(BlockID.Leaves, new Block(BlockID.Leaves, "Leaves", 5, 3));
            blocks.Add(BlockID.Sandstone, new Block(BlockID.Sandstone, "Sandstone", 0, 12));
        }
    }

    public static Block GetBlock(BlockID id)
    {
        if (blocks == null)
            Awake();
        return blocks[id];
    }
}
