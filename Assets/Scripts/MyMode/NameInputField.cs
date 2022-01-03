using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameInputField : MonoBehaviour
{
    public void ToggleControlNameText(bool isToggleOn)
    {
        if (isToggleOn == false)
        {
            gameObject.GetComponent<TMP_InputField>().text = "";
        }

    }
}
