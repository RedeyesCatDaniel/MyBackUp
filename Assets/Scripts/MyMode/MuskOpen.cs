using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuskOpen : MonoBehaviour
{
    public void MuskSwitch(bool ToggleIsOn)
    {
        if (ToggleIsOn == true)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
