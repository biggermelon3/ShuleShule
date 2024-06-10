using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "ScriptableObjects/ColorPalette", order = 1)]
public class ColorSetting : ScriptableObject
{
    public Color[] colors;
}