using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class LevelManager : MonoBehaviour
{
    public Vector3 _width_height_depth;
    public ColorSetting[] colorPalette;
    public ShapeData[] allShapes;
    
    public GridSystem gridSystem;
    public GameObject blockPrefab;
    
    public TMP_InputField fileNameInput;
    public Button saveButton;
    public Button loadButton;
    public Button startPlacingButton;
    public Button stopPlacingButton;
    public Button startDeletingButton;
    public Button stopDeletingButton;
    public GameObject colorButtonPrefab;
    
    private Color selectedColor = Color.white;
    private bool isPlacingBlocks = false;
    private bool isDeletingBlocks = false;

    private GameObject colorButtonContainer;

    private void Start()
    {
        if (colorPalette != null && colorPalette[0].colors.Length > 0)
        {
            selectedColor = colorPalette[0].colors[0];
        }

        //search for private references
        colorButtonContainer = FindObjectOfType<HorizontalLayoutGroup>().gameObject;
        Debug.Log(colorButtonContainer +"1");
        
        InitializeColorButtons();
        
        //loadResources
        blockPrefab = Resources.Load<GameObject>("Prefabs/Block");
        gridSystem.InitializeGrid((int)_width_height_depth.x,(int)_width_height_depth.y,(int)_width_height_depth.z);
        
        //UI Events
        saveButton.onClick.AddListener(SaveLevel);
        loadButton.onClick.AddListener(LoadLevel);
        startPlacingButton.onClick.AddListener(StartPlacingBlocks);
        stopPlacingButton.onClick.AddListener(StopPlacingBlocks);
        startDeletingButton.onClick.AddListener(StartDeletingBlocks);
        stopDeletingButton.onClick.AddListener(StopDeletingBlocks);  
    }

    private void Update()
    {
        if (isPlacingBlocks)
        {
            HandleBlockPlacement();
        }else if (isDeletingBlocks)
        {

            HandleBlockDeleting();
        }
    }

    private void InitializeColorButtons()
    {
        foreach (Transform child in colorButtonContainer.transform)
        {
            Destroy(child.gameObject);
        }

        if (colorPalette != null)
        {
            foreach (Color color in colorPalette[0].colors)
            {
                GameObject colorButton = Instantiate(colorButtonPrefab, colorButtonContainer.transform);
                colorButton.GetComponent<Image>().color = color;
                Button button = colorButton.GetComponent<Button>();
                button.onClick.AddListener(() => SelectColor(color));
            }
        }
    }

    private void SelectColor(Color color)
    {
        selectedColor = color;
        Debug.Log("Selected Color: " + color);
    }
    
    //Placing block function
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
                EventManager.OnDraggablePlaced.Invoke(z);
                GameObject block = Instantiate(blockPrefab, new Vector3(x, y, z), Quaternion.identity);
                Block blockComponent = block.GetComponent<Block>();
                blockComponent.Initialize(selectedColor);
                gridSystem.AddBlock(x, y, z, blockComponent);
            }
        }
    }

    //Deleting block function
    private void HandleBlockDeleting()
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
                int z = gridSystem.GetHighestZ(x, y) + 1;
                EventManager.OnDraggablePlaced.Invoke(z);
                //TODO:make a for loop tha delete multiple grids one time
                Block currentBlock = gridSystem.grid[x, y, z];
                List<Block> blocksToRemove = new List<Block>();
                blocksToRemove.Add(currentBlock);
                Debug.Log(blocksToRemove);
                gridSystem.RemoveBlocks(blocksToRemove);
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
        ToggleUIElement(startPlacingButton, false);
        ToggleUIElement(stopPlacingButton, true);
        ToggleUIElement(startDeletingButton, true,false);
    }

    public void StopPlacingBlocks()
    {
        isPlacingBlocks = false;
        ToggleUIElement(startPlacingButton, true);
        ToggleUIElement(stopPlacingButton, false);
        ToggleUIElement(startDeletingButton, true,true);
    }

    public void StartDeletingBlocks()
    {
        isDeletingBlocks = true;
        ToggleUIElement(startDeletingButton, false);
        ToggleUIElement(stopDeletingButton, true);
        ToggleUIElement(startPlacingButton, true,false);
    }
    public void StopDeletingBlocks()
    {
        isDeletingBlocks = false;
        ToggleUIElement(startDeletingButton, true);
        ToggleUIElement(stopDeletingButton, false);
        ToggleUIElement(startPlacingButton, true,true);
    }
    
    //setUIbutton gameobject active or interactive
    public void ToggleUIElement(Button UIbutton, bool OnAndOff = true,bool interable = true)
    {
        UIbutton.interactable = interable;
        UIbutton.gameObject.SetActive(OnAndOff);
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
        // 保存颜色数据
        foreach (ColorSetting color in colorPalette)
        {
            levelData.colors.Add(color);
        }

        // 保存形状数据
        foreach (ShapeData shape in allShapes)
        {
            levelData.shapes.Add(shape);
        }

        return levelData;
    }
}