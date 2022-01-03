using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class TestButton : MonoBehaviour
{
	public GameObject MeetingPanel;

    public void testBtn1()
	{
		if (MeetingPanel.activeSelf)
		{
			MeetingPanel.SetActive(false);
		}
		else
		{
			MeetingPanel.SetActive(true);
		}
	}

	public void testBtn2()
	{
		List<Meeting> testMeetingDate = new List<Meeting>();

		Meeting m1 = new Meeting("001", "会议1", "2021-11-12 13:30:00", "2021-11-12 15:00:00");
		Meeting m2 = new Meeting("001", "会议2", "2021-11-13 15:40:00", "2021-11-13 17:00:00");
		Meeting m3 = new Meeting("001", "会议3", "2021-11-14 13:00:00", "2021-11-14 15:00:00");
		Meeting m4 = new Meeting("001", "会议4", "2021-11-13 13:10:00", "2021-11-13 15:10:00");
		Meeting m5 = new Meeting("001", "会议5", "2021-11-14 13:40:00", "2021-11-14 15:00:00");
		Meeting m7 = new Meeting("001", "会议7", "2021-11-12 10:00:00", "2021-11-12 11:00:00");
		Meeting m8 = new Meeting("001", "会议8", "2021-11-12 13:00:00", "2021-11-12 15:00:00");
		Meeting m9 = new Meeting("001", "会议9", "2021-11-11 13:00:00", "2021-11-11 15:00:00");


		testMeetingDate.AddRange(new Meeting[] { m1,m4, m5, m8, m9 });

		MeetingSystem.Instance.SetMeetingList(testMeetingDate);
	}

	public void testBtn10()
	{
		Process.Start("calc.exe");
		Process.Start("cale.exe");
	}
}
