using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public CanvasGroup gameOverPopup;
    public AudioSource audioSource;
    public AudioClip gameOverClip;

    public GameObject scoreBoardParent;
    public GameObject scoreBoardDisplayElementPrefab;
    public List<GameObject> scoreBoardList = new List<GameObject>();
    public Dictionary<Color, GameObject> scoreBoardDict = new Dictionary<Color, GameObject>();
    public GameObject _ScoreObject;

    private void Start()
    {
        EventManager.GameOver.AddListener(PopupGameOver);
        EventManager.onColorComboEffectStatusCheck.AddListener(CheckColorComboScore);
        EventManager.newOnCOlorComboEffectStatusCheck.AddListener(UpdateColorComboScoreDisplay);
        EventManager.OnRoundComplete.AddListener(ClearAllComboScoreDisplay);

    }

    public void UI_Score_Update(int currentScore)
    {
        _ScoreObject.GetComponent<TextMeshProUGUI>().text = "Score: " + currentScore.ToString();
    }
    
    private void PopupGameOver()
    {
        audioSource.PlayOneShot(gameOverClip);
        gameOverPopup.alpha = 1;
        gameOverPopup.blocksRaycasts = true;
        gameOverPopup.interactable = true;
    }

    private void UpdateColorComboScoreDisplay(List<KeyValuePair<Color, int>> newSortedList)
    {
        for (int i = 0; i < scoreBoardList.Count; i++)
        {
            Destroy(scoreBoardList[i]);
        }
        scoreBoardList.Clear();
        //scoreBoardDict.Clear();

        for (int i = 0; i < newSortedList.Count; i++)
        {
            GameObject spawnedScoreBoardDisplayElement = Instantiate(scoreBoardDisplayElementPrefab);
            spawnedScoreBoardDisplayElement.transform.parent = scoreBoardParent.transform;
            spawnedScoreBoardDisplayElement.transform.localScale = Vector3.one;
            spawnedScoreBoardDisplayElement.GetComponent<ColorComboScoreDisplay>().UpdateScoreDisplay(newSortedList[i].Value/2);
            spawnedScoreBoardDisplayElement.GetComponent<ColorComboScoreDisplay>().UpdateColor(newSortedList[i].Key);

            scoreBoardList.Add(spawnedScoreBoardDisplayElement);
            //scoreBoardDic
        }

    }

    private void ClearAllComboScoreDisplay()
    {
        for (int i = 0; i < scoreBoardList.Count; i++)
        {
            Destroy(scoreBoardList[i]);
        }
        scoreBoardList.Clear();
    }

    private void CheckColorComboScore(Color c, int count, int index)
    {
        Debug.Log("--------count " + count);
        if (scoreBoardDict.ContainsKey(c))
        {
            Debug.Log("Check COlor combo score here");
            if (count == 0)
            {
                Debug.Log("reset score here");
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
