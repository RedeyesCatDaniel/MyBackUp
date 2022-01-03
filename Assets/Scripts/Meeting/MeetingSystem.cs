using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 管理当前用户会议列表
/// 负责在View与Date之间传递及处理数据
/// </summary>

public class MeetingSystem
{
    private static MeetingSystem instance = null;
    public static MeetingSystem Instance
    {
        get
        {
            if (instance == null)
			{
                instance = new MeetingSystem();
                return instance;
			}
            else
                return instance;
        }
    }

    public Meeting_View meetingView;
    public TestMeetingDate meetingDate;

    /// <summary>
    /// 该用户的会议列表
    /// </summary>
    public List<Meeting> myMeetingList { get; private set; }

    public int userID { get; private set; }

	public MeetingSystem()
	{
        myMeetingList = new List<Meeting>();
        userID = 11111;
	}

    /// <summary>
    /// 获取会议列表
    /// </summary>
    /// <param name="meetings">会议列表</param>
    public void SetMeetingList(List<Meeting> meetings)
	{
        myMeetingList = new List<Meeting>();
        myMeetingList.AddRange(meetings);
        ArrangeMeetingList();
        TryToRefreshView();
    }

    /// <summary>
    /// 返还会议列表
    /// </summary>
    /// <returns>会议列表</returns>
    public List<Meeting> GetMeetingList()
    {
        ArrangeMeetingList();
        return myMeetingList;
	}

    /// <summary>
    /// 根据若干个日期返还当中会议列表
    /// </summary>
    /// <returns>会议列表</returns>
    public List<Meeting> GetMeetingListByDate(params System.DateTime[] dateTimes)
    {
        List<System.DateTime> dates = new List<System.DateTime>(dateTimes);
        dates.Sort();
        List<Meeting> meetingsByDate = new List<Meeting>();
        if (dateTimes.Length == 0) return meetingsByDate;
        ArrangeMeetingList();
        for(int i = 0, curDate = 0;i<myMeetingList.Count&& curDate < dateTimes.Length; i++)
		{
            Meeting meet = myMeetingList[i];
            if (meet.startDt.Date == dateTimes[curDate].Date)
			{
                meetingsByDate.Add(meet);
			}
			else if(meet.startDt.Date > dateTimes[curDate].Date)
			{
                curDate++;
                i--;
			}
		}
        return meetingsByDate;
    }

    /// <summary>
    /// 添加若干会议
    /// </summary>
    /// <param name="Meetings"></param>
    public void AddMeetings(params Meeting[] Meetings)
	{
        myMeetingList.AddRange(Meetings);
        ArrangeMeetingList();
        TryToRefreshView();
    }
    /// <summary>
    /// 移除一个会议
    /// </summary>
    /// <param name="meeting">会议</param>
    public void RemoveMeeting(Meeting meeting)
	{
        Debug.Log("移除名为" + meeting.title + "的会议");
        myMeetingList.Remove(meeting);
        ArrangeMeetingList();
        TryToRefreshView();
    }

    /// <summary>
    /// 整理会议列表并进行排序
    /// 0：按起始时间顺序
    /// 1：按起始时间逆序
    /// </summary>
    /// <param name="mode">排序模式</param>
    void ArrangeMeetingList()
	{
        for (int i = 0; i < myMeetingList.Count; i++)
        {
            Meeting meeting = myMeetingList[i];

            // 剔除结束时间超过的
            if (meeting.endDt <= System.DateTime.Now)
                myMeetingList.RemoveAt(i--);

            // 更改状态
            if (meeting.startDt <= System.DateTime.Now)
                meeting.roomStatus = 2;
            else
                meeting.roomStatus = 1;

            // 剔除不包含自身的

        }
        // 根据时间排序
        myMeetingList = myMeetingList.OrderBy(s => s.startDt).ToList<Meeting>();
    }

 //   /// <summary>
 //   /// 整理会议列表并进行剔除
 //   /// </summary>
 //   void CheckMeetingList()
	//{
 //       for(int i =0;i< myMeetingList.Count; i++)
	//	{
 //           Meeting meeting = myMeetingList[i];

 //           // 剔除结束时间超过的
 //           if(meeting.endDt<= System.DateTime.Now)
 //               myMeetingList.RemoveAt(i--);

 //           // 更改状态
 //               if (meeting.startDt <= System.DateTime.Now)
 //                   meeting.roomStatus = 2;
 //               else
 //                   meeting.roomStatus = 1;
            
 //           // 剔除不包含自身的

 //       }
 //   }

    /// <summary>
    /// 尝试更新View
    /// </summary>
    void TryToRefreshView()
	{
		if (meetingView != null)
		{
            meetingView.RefreshMeetingView();
		}
	}
}
