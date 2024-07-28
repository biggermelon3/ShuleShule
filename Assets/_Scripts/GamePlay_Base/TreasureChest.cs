using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public List<Treasure> treasures = new List<Treasure>();
    bool success;
    private void Update()
    {
        if (CheckForTreasures() && !success)
        {
            StartCoroutine(SuccessSequence());

        }
    }

    IEnumerator SuccessSequence()
    {

        yield return new WaitForSeconds(1f);
        EventManager.OnRoundComplete.Invoke();
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
