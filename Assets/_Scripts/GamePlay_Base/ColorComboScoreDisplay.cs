using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ColorComboScoreDisplay : MonoBehaviour
{
    public TMP_Text currentScoreDisplay;
   public Image background;

    public void UpdateScoreDisplay(int i)
    {
        currentScoreDisplay.text = i.ToString();
    }

    public void UpdateColor(Color c)
    {
        background.color = c;
    }
}
