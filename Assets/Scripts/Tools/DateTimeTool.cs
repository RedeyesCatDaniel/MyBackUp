using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateTimeTool
{
	public enum Months { Jan=1, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Noc, Dec }
	public static DateTime GetDateTime(string DateTimeString)
	{
		// "2021-08-27 13:00:00"
		string[] dateTime = DateTimeString.Split(' ');
		string[] date = dateTime[0].Split('-');
		string[] time = dateTime[1].Split(':');
		int year = int.Parse(date[0]);
		int month = int.Parse(date[1]);
		int day = int.Parse(date[2]);
		int hour = int.Parse(time[0]);
		int minute = int.Parse(time[1]);
		int second = int.Parse(time[2]);
		return new DateTime(year, month, day, hour, minute, second);
	}

	public static int GetDaysInMonth(DateTime curMonth)
	{
		return DateTime.DaysInMonth(curMonth.Year, curMonth.Month);
	}

	public static DayOfWeek GetFirstInWeek(DateTime curMonth)
	{
		DateTime FirstDay = new DateTime(curMonth.Year, curMonth.Month, 1);
		return FirstDay.DayOfWeek;
	}
}
