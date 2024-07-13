using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CanvasGroup gameOverPopup;
    public AudioSource audioSource;
    public AudioClip gameOverClip;

    private void Start()
    {
        EventManager.GameOver.AddListener(PopupGameOver);
    }

    private void PopupGameOver()
    {
        audioSource.PlayOneShot(gameOverClip);
        gameOverPopup.alpha = 1;
        gameOverPopup.blocksRaycasts = true;
        gameOverPopup.interactable = true;
    }
}
