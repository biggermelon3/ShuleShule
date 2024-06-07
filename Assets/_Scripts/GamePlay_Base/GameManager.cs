using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public GridSystem gridSystem;
    public int width;
    public int height;
    public int depth;
    void Start()
    {
        int gridWidth = width; // ¸ù¾ÝÐèÒªÉèÖÃ
        int gridHeight = height; // ¸ù¾ÝÐèÒªÉèÖÃ
        int gridDepth = depth;
        gridSystem = new GridSystem(gridWidth, gridHeight,gridDepth);

    }

    private void Update()
    {
        if (score > _HIGHSCORE)
        {
            _HIGHSCORE = score;
            AddScore(0);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // ¶¨Òå³õÊ¼ÑÕÉ«Êý×é
    public Color[] initialColors;
    public ShapeData[] allShapes;

    // ÑÕÉ«"±³°ü"ÁÐ±í
    private List<Color> colorBag = new List<Color>();
    private ShapeData lastPickedShape = null;

    public ShapeGenerator ShapeGenerator;

    public int score = 0;
    public static int _HIGHSCORE = 0;
    public GameObject _ScoreObject;


    // µ÷ÓÃÕâ¸ö·½·¨À´Ôö¼Ó·ÖÊý
    public void AddScore(int points)
    {
        score += points;
    }

    ////////////////////////////////////////////////////////sceneManagement////////////////////


    ////////////////////////////////////////////////////////color random////////////////////
    // ³õÊ¼»¯ÑÕÉ«"±³°ü"£¬Ã¿ÖÖÑÕÉ«¼ÓÈëÁ½´Î
    public void InitializeColorBag()
    {
        colorBag.Clear(); // È·±£"±³°ü"ÊÇ¿ÕµÄ

        foreach (Color color in initialColors)
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

}