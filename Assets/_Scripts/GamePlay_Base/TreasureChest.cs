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
            Debug.Log("Success!");
            success = true;
        }
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
