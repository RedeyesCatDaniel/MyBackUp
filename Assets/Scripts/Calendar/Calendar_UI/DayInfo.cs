using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DayInfo : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerUpHandler
{
    public GameObject BackImg;
    public Text Txt;
    public GameObject Selector;
    public int DayInMonth { get; private set; }
	DateManager dateManager;

    public void Initialize(int DayInMonth,bool isCurMon,DateManager dateManager)
	{
		this.dateManager = dateManager;
		Selector.SetActive(false);
		if (DayInMonth < 0)
		{
			BackImg.SetActive(false);
			Txt.gameObject.SetActive(false);
		}
		else
		{
			BackImg.SetActive(true);
			BackImg.GetComponent<Image>().color = Color.white;
			Txt.gameObject.SetActive(true);
			Txt.text = DayInMonth.ToString();
			if (isCurMon && System.DateTime.Now.Day==DayInMonth)
			{
				// 今天当天
				BackImg.GetComponent<Image>().color = Color.red;
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{

	}

	public void OnPointerEnter(PointerEventData eventData)
	{

	}

	public void OnPointerUp(PointerEventData eventData)
	{

	}

	public void Selecte()
	{
		Selector.SetActive(true);
	}
	public void UnSelecte()
	{
		Selector.SetActive(false);
	}
}
