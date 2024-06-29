using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepthProgressDisplay : MonoBehaviour
{
    [SerializeField]
    private Slider depthSlider;

    public void InitializeDepthSlider(int maxHeight, int currentHeight)
    {
        depthSlider.maxValue = maxHeight;
        depthSlider.value = currentHeight;
    }
}
