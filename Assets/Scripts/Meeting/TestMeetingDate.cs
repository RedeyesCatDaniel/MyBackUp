using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeetingDate : MonoBehaviour
{
	List<Meeting> testMeetingDate;
	private void Start()
	{
		testMeetingDate = new List<Meeting>();

		Meeting m1 = new Meeting("001", "会议1", "2021-11-15 13:30:00", "2021-11-15 15:00:00");
		Meeting m2 = new Meeting("001", "会议2", "2021-11-13 15:40:00", "2021-11-13 17:00:00");
		Meeting m3 = new Meeting("001", "会议3", "2021-11-14 13:00:00", "2021-11-14 15:00:00");
		Meeting m4 = new Meeting("001", "会议4", "2021-11-13 13:10:00", "2021-11-13 15:10:00");
		Meeting m5 = new Meeting("001", "会议5", "2021-11-14 13:40:00", "2021-11-14 15:00:00");
		Meeting m6 = new Meeting("001", "会议6", "2021-11-15 13:40:00", "2021-11-15 15:00:00");
		Meeting m7 = new Meeting("001", "会议7", "2021-11-15 10:00:00", "2021-11-15 11:00:00");
		Meeting m8 = new Meeting("001", "会议8", "2021-11-15 13:00:00", "2021-11-15 15:00:00");
		Meeting m9 = new Meeting("001", "会议9", "2021-11-11 13:00:00", "2021-11-11 15:00:00");


		testMeetingDate.AddRange(new Meeting[] { m1, m2, m3, m4, m5, m6, m7, m8, m9 });




		MeetingSystem.Instance.meetingDate = this;
		MeetingSystem.Instance.SetMeetingList(testMeetingDate);
	}

}
