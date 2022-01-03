using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateManager : MonoBehaviour
{
    List<DayInfo> DaysOnSelected;
	DayInfo[] Days;

	private void Start()
	{
		DaysOnSelected = new List<DayInfo>();
	}

	public void InitializeDays()
	{
		Days = new DayInfo[35];
		Days[0] = GetComponentInChildren<DayInfo>();
		for (int i = 1; i < 35; i++)
		{
			Days[i] = Instantiate<GameObject>(Days[0].gameObject, transform).GetComponent<DayInfo>();
		}
		
	}

	public void Refresh()
	{
		
	}
}
