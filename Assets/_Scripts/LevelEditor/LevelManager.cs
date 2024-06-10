using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GridSystem gridSystem;
    public GameObject blockPrefab;
    public ColorSetting colorPalette;
    public InputField fileNameInput;
    public Button saveButton;
    public Button loadButton;
    public Button startPlacingButton;
    public Button stopPlacingButton;
    private Color selectedColor = Color.white;
    private bool isPlacingBlocks = false;

    private void Start()
    {
        if (colorPalette != null && colorPalette.colors.Length > 0)
        {
            selectedColor = colorPalette.colors[0];
        }

        saveButton.onClick.AddListener(SaveLevel);
        loadButton.onClick.AddListener(LoadLevel);
        startPlacingButton.onClick.AddListener(StartPlacingBlocks);
        stopPlacingButton.onClick.AddListener(StopPlacingBlocks);
    }

    private void Update()
    {
        if (isPlacingBlocks)
        {
            HandleBlockPlacement();
        }
    }

    private void HandleBlockPlacement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 position = hit.point;
                int x = Mathf.RoundToInt(position.x);
                int y = Mathf.RoundToInt(position.y);
                int z = gridSystem.GetHighestZ(x, y);

                GameObject block = Instantiate(blockPrefab, new Vector3(x, y, z), Quaternion.identity);
                Block blockComponent = block.GetComponent<Block>();
                blockComponent.Initialize(selectedColor);
                gridSystem.AddBlock(x, y, z, blockComponent);
            }
        }
    }

    public void SaveLevel()
    {
        string fileName = fileNameInput.text;
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("File name cannot be empty.");
            return;
        }

        LevelData levelData = GenerateLevelData();
        string json = JsonUtility.ToJson(levelData);
        string path = Path.Combine(Application.dataPath, fileName + ".json");
        File.WriteAllText(path, json);
        Debug.Log("Level saved to " + path);
    }

    public void LoadLevel()
    {
        string fileName = fileNameInput.text;
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("File name cannot be empty.");
            return;
        }

        string path = Path.Combine(Application.dataPath, fileName + ".json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            gridSystem.LoadLevelData(levelData);
            Debug.Log("Level loaded from " + path);
        }
        else
        {
            Debug.LogError("Saved level file not found at " + path);
        }
    }

    public void StartPlacingBlocks()
    {
        isPlacingBlocks = true;
    }

    public void StopPlacingBlocks()
    {
        isPlacingBlocks = false;
    }

    public LevelData GenerateLevelData()
    {
        LevelData levelData = new LevelData(gridSystem.width, gridSystem.height, gridSystem.depth);

        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                for (int z = 0; z < gridSystem.depth; z++)
                {
                    if (gridSystem.grid[x, y, z] != null)
                    {
                        Block currentBlock = gridSystem.grid[x, y, z];
                        BlockData blockData = new BlockData(currentBlock.x, currentBlock.y, currentBlock.z, currentBlock.BlockColor);
                        levelData.blocks.Add(blockData);
                    }
                }
            }
        }

        return levelData;
    }
}