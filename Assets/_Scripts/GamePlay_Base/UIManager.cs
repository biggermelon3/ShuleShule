using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CanvasGroup gameOverPopup;
    public AudioSource audioSource;
    public AudioClip gameOverClip;

    public GameObject scoreBoardParent;
    public GameObject scoreBoardDisplayElementPrefab;
    public Dictionary<Color, GameObject> scoreBoardDict = new Dictionary<Color, GameObject>();

    private void Start()
    {
        EventManager.GameOver.AddListener(PopupGameOver);
        EventManager.onColorComboEffectStatusCheck.AddListener(CheckColorComboScore);
    }

    private void PopupGameOver()
    {
        audioSource.PlayOneShot(gameOverClip);
        gameOverPopup.alpha = 1;
        gameOverPopup.blocksRaycasts = true;
        gameOverPopup.interactable = true;
    }

    private void CheckColorComboScore(Color c, int count, int index)
    {
        if (scoreBoardDict.ContainsKey(c))
        {
            if (count == 0)
            {
                Destroy(scoreBoardDict[c].gameObject);
                scoreBoardDict.Remove(c);
            }
            else
            {
                scoreBoardDict[c].GetComponent<ColorComboScoreDisplay>().UpdateScoreDisplay(count);

            }
            //scoreBoardDict[c].transform.SetSiblingIndex(index);
        }
        else
        {
            if (count == 0)
            {
                return;
            }
            else
            {
                GameObject spawnedSocreBoardDisplayElement = Instantiate(scoreBoardDisplayElementPrefab);
                spawnedSocreBoardDisplayElement.transform.parent = scoreBoardParent.transform;
                spawnedSocreBoardDisplayElement.transform.localScale = Vector3.one;
                spawnedSocreBoardDisplayElement.GetComponent<ColorComboScoreDisplay>().UpdateScoreDisplay(count);
                spawnedSocreBoardDisplayElement.GetComponent<ColorComboScoreDisplay>().UpdateColor(c);
                //scoreBoardDict[c].transform.SetSiblingIndex(index);
                scoreBoardDict.Add(c, spawnedSocreBoardDisplayElement);
            }

        }

        foreach (Transform child in scoreBoardParent.transform)
        {
            child.SetSiblingIndex(index);
        }
    }
}
