using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OpenURL : MonoBehaviour
{
    public string url;

    public void OpenChrome()
    {
        Application.OpenURL(url);
    }
}
