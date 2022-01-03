using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameCanvas 
{
    private static NameCanvas instance;

    public static NameCanvas Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new NameCanvas();
            }
            return instance;
        }
    }

    public static GameObject nameCanvas;
    

    public void CreatCanvas()
    {
        GameObject CanvasObj;
        CanvasObj = new GameObject();
        Debug.Log("Creat one canvas");
        CanvasObj.name = "NameCanvas";
        CanvasObj.AddComponent<Canvas>();
        CanvasObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;


        nameCanvas = CanvasObj;
        
    }

}
