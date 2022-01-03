using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectView : MonoBehaviour
{
    public void ToggleControl(bool ToggleIsOn)
    {
        if (ToggleIsOn == false)
        {
            gameObject.SetActive(false);
        }
    }

    public void ButtonControl()
    {
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
