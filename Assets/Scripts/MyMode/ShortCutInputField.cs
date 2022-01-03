using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class ShortCutInputField : MonoBehaviour
{

    Color PositiveColor = new Color(0.32f, 0.42f, 0.97f, 1f);
    Color NegativeColor = new Color(0.196f, 0.196f, 0.196f, 1f);

    //Color CloseColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
    bool IsURL(string InputURL)
    {
        Regex reg = new Regex(@"^(https?|ftp|file|ws)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$");
        return reg.IsMatch(InputURL);
    }


    public void URLTest(string inputURL)
    {
        if (IsURL(inputURL) == true)
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = PositiveColor;
        }
        else
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = NegativeColor;
        }
    }

    



}
