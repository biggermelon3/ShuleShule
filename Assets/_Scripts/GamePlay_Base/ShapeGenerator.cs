using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator : MonoBehaviour
{
    private GameManager GM;
    private ShapeData prevShape = null;
    private List<Color> prevColors;

    private void Awake()
    {
        prevColors = new List<Color>();
    }

    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GenerateShape();
    }
    public void GenerateShape()
    {
        
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
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.transform.position = new Vector3(blockPosition.x, blockPosition.y, 0);
            Color selectedShapeColor = GM.PickRandomColor();
            prevColors.Add(selectedShapeColor);
            block.GetComponent<Renderer>().material.color = selectedShapeColor; // Ó¦ÓÃÑÕÉ«
            block.transform.parent = _Shape.transform; // ÉèÖÃ¸¸¶ÔÏóÒÔ±£³Ö²ã¼¶ÇåÎú

            Block blockComponent = block.AddComponent<Block>(); // Ìí¼ÓBlock×é¼þ
            blockComponent.Initialize(selectedShapeColor);

            //jelly feel
            block.AddComponent<JellyFeel>();

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
    }
    
    public void GeneratePrevShape()
    {
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

        // Ê¹ÓÃÐÎ×´Êý¾ÝÉú³ÉÐÎ×´
        int i = 0;
        foreach (Vector2Int blockPosition in selectedShape.blocksPositions)
        {
            // ÎªÃ¿¸ö·½¿éÎ»ÖÃÉú³ÉÒ»¸ö·½¿é
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.transform.position = new Vector3(blockPosition.x, blockPosition.y, 0);
            Color selectedShapeColor = prevColors[i];
            i++;
            block.GetComponent<Renderer>().material.color = selectedShapeColor; // Ó¦ÓÃÑÕÉ«
            block.transform.parent = _Shape.transform; // ÉèÖÃ¸¸¶ÔÏóÒÔ±£³Ö²ã¼¶ÇåÎú

            Block blockComponent = block.AddComponent<Block>(); // Ìí¼ÓBlock×é¼þ
            blockComponent.Initialize(selectedShapeColor);

            //jelly feel
            block.AddComponent<JellyFeel>();


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
    }
}
