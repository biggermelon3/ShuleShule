using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public List<Treasure> treasures = new List<Treasure>();
    public bool success;

    private void Start()
    {
        EventManager.OnRoundCompleteRemoveBlock.AddListener(ResetSuccessState);
    }

    private void ResetSuccessState()
    {
        success = false;
    }

    private void Update()
    {
        if (CheckForTreasures() && !success)
        {
            EventManager.OnRoundComplete.Invoke();
            success = true;
        }
    }

    IEnumerator SuccessSequence()
    {

        yield return new WaitForSeconds(1f);
        success = true;
    }

    private bool CheckForTreasures()
    {
        foreach (Treasure t in treasures)
        {
            if (!t.isNotBlocked)
            {
                return false;
            }
        }
        return true;
    }
}
