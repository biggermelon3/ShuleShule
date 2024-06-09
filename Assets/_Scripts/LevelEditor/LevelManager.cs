using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GridSystem gridSystem;
    public GameObject blockPrefab;
    public LevelData GenerateLevelData()
    {
        int width = gridSystem.width;
        int height = gridSystem.height;
        int depth = gridSystem.depth;
        Block[,,] grid = gridSystem.grid;

        LevelData levelData = new LevelData(width, height, depth);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (grid[x, y, z] != null)
                    {
                        Block currentBlock = grid[x, y, z];
                        BlockData blockData = new BlockData(currentBlock.x, currentBlock.y, currentBlock.z, currentBlock.BlockColor);
                        levelData.blocks.Add(blockData);
                    }
                }
            }
        }

        return levelData;
    }

    public void LoadLevelData(LevelData levelData)
    {
        //TODO: make a clearGrid function that clear all blocks and rebuild the gridsystem
        //gridSystem.ClearGrid();

        foreach (var blockData in levelData.blocks)
        {
            GameObject block = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), new Vector3(blockData.x, blockData.y, blockData.z), Quaternion.identity); // Assuming you have a blockPrefab
            Block blockComponent = block.AddComponent<Block>(); // Ìí¼ÓBlock×é¼þ
            blockComponent.Initialize(blockData.color);
            gridSystem.AddBlock(blockData.x, blockData.y, blockData.z, blockComponent);
        }
    }
}
