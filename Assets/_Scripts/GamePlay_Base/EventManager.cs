using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public static class EventManager
{
    public static UnityEvent<int> OnDraggablePlaced = new UnityEvent<int>();
    public static UnityEvent OnRotateDraggable = new UnityEvent();
    public static UnityEvent ShuffleColor = new UnityEvent();
    public static UnityEvent OnRoundComplete = new UnityEvent();
    public static UnityEvent OnRoundCompleteRemoveBlock = new UnityEvent();
    public static UnityEvent<string> ProceedToNextLevel = new UnityEvent<string>();
}
