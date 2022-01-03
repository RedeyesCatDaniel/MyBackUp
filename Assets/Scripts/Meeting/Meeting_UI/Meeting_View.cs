using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Meeting_View:MonoBehaviour
{
	/// <summary>
	/// 会议与UI的对应字典
	/// </summary>
	Dictionary<GameObject, Meeting> Dic_MeetingUI;

	/// <summary>
	/// 会议列表面板
	/// </summary>
	public MeetingListLayoutGroup meetingListLayout;


	public GameObject OriMeetingGO;
	public GameObject OriDateGo;

	public List<GameObject> MeetingGOPool;
	public List<GameObject> DateGOPool;

	bool isInitilized =false;				//是否初始化过

	private void OnEnable()
	{
		if (!isInitilized) return;
		MeetingSystem.Instance.meetingView = this;
		RefreshMeetingView();
	}
	private void Start()
	{
		MeetingGOPool = new List<GameObject>();
		DateGOPool = new List<GameObject>();
		OriMeetingGO.SetActive(false);
		OriDateGo.SetActive(false);
		MeetingGOPool.Add(OriMeetingGO);
		DateGOPool.Add(OriDateGo);

		isInitilized = true;
		RefreshMeetingView();

	}
	private void OnDisable()
	{
		MeetingSystem.Instance.meetingView = null;
	}





	/// <summary>
	/// 获取会议列表，生成并整理产生UI，将UI与会议一一对应
	/// </summary>
	/// <param name="meetings">会议列表</param>
	public void RefreshMeetingView()
	{
		List<Meeting> meetings = MeetingSystem.Instance.GetMeetingList();
		// 重置状态
		Dic_MeetingUI = new Dictionary<GameObject, Meeting>();
		foreach (GameObject date in DateGOPool)
		{
			date.SetActive(false);
		}
		foreach (GameObject meet in MeetingGOPool)
		{
			meet.SetActive(false);
		}
		if (meetings.Count == 0) return;

		// 生成或激活一个会议UI，将与之Meeting对应
		for (int i = 0; i < meetings.Count; i++)
		{
			if (Dic_MeetingUI.ContainsValue(meetings[i])) continue;
			GameObject newMeetingUI;
			if (i > MeetingGOPool.Count - 1)
			{
				newMeetingUI = Instantiate<GameObject>(OriMeetingGO, meetingListLayout.transform);
				MeetingGOPool.Add(newMeetingUI);
			}
			else
				newMeetingUI = MeetingGOPool[i];
			InitializedMeetingGo(newMeetingUI, meetings[i]);
			Dic_MeetingUI.Add(newMeetingUI, meetings[i]);
		}

		// 根据会议列表时间戳整理时间

		// 整理时间UI
		GameObject dateGo;
		if (DateGOPool.Count > 0)
			dateGo = DateGOPool[0];
		else
		{
			dateGo = GameObject.Instantiate<GameObject>(OriDateGo, meetingListLayout.transform);
			DateGOPool.Add(dateGo);
		}
		InitializeDateGo(dateGo, meetings[0].startDt);
		dateGo.transform.SetSiblingIndex(0);
		if (meetings.Count > 1)
		{
			int group = 0;
			int dateNum = 1;
			for (int i = 1; i < meetings.Count; i++)
			{
				if (meetings[i].startDt.Date != meetings[group].startDt.Date)
				{
					if (DateGOPool.Count > dateNum)
						dateGo = DateGOPool[dateNum];
					else
					{
						dateGo = GameObject.Instantiate<GameObject>(OriDateGo, meetingListLayout.transform);
						DateGOPool.Add(dateGo);
					}
					InitializeDateGo(dateGo, meetings[i].startDt);
					dateGo.transform.SetSiblingIndex(dateNum + i);
					dateNum++;
					group = i;
				}
			}
		}

		// 调整UI
		meetingListLayout.Refresh();
	}

	/// <summary>
	/// 根据UI移除对应会议
	/// </summary>
	public void RemoveMeeting(GameObject UI)
	{
		// 向System更新会议列表
		MeetingSystem.Instance.RemoveMeeting(Dic_MeetingUI[UI]);
		// 更新会议UI字典
		Dic_MeetingUI.Remove(UI);
		Destroy(UI);
		// 按时间戳重新整理

		// 调整UI
		meetingListLayout.Refresh();
	}

	/// <summary>
	/// 根据若干个会议移除对应UI
	/// </summary>
	/// <param name="meetings"></param>
	public void RemoveMeetingFormSystem(params Meeting[] meetings)
	{
		// ？？？
	}

	/// <summary>
	/// 会议UI初始化赋值
	/// </summary>
	void InitializedMeetingGo(GameObject meetingGo,Meeting meeting)
	{
		meetingGo.SetActive(true);
		meetingGo.transform.Find("Info/Tag/Title").GetComponent<Text>().text = meeting.title;
		meetingGo.transform.Find("Info/TimeTxt").GetComponent<Text>().text = meeting.startDt.ToShortTimeString() + "~" + meeting.endDt.ToShortTimeString();
		switch (meeting.roomStatus)
		{
			case 01:
				meetingGo.transform.Find("JoinBtn").gameObject.SetActive(false);
				meetingGo.transform.Find("Info/Tag/State").gameObject.SetActive(false);
				break;
			case 02:
				meetingGo.transform.Find("JoinBtn").gameObject.SetActive(true);
				meetingGo.transform.Find("Info/Tag/State").gameObject.SetActive(true);
				break;
			default:
				meetingGo.transform.Find("JoinBtn").gameObject.SetActive(false);
				meetingGo.transform.Find("Info/Tag/State").gameObject.SetActive(false);
				break;
		}

	}
	
	/// <summary>
	/// 日期UI初始化
	/// </summary>
	void InitializeDateGo(GameObject dateGo, System.DateTime dateTime)
	{
		dateGo.SetActive(true);
		string DateString = "";
		if(dateTime.Date == System.DateTime.Now.Date)
		{
			DateString = "Today - ";
		}
		DateString += dateTime.Year.ToString()+"."+ dateTime.Month.ToString() + "."+dateTime.Day.ToString();
		dateGo.GetComponentInChildren<Text>().text = DateString;
	}
}
