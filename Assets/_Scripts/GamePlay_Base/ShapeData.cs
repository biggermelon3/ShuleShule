using UnityEngine;

[CreateAssetMenu(fileName = "NewShape", menuName = "Tetris Shape", order = 1)]
public class ShapeData : ScriptableObject
{
    public Vector2Int[] blocksPositions; // ¶¨ÒåÐÎ×´µÄ·½¿éÎ»ÖÃ
}