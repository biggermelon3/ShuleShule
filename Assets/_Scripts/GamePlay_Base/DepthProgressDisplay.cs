using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepthProgressDisplay : MonoBehaviour
{
    [SerializeField]
    private Slider depthSlider;


    private void Start()
    {
        EventManager.OnDraggablePlaced.AddListener(UpdateDepthSlider);
        InitializeDepthSlider(12,8);
    }
    public void InitializeDepthSlider(int maxHeight, int currentHeight)
    {
        depthSlider.maxValue = maxHeight;
        depthSlider.value = currentHeight;
    }

    public void UpdateDepthSlider(int z)
    {
        Debug.Log("Update depth slider " + z);
        depthSlider.value = z;
    }
}
