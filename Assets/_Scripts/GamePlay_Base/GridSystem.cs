using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public Block[,,] grid;
    public int width;
    public int height;
    public int depth;
    private GameObject wireCubePrefab;
    private GameObject[,,] emptyGrid;

    public GridSystem(int width, int height,  int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        grid = new Block[width, height, depth];

        // Load the wireframe cube prefab
        wireCubePrefab = Resources.Load<GameObject>("Prefabs/WireframeCube");
        emptyGrid = new GameObject[this.width, this.height, this.depth];

        for (int i = 0; i < this.width; i++)
        {
            for (int j = 0; j < this.height; j++)
            {
                // Only create wireframe cubes in the top layer (highest z value)
                emptyGrid[i, j, this.depth - 1] = Instantiate(wireCubePrefab, new Vector3(i, j, this.depth - 1), Quaternion.identity);
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
    
    public int CheckAndRemoveBlocks()
    {
        bool[,,] visited = new bool[width, height, depth];
        int removedBlocksCount = 0;

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
                        List<Block> blocksToRemove = new List<Block>();

                        // Check above
                        if (z + 1 < depth && grid[x, y, z + 1] != null && grid[x, y, z + 1].BlockColor == currentBlock.BlockColor)
                        {
                            blocksToRemove.Add(currentBlock);
                            blocksToRemove.Add(grid[x, y, z + 1]);
                        }

                        // Check below
                        if (z - 1 >= 0 && grid[x, y, z - 1] != null && grid[x, y, z - 1].BlockColor == currentBlock.BlockColor)
                        {
                            blocksToRemove.Add(currentBlock);
                            blocksToRemove.Add(grid[x, y, z - 1]);
                        }

                        // Remove blocks if any matching pairs found
                        if (blocksToRemove.Count > 0)
                        {
                            foreach (Block block in blocksToRemove)
                            {
                                if (grid[block.x, block.y, block.z] != null)
                                {
                                    grid[block.x, block.y, block.z] = null;
                                    emptyGrid[block.x, block.y, block.z] = Instantiate(wireCubePrefab, new Vector3(block.x, block.y, block.z), Quaternion.identity);
                                    block.RemoveBlock(block);
                                    removedBlocksCount++;
                                }
                            }
                        }
                    }
                }
            }
        }
        return removedBlocksCount;
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

    public bool SnapToGrid(Block block)
    {
        // Calculate and snap the block to the nearest grid cell
        Vector3 position = block.transform.position;
        int closestX = Mathf.RoundToInt(position.x);
        int closestY = Mathf.RoundToInt(position.y);
        int closestZ = Mathf.RoundToInt(position.z);

        block.transform.position = new Vector3(closestX, closestY, closestZ);
        return AddBlock(closestX, closestY, closestZ, block); // Add to the grid system
    }
}
