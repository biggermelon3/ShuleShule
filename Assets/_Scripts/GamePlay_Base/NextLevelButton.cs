using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelButton : MonoBehaviour
{
    [SerializeField]
    private Button nextLevelButton;

    private void Start()
    {
        nextLevelButton.onClick.AddListener(LoadNextLevelOnClick);
    }

    public void LoadNextLevelOnClick()
    {
        EventManager.ProceedToNextLevel.Invoke("lvl1");
    }
}
