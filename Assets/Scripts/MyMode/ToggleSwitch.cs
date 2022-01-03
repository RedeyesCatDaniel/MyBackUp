using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour
{
    public void ChangeIsOn(bool ToggleIsOn)
    {
        if (ToggleIsOn == true)
        {
            gameObject.GetComponent<Toggle>().isOn = false;
        }
        else
        {
            gameObject.GetComponent<Toggle>().isOn = true;
        }
    }
}
