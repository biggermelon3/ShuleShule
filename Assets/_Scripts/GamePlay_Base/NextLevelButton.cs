using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelButton : MonoBehaviour
{
    [SerializeField]
    private Button nextLevelButton;
    [SerializeField]
    private GameObject nextLevelPrompt;
    private int levelIndex = 0;

    private void Start()
    {
        nextLevelButton.onClick.AddListener(LoadNextLevelOnClick);
    }

    public void LoadNextLevelOnClick()
    {
        levelIndex++;
        nextLevelPrompt.SetActive(false);
        EventManager.OnRoundCompleteRemoveBlock.Invoke();
        EventManager.ProceedToNextLevel.Invoke("lvl"+levelIndex);
    }
}
