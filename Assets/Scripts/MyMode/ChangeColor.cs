using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeColor : MonoBehaviour
{
    Color PositiveColor = new Color(0.32f, 0.42f, 0.97f, 1f);
    Color NegativeColor = new Color(0.81f, 0.81f, 0.82f, 1f);

  

    public void changeColor(bool ToggleIsON)
    {
        if (ToggleIsON == true)
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = PositiveColor;
        }
        else
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = NegativeColor;
        }
    }



}
