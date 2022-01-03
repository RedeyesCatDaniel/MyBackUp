using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyMode
{
    public string Name { get; set; }

    public Image myModeImage { get; set; }

    public MyMode(string name,Image image)
    {
        this.Name = name;
        myModeImage = image;
    }
}
