using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ShapeGenerator : MonoBehaviour
{
    private GameManager GM;
    private ShapeData prevShape = null;
    private List<Color> prevColors;
    private GameObject blockPrefab;

    private GameObject currentBlock;//stores the generated shape into this, for shuffling color
    private bool isColorComboEffectActive = false;//track color combo

    private void Awake()
    {
        prevColors = new List<Color>();
    }

    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        blockPrefab = Resources.Load<GameObject>("Prefabs/Block");
        EventManager.ProceedToNextLevel.AddListener(ShuffleColorForNewLevel);
        EventManager.ShuffleColor.AddListener(ShuffleColor);
        GenerateShape();
    }
    public void GenerateShape()
    {
        //color combo time
        if (isColorComboEffectActive)
        {
            GenerateSpecialBlock();
            GM.remainingCount--; // only generate special block when it is the first Generated block
            return;
        }

        // Ëæ»úÑ¡ÔñÒ»¸öÐÎ×´Êý¾Ý
        ShapeData selectedShape = GM.PickRandomShape();
        prevShape = selectedShape;
        prevColors.Clear();
        //Random Color from GM
        GM.InitializeColorBag();
        //parenting
        GameObject _Shape = new GameObject("Shape" + selectedShape.name);
        _Shape.transform.position = Vector3.zero;
        Draggable dragcomponet = _Shape.AddComponent<Draggable>();
        

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        // Ê¹ÓÃÐÎ×´Êý¾ÝÉú³ÉÐÎ×´
        foreach (Vector2Int blockPosition in selectedShape.blocksPositions)
        {
            // ÎªÃ¿¸ö·½¿éÎ»ÖÃÉú³ÉÒ»¸ö·½¿é
            
            GameObject block = Instantiate(blockPrefab);
            
            block.transform.position = new Vector3(blockPosition.x, blockPosition.y, 0);
            //Color selectedShapeColor = GM.PickRandomColor();
            Color selectedShapeColor = GetRandomColorBaseOnChance();



            prevColors.Add(selectedShapeColor);
            block.GetComponent<Renderer>().material.color = selectedShapeColor; // Ó¦ÓÃÑÕÉ«
            block.transform.parent = _Shape.transform; // ÉèÖÃ¸¸¶ÔÏóÒÔ±£³Ö²ã¼¶ÇåÎú

            Block blockComponent = block.GetComponent<Block>(); // Ìí¼ÓBlock×é¼þ
            blockComponent.Initialize(selectedShapeColor);
            
            // ¼ÆËã±ß½ç
            minBounds = Vector3.Min(minBounds, block.transform.position);
            maxBounds = Vector3.Max(maxBounds, block.transform.position);
        }
        
        // ´´½¨²¢µ÷ÕûBoxCollider
        BoxCollider collider = _Shape.AddComponent<BoxCollider>();
        collider.center = (minBounds + maxBounds) / 2;
        collider.size = maxBounds - minBounds;

        float padding = 0.1f; // ±ß¾à´óÐ¡£¬¿ÉÒÔ¸ù¾ÝÐèÒªµ÷Õû
        collider.size += new Vector3(1 + padding, 1 + padding, 1 + padding);
        _Shape.transform.position = transform.position;
        currentBlock = _Shape;
    }
    
    //when failed to place the shape regenerate the old shape for the player
    public void GeneratePrevShape()
    {
        if (isColorComboEffectActive)
        {
            GenerateSpecialBlock();
            return;
        }

        // Ëæ»úÑ¡ÔñÒ»¸öÐÎ×´Êý¾Ý
        ShapeData selectedShape = prevShape;
        prevShape = selectedShape;
        //Random Color from GM
        GM.InitializeColorBag();
        //parenting
        GameObject _Shape = new GameObject("Shape" + selectedShape.name);
        Draggable dragcomponet = _Shape.AddComponent<Draggable>();

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        int i = 0;
        foreach (Vector2Int blockPosition in selectedShape.blocksPositions)
        {
            GameObject block = Instantiate(blockPrefab);
            Color selectedShapeColor = prevColors[i];
            block.transform.position = new Vector3(blockPosition.x, blockPosition.y, 0);
            //Color selectedShapeColor = GM.PickRandomColor();
            //prevColors.Add(selectedShapeColor);
            block.GetComponent<Renderer>().material.color = selectedShapeColor; // Ó¦ÓÃÑÕÉ«
            block.transform.parent = _Shape.transform; // ÉèÖÃ¸¸¶ÔÏóÒÔ±£³Ö²ã¼¶ÇåÎú

            Block blockComponent = block.GetComponent<Block>(); // Ìí¼ÓBlock×é¼þ
            blockComponent.Initialize(selectedShapeColor);
            
            // ¼ÆËã±ß½ç
            minBounds = Vector3.Min(minBounds, block.transform.position);
            maxBounds = Vector3.Max(maxBounds, block.transform.position);
            i++;
        }

        // ´´½¨²¢µ÷ÕûBoxCollider
        BoxCollider collider = _Shape.AddComponent<BoxCollider>();
        collider.center = (minBounds + maxBounds) / 2;
        collider.size = maxBounds - minBounds;

        float padding = 0.1f; // ±ß¾à´óÐ¡£¬¿ÉÒÔ¸ù¾ÝÐèÒªµ÷Õû
        collider.size += new Vector3(1 + padding, 1 + padding, 1 + padding);
        _Shape.transform.position = transform.position;
        currentBlock = _Shape;
    }

    //change basic shape to special items
    private void GenerateSpecialBlock()
    {
        // use the same logic to the normal generation
        GameObject _Shape = new GameObject("SpecialShape");
        _Shape.transform.position = Vector3.zero;
        Draggable dragcomponet = _Shape.AddComponent<Draggable>();
        dragcomponet.ShapeType = GameManager.SpecialShapes.Bomb_Shape;//TODO: add more possibiablity

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        GameObject block = Instantiate(blockPrefab);
        block.transform.position = Vector3.zero;
        Color specialColor = Color.white; // temp color to white,TODO: set it to a new object with bomb
        
        block.GetComponent<Renderer>().material.color = specialColor;
        block.GetComponent<Renderer>().material = Resources.Load<Material>("Bomb_Mat");
        block.transform.parent = _Shape.transform;

        Block blockComponent = block.GetComponent<Block>();
        blockComponent.Initialize(specialColor);

        minBounds = Vector3.Min(minBounds, block.transform.position);
        maxBounds = Vector3.Max(maxBounds, block.transform.position);

        BoxCollider collider = _Shape.AddComponent<BoxCollider>();
        collider.center = (minBounds + maxBounds) / 2;
        collider.size = maxBounds - minBounds;

        float padding = 0.1f;
        collider.size += new Vector3(1 + padding, 1 + padding, 1 + padding);
        _Shape.transform.position = transform.position;
        currentBlock = _Shape;
    }

    //change the state
    public void SetColorComboEffect(bool isActive)
    {
        isColorComboEffectActive = isActive;
    }

    private Color GetRandomColorBaseOnChance()
    {
        float random = Random.Range(0f,1f);
        float numForAdding = 0;
        float total = 0;
        Color badColor = Color.white;
        foreach (KeyValuePair<Color, float> cP in GM.gridSystem.colorPickPercentage)
        {
            total += cP.Value;
        }
        //Debug.Log("total color chances " + total);

        foreach (KeyValuePair<Color, float> cP in GM.gridSystem.colorPickPercentage)
        {
            if (cP.Value / total + numForAdding >= random)
            {
                return cP.Key;
            }
            else
            {
                numForAdding += cP.Value / total;
            }
        }
        return badColor;
    }

    public void ShuffleColorForNewLevel(string t)
    {
        Debug.Log(t);
        List<GameObject> blocksInShape = new List<GameObject>();
        foreach (Transform child in currentBlock.transform)
        {
            blocksInShape.Add(child.gameObject);
        }

        for (int x = 0; x < blocksInShape.Count; x++)
        {
            //Color selectedShapeColor = GM.PickRandomColor();
            Color selectedShapeColor = GetRandomColorBaseOnChance();
            prevColors[x] = selectedShapeColor;
            blocksInShape[x].GetComponent<Renderer>().material.color = selectedShapeColor;
            blocksInShape[x].GetComponent<Block>().Initialize(selectedShapeColor);
        }
    }

    public void ShuffleColor()
    {
        Debug.Log("shuffle");
        List<GameObject> blocksInShape = new List<GameObject>();
        foreach (Transform child in currentBlock.transform)
        {
            blocksInShape.Add(child.gameObject);
        }

        for (int x = 0; x < blocksInShape.Count; x++)
        {
            //Color selectedShapeColor = GM.PickRandomColor();
            Color selectedShapeColor = GetRandomColorBaseOnChance();
            prevColors[x] = selectedShapeColor;
            blocksInShape[x].GetComponent<Renderer>().material.color = selectedShapeColor;
            blocksInShape[x].GetComponent<Block>().Initialize(selectedShapeColor);
        }
    }
}
