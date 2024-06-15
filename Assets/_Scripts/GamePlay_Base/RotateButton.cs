using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateButton : MonoBehaviour
{
    public void RotateCurrentDraggable()
    {
        EventManager.OnRotateDraggable.Invoke();
    }

    public void ShuffleDraggableColor()
    {
        EventManager.ShuffleColor.Invoke();
    }
}
