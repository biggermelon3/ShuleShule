using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Color BlockColor { get; private set; }

    private Material currMat;
    private Material invalidMat;
    private MeshRenderer _renderer;
    public int x;
    public int y;
    public int z;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        currMat = _renderer.material;
        invalidMat = Resources.Load<Material>("InvalidMat");
    }
    // ¿ÉÒÔÌí¼Ó¸ü¶àÊôÐÔ£¬ÀýÈç·½¿éµÄ×ø±êµÈ

    // ³õÊ¼»¯·½¿é
    public void Initialize(Color color)
    {
        BlockColor = color;
        // ÉèÖÃ·½¿éµÄÑÕÉ«
        GetComponent<Renderer>().material.color = color;
    }

    public void RemoveBlock(Block block)
    {
        // ¼ÙÉèÎÒÃÇÖ±½ÓÏú»ÙGameObjectÀ´Ïû³ý·½¿é
        Destroy(block.gameObject);
    }

    public void ShowInvalid()
    {
        _renderer.material = invalidMat;
    }

    public void ShowValid()
    {
        _renderer.material = currMat;
    }
}
