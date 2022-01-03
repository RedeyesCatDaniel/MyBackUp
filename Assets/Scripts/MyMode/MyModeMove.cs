using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MyModeMove : MonoBehaviour,IPointerEnterHandler
{
    public GameObject mymodeView;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mymodeView.transform.DOMoveY(0, 1);
    }

   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
