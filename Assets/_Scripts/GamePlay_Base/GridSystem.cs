using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Linq;
using System;


public class GridSystem : MonoBehaviour
{
    public Block[,,] grid;
    public int width;
    public int height;
    public int depth;
    private GameObject wireCubePrefab;
    private GameObject[,,] emptyGrid;
    [SerializeField]
    private Vector3 goalCoords;

    public Dictionary<Color, float> colorPickPercentage = new Dictionary<Color, float>();

    private void Start()
    {
        EventManager.OnDraggablePlaced.AddListener(ReadColorPercentages);
        EventManager.CheckGameOver.AddListener(CheckGameOver);
    }


    public Block[,,] InitializeGrid(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        grid = new Block[width, height, depth];

        // 初始化空网格
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    grid[x, y, z] = null;
                    
                }
            }
        }
        WireFrameTheGrid(grid);
        return grid;
    }
    //Loaddata from LevelData
    public void LoadLevelData(LevelData levelData)
    {
        if (grid != null)
        {
            ClearGrid();
        }
        
        GameObject blockPrefab = Resources.Load<GameObject>("Prefabs/Block");
        foreach (var blockData in levelData.blocks)
        {
            Vector3 position = new Vector3(blockData.x, blockData.y, blockData.z);
            GameObject blockObject = Instantiate(blockPrefab, position, Quaternion.identity);
            Block blockComponent = blockObject.GetComponent<Block>();
            blockComponent.Initialize(blockData.color);
            AddBlock(blockData.x, blockData.y, blockData.z, blockComponent);
            //Debug.Log(AddBlock(blockData.x, blockData.y, blockData.z, blockComponent));
        }
    }
    
    public void WireFrameTheGrid(Block[,,] grid)
    {
        // Load the wireframe cube prefab
        wireCubePrefab = Resources.Load<GameObject>("Prefabs/WireframeCube");
        emptyGrid = new GameObject[grid.GetLength(0), grid.GetLength(1), grid.GetLength(2)];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // Only create wireframe cubes in the top layer (highest z value)
                int highestZ = GetHighestZ(i, j);
                emptyGrid[i, j, highestZ] = Instantiate(wireCubePrefab, new Vector3(i, j, highestZ), Quaternion.identity);
            }
        }
    }

    // Ìí¼Ó·½¿éµ½Grid
    public bool AddBlock(int x, int y, int z, Block block)
    {
        if (x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth)
        {
            block.x = x;
            block.y = y;
            block.z = z;
            grid[x, y, z] = block;
            if (emptyGrid[x, y, z] != null)
            {
                Destroy(emptyGrid[x, y, z]);
                emptyGrid[x, y, z] = null;
            }
            return true;
        }
        return false;
    }
    
    //Clear all block infos
    public void ClearGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (grid[x, y, z] != null)
                    {
                        Destroy(grid[x, y, z].gameObject);
                        grid[x, y, z] = null;
                    }
                }
            }
        }
    }
    
    
    //return the list of removed blcoks, or return null is no blocks are removable
    public List<Block> CheckAndRemoveBlocks()
    {
        bool[,,] visited = new bool[width, height, depth];

        List<Block> blocksToRemove = new List<Block>();
        HashSet<Block> blocksSet = new HashSet<Block>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    // Check the block above and below
                    if (grid[x, y, z] != null)
                    {
                        Block currentBlock = grid[x, y, z];

                        // Check above
                        if (z + 1 < depth && grid[x, y, z + 1] != null && grid[x, y, z + 1].BlockColor == currentBlock.BlockColor)
                        {
                            if (!blocksSet.Contains(currentBlock))
                            {
                                blocksToRemove.Add(currentBlock);
                                blocksSet.Add(currentBlock);
                            }
                            if (!blocksSet.Contains(grid[x, y, z + 1]))
                            {
                                blocksToRemove.Add(grid[x, y, z + 1]);
                                blocksSet.Add(grid[x, y, z + 1]);
                            }
                        }

                        // Check below
                        if (z - 1 >= 0 && grid[x, y, z - 1] != null && grid[x, y, z - 1].BlockColor == currentBlock.BlockColor)
                        {
                            if (!blocksSet.Contains(currentBlock))
                            {
                                blocksToRemove.Add(currentBlock);
                                blocksSet.Add(currentBlock);
                            }
                            if (!blocksSet.Contains(grid[x, y, z - 1]))
                            {
                                blocksToRemove.Add(grid[x, y, z - 1]);
                                blocksSet.Add(grid[x, y, z - 1]);
                            }
                        }
                    }
                }
            }
        }

        // Remove blocks if any matching pairs found
        if (blocksToRemove.Count > 0)
        {
            RemoveBlocks(blocksToRemove);
            return blocksToRemove;
        }
        return null;
    }
    

    //special remove function
    public List<Block> SpecialRemove(GameManager.SpecialShapes specialShapes, Block centerblock)
    {
        List<Block> blocksToRemove = new List<Block>();
        Debug.Log("bomb");
        if (specialShapes == GameManager.SpecialShapes.Bomb_Shape)
        {
            int x = centerblock.x;
            int y = centerblock.y;
            int z = centerblock.z;

            // get blocks around the center
            AddBlockToRemoveList(blocksToRemove, x + 1, y, z); // 右边
            AddBlockToRemoveList(blocksToRemove, x - 1, y, z); // 左边
            AddBlockToRemoveList(blocksToRemove, x, y + 1, z); // 上面
            AddBlockToRemoveList(blocksToRemove, x, y - 1, z); // 下面
            AddBlockToRemoveList(blocksToRemove, x, y, z + 1); // 前面
            AddBlockToRemoveList(blocksToRemove, x, y, z - 1); // 后面
            AddBlockToRemoveList(blocksToRemove, x + 1, y + 1, z); // 右前方
            AddBlockToRemoveList(blocksToRemove, x + 1, y - 1, z); // 右后方
            AddBlockToRemoveList(blocksToRemove, x - 1, y + 1, z); // 左前方
            AddBlockToRemoveList(blocksToRemove, x - 1, y - 1, z); // 左后方

            // 添加中心方块
            blocksToRemove.Add(centerblock);

            // 移除这些方块
            RemoveBlocks(blocksToRemove);

            return blocksToRemove;
        }

        return null;
    }

    private void AddBlockToRemoveList(List<Block> blocksToRemove, int x, int y, int z)
    {
        if (x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth)
        {
            Block block = grid[x, y, z];
            if (block != null && !blocksToRemove.Contains(block))
            {
                blocksToRemove.Add(block);
            }
        }
    }

    //removeblock function
    public void RemoveBlocks(List<Block> blocksToRemove)
    {
        foreach (Block block in blocksToRemove)
        {
            if (block != null)
            {
                if (grid[block.x, block.y, block.z] != null)
                {
                    grid[block.x, block.y, block.z] = null;
                    //emptyGrid[block.x, block.y, block.z] = Instantiate(wireCubePrefab, new Vector3(block.x, block.y, block.z), Quaternion.identity);
                    EventManager.onBlockRemoved.Invoke(block.transform.position, block.BlockColor);
                    block.RemoveBlock(block);
                }
            }
        }
    }

    //check if game is over
    public void CheckGameOver()
    {
        Debug.Log("Check Game Over");
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int z = GetHighestZ(x, y);
                //Debug.Log("checking game over, highest z is " + (depth - z - 1));
                if ((depth - z - 1) >= depth)
                {
                    EventManager.GameOver.Invoke();
                }
            }
        }
    }

    public bool ValidSnap(Block block)
    {
        Vector3 position = block.transform.position;
        int closestX = Mathf.RoundToInt(position.x);
        int closestY = Mathf.RoundToInt(position.y);
        int closestZ = Mathf.RoundToInt(position.z);

        if (closestX >= 0 && closestX < width && closestY >= 0 && closestY < height && closestZ >= 0 && closestZ < depth)
        {
            if (grid[closestX, closestY, closestZ] == null)
            {
                return true;
            }
        }
        return false;
    }

    public Vector3 GetBlockCoord(Block block)
    {
        Vector3 pos = block.transform.position;
        int pX = Mathf.RoundToInt(pos.x);
        int pY = Mathf.RoundToInt(pos.y);
        int pZ = Mathf.RoundToInt(pos.z);
        return new Vector3(pX, pY, pZ);
    }

    public int GetHightestZCoord(List<Vector2> xy)
    {
        List<int> allZ = new List<int>();

        for (int i = 0; i < xy.Count; i++)
        {
            int px = Mathf.RoundToInt(xy[i].x);
            int py = Mathf.RoundToInt(xy[i].y);
            if (px >= 0 && px < width && py >= 0 && py < height)
            {
                for (int j = 0; j < depth; j++)
                {
                    if (grid[px, py, j] != null)
                    {
                        int tempz = grid[px, py, j].z;
                        allZ.Add(tempz);
                    }
                }
            }
        }
        if (allZ.Count > 0)
        {
            //Debug.Log("highest z is " + allZ.Min());
            return allZ.Min() - 1;
        }
        else
        {
            return depth-1;
        }
    }

    public int GetHighestZ(int x, int y)
    {
        List<int> allZ = new List<int>();
        for (int i = 0; i < depth; i++)
        {
            if (grid[x,y,i] != null)
            {
                allZ.Add(grid[x,y,i].z);
            }
        }

        if (allZ.Count > 0)
        {
            return allZ.Min()-1;
        }
        else
        {
            return depth-1;
        }
    }

    public int CheckEntireGridForHighestZ()
    {
        List<int> allZ = new List<int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (grid[x,y,z] != null)
                    {
                        allZ.Add(grid[x, y, z].z);
                    }
                }
            }
        }

        return allZ.Min();
    }

    public List<int> GetHightestZCoordList(List<Vector2> xy)
    {
        List<int> allZ = new List<int>();
        for (int i = 0; i < xy.Count; i++)
        {
            int px = Mathf.RoundToInt(xy[i].x);
            int py = Mathf.RoundToInt(xy[i].y);
            if (px >= 0 && px < width && py >= 0 && py < height)
            {
                allZ.Add(GetHighestZ(px,py));
            }
        }
        return allZ;
    }

    public bool SnapToGrid(Block block, int newZ)
    {
        // Calculate and snap the block to the nearest grid cell
        Vector3 position = block.transform.position;
        int closestX = Mathf.RoundToInt(position.x);
        int closestY = Mathf.RoundToInt(position.y);
        int closestZ = Mathf.RoundToInt(position.z);

        block.transform.position = new Vector3(closestX, closestY, newZ);
        return AddBlock(closestX, closestY, newZ, block); // Add to the grid system
    }

    public void ReadColorPercentages(int i)//i is useless, just need to be called when things are placed
    {
        List<Color> allZColor = new List<Color>();
        colorPickPercentage = new Dictionary<Color, float>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                int z = GetHighestZ(x, y) + 1;
                if (z < 10)
                {
                    if (grid[x, y, z] != null)
                    {
                        Block b = grid[x, y, z];
                        allZColor.Add(b.BlockColor);
                        if (!colorPickPercentage.ContainsKey(b.BlockColor))
                        {
                            colorPickPercentage.Add(b.BlockColor, 1);
                        }
                        else
                        {
                            colorPickPercentage[b.BlockColor] += 1;
                        }
                    }
                }

            }
        }
        foreach (KeyValuePair<Color, float> c in colorPickPercentage)
        {
            //Debug.Log("color percentage of " + c.Key + " " + c.Value);
        }
    }
}
