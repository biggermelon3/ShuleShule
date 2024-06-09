using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public List<Treasure> treasures = new List<Treasure>();

    private void Update()
    {
        if (CheckForTreasures())
        {
            Debug.Log("Success!");
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
