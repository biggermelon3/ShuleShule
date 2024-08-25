using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GridSystem gridSystem;
    public Block[,,] currentGrid;
    public int width;
    public int height;
    public int depth;
    public string lvlJsonName;

    [SerializeField]
    private GameObject roundCompletePrompt;

    void Awake()
    {
        if (lvlJsonName != "")
        {
            LoadLevel(lvlJsonName);
        }
        else
        {
            int gridWidth = width; // ¸ù¾ÝÐèÒªÉèÖÃ
            int gridHeight = height; // ¸ù¾ÝÐèÒªÉèÖÃ
            int gridDepth = depth;
            currentGrid = gridSystem.InitializeGrid(gridWidth, gridHeight,gridDepth);
        }
        
        
    }

    private void Start()
    {
        EventManager.OnRoundComplete.AddListener(DisplayRoundCompletePrompt);
        EventManager.OnRoundCompleteRemoveBlock.AddListener(RemoveAllPrevRoundBlocks);
        EventManager.ProceedToNextLevel.AddListener(LoadLevel);
        EventManager.onScoreUpdate.AddListener(AddScore);
        gridSystem.ReadColorPercentages(0);
    }

    private void DisplayRoundCompletePrompt()
    {
        roundCompletePrompt.SetActive(true);
    }

    private void RemoveAllPrevRoundBlocks()
    {
        List<Block> blocksToRemove = new List<Block>();
        Block currentBlock;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    
                    currentBlock = gridSystem.grid[x, y, z];
                    blocksToRemove.Add(currentBlock);
                }
            }
        }
        gridSystem.RemoveBlocks(blocksToRemove);
    }

    private void Update()
    {
        TimeUpdate();

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // ColorSetting 
    public ColorSetting colorPalette;
    public ShapeData[] allShapes;

    // ÑÕÉ«"±³°ü"ÁÐ±í
    private List<Color> colorBag = new List<Color>();
    private ShapeData lastPickedShape = null;
    public enum SpecialShapes
    {
        Basic,
        Bomb_Shape,
        Bomb_Color
        //TODO:add more spcial shapes here

    }


    public ShapeGenerator ShapeGenerator;

    ////////////////////////////////////////////////////////Time/score Management////////////////////////

    public float CurrentTime;
    //UI manager
    public UIManager uiManager;

    private void TimeUpdate()
    {
        CurrentTime -= Time.deltaTime;
        uiManager.UI_Update_CountdownText(CurrentTime);
        //countdown to 0
        if (CurrentTime <= 0)
        {
            EventManager.GameOver.Invoke();
            CurrentTime = 0.1f;
        }
    }

    

    // add score
    public void AddScore(int points)
    {
        CurrentTime += points;
    }

    ////////////////////////////////////////////////////////Color Combo happyTime//////////////////////

    //color Combo dictionary
    private Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();
    
    private int comboThreshold = 5; // how many blocks to trigger color combo
    private int effectDuration = 1; // how long does the effect last
    public int remainingCount = 0; //how many special blocs you can put
    private bool isEffectActive = false;



    //update color counts, the combo board
    public void UpdateColorCounts(List<Block> blocks)
    {
        // temp dictionary for the new list
        Dictionary<Color, int> newColorCounts = new Dictionary<Color, int>();

        //update dictionary by selecting blocks with same colors
        if (blocks != null)
        {
            foreach (var block in blocks)
            {
                if (newColorCounts.ContainsKey(block.BlockColor))
                {
                    newColorCounts[block.BlockColor]++;
                    //Debug.Log("add one to the new colorCOunt");
                }
                else
                {
                    newColorCounts[block.BlockColor] = 1;
                    //Debug.Log("new colorCOunt");
                }
            }

        }




        // Update the main dic, add new colors into the main dic, if it doesnt exist add a new one
        foreach (var kvp in newColorCounts)
        {
            if (colorCounts.ContainsKey(kvp.Key))
            {
                colorCounts[kvp.Key] += kvp.Value;
                //Debug.Log("old add value" + kvp.Value);
            }
            else
            {
                colorCounts[kvp.Key] = kvp.Value;
                //Debug.Log("new value" + kvp.Value);
            }

            // check if it trigger the combo effect
            if (colorCounts[kvp.Key] >= comboThreshold && !isEffectActive)
            {
                StartCoroutine(TriggerComboEffect(effectDuration));
            }
        }

        // remove the color in the main dic that are not in the new dic
        List<Color> keysToRemove = new List<Color>();
        foreach (var color in colorCounts.Keys)
        {
            if (!newColorCounts.ContainsKey(color))
            {
                keysToRemove.Add(color);
            }
        }
        foreach (var color in keysToRemove)
        {
            colorCounts.Remove(color);
        }
        UpdateColorComboUI();
    }

    //the color combo effect!!!!
    private IEnumerator TriggerComboEffect(int triggerCount)
    {
        isEffectActive = true;
        Debug.Log("Combo effect triggered!");
        ShapeGenerator.SetColorComboEffect(isEffectActive);

        // initialize remainingCount
        remainingCount = triggerCount;

        while (remainingCount > 0)
        {
            // TODO: put vfx there
            // 等待触发减少次数的脚本调用
            yield return null;
        }

        // 颜色组合效果结束
        isEffectActive = false;
        ShapeGenerator.SetColorComboEffect(isEffectActive);
        ClearColorCounts();
    }
    

    List<KeyValuePair<Color, int>> lastSortedColorCounts = new List<KeyValuePair<Color, int>>();

    private void UpdateColorComboUI()
    {
        // Get the order, so that highest display on the top
        var sortedColorCounts = new List<KeyValuePair<Color, int>>(colorCounts);
        sortedColorCounts.Sort((firstPair, nextPair) =>
        {
            return nextPair.Value.CompareTo(firstPair.Value);
        });


        EventManager.newOnCOlorComboEffectStatusCheck.Invoke(sortedColorCounts);
        //lastSortedColorCounts = sortedColorCounts;
        //TODO: index is the order, write UpdateColorDisplay function
    }
    //TODO: even when count is 0, should still call UpdateColorDisplay, with count of 0
    private void UpdateColorDisplay(Color color, int count, int index)
    {
        //Debug.Log($"Color: {color}, Count: {count/2}, Index: {index}");
        EventManager.onColorComboEffectStatusCheck.Invoke(color, count, index);
        // TODO:UIarray: colorDisplays
        // colorDisplays[index].SetColor(color);
        // colorDisplays[index].SetCount(count);
    }

    //clear dic and UI
    public void ClearColorCounts()
    {
        colorCounts.Clear();
        UpdateColorComboUI();
    }

    ////////////////////////////////////////////////////////sceneManagement////////////////////


    ////////////////////////////////////////////////////////color random////////////////////
    // ³õÊ¼»¯ÑÕÉ«"±³°ü"£¬Ã¿ÖÖÑÕÉ«¼ÓÈëÁ½´Î
    public void InitializeColorBag()
    {
        colorBag.Clear(); // È·±£"±³°ü"ÊÇ¿ÕµÄ
        if (colorPalette == null)
        {
            Debug.LogError("no colorPalette Settings");
            return;
        }
        foreach (Color color in colorPalette.colors)
        {
            colorBag.Add(color);
            colorBag.Add(color);
            colorBag.Add(color);// Ã¿ÖÖÑÕÉ«¼ÓÈëÁ½´Î
        }
    }

    // ´Ó"±³°ü"ÖÐËæ»úÑ¡ÔñÒ»¸öÑÕÉ«£¬²¢ÔÚÑ¡Ôñºó½«ÆäÒÆ³ý
    public Color PickRandomColor()
    {

        if (colorBag.Count == 0)
        {
            // Èç¹û"±³°ü"¿ÕÁË£¬ÖØÐÂÌî³ä
            InitializeColorBag();
        }

        int index = Random.Range(0, colorBag.Count);
        Color pickedColor = colorBag[index];
        colorBag.RemoveAt(index); // ÒÆ³ýÒÑÑ¡ÑÕÉ«
        Debug.Log(pickedColor);
        return pickedColor;
    }

    /// <summary>
    /// ////////////////////////////////////////////////////////shape random////////////////////
    /// </summary>
    /// <returns></returns>
    public ShapeData PickRandomShape()
    {
        if (allShapes.Length == 0)
        {
            Debug.LogError("No shapes available.");
            return null;
        }

        int index = Random.Range(0, allShapes.Length);
        ShapeData pickedShape = allShapes[index];

        // Èç¹ûÖ»ÓÐÒ»ÖÖÐÎ×´»òÕßËæ»úµ½µÄÐÎ×´ÓëÉÏ´Î²»Í¬£¬Ö±½Ó·µ»Ø¸ÃÐÎ×´
        if (allShapes.Length == 1 || pickedShape != lastPickedShape)
        {
            lastPickedShape = pickedShape;
            return pickedShape;
        }

        // Èç¹ûÐÎ×´ÓëÉÏ´ÎÏàÍ¬£¬ÇÒÐÎ×´ÊýÁ¿´óÓÚ1£¬ÔòÔÙËæ»úÒ»´Î£¬Õâ´Î²»¼ì²éÊÇ·ñÓëÉÏ´ÎÏàÍ¬
        index = Random.Range(0, allShapes.Length);
        // Ö±½ÓÑ¡ÔñÕâ´ÎµÄ½á¹û£¬²»½øÐÐ±È½Ï
        pickedShape = allShapes[index];
        lastPickedShape = pickedShape;

        return pickedShape;
    }
    
    /// ////////////////////////////////////////////////////////Load Levels////////////////////
    public void LoadLevel(string lvlName)
    {
        string fileName = lvlName;
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("File name cannot be empty.");
            return;
        }

        TextAsset jsonFile = Resources.Load<TextAsset>("lvlFiles/" + fileName);
        if (jsonFile != null)
        {
            string json = jsonFile.text;
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            currentGrid = gridSystem.InitializeGrid(levelData.width, levelData.height, levelData.depth);
            height = levelData.height;
            width = levelData.width;
            depth = levelData.depth;
            CurrentTime = levelData.levelTime;
            
            // Initialize colorPalette and allShapes from levelData
            colorPalette = ScriptableObject.CreateInstance<ColorSetting>();
            colorPalette.colors = levelData.colors.ToArray();
            //shapes
            allShapes = new ShapeData[levelData.shapes.Count];
            for (int i = 0; i < levelData.shapes.Count; i++)
            {
                ShapeData shape = ScriptableObject.CreateInstance<ShapeData>();
                shape.blocksPositions = levelData.shapes[i].blocksPositions;
                allShapes[i] = shape;
            }
            
            gridSystem.LoadLevelData(levelData);
            Debug.Log("Level loaded from Resources/lvlFiles/" + fileName);
        }
        else
        {
            Debug.LogError("Saved level file not found in Resources/lvlFiles/" + fileName);
        }
        gridSystem.ReadColorPercentages(0);
    }
}
