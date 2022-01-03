using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ViewExit : MonoBehaviour,IPointerExitHandler
{
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOMoveY(-166, 1);
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
