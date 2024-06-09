using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int width;
    public int height;
    public int depth;
    public List<BlockData> blocks;

    public LevelData(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.blocks = new List<BlockData>();
    }
}

[Serializable]
public class BlockData
{
    public int x;
    public int y;
    public int z;
    public Color color;

    public BlockData(int x, int y, int z, Color color)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.color = color;
    }
}
