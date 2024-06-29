using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int levelTime;
    public int width;
    public int height;
    public int depth;
    public List<Color> colors;  // 这里直接存储颜色数组
    public List<ShapeDataSerializable> shapes;  // 存储可序列化的形状数据
    public List<BlockData> blocks;

    public LevelData(int width, int height, int depth, int levelTime = 600 )
    {
        this.levelTime = levelTime;
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.colors = new List<Color>();
        this.shapes = new List<ShapeDataSerializable>();
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

[Serializable]
public class ShapeDataSerializable
{
    public Vector2Int[] blocksPositions;

    public ShapeDataSerializable(Vector2Int[] blocksPositions)
    {
        this.blocksPositions = blocksPositions;
    }
}
