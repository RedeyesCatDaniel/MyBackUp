using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理视频通话相关内容
/// </summary>

public class ConversationSystem:MonoBehaviour
{
	
	int meetingID;

	/// <summary>
	/// 对话规则：0.无自动连线；1.自动连线所有人；2.自动连线靠近的人；3.自动连线大厅距离规则
	/// </summary>
	int ConverPolicy;
	/// <summary>
	/// 对话框物体
	/// </summary>
	List<GameObject> ConverGO;

	/// <summary>
	/// 客户端对应的角色
	/// </summary>
	public Chara Player { get; private set; }
	/// <summary>
	/// 可以进行连线的角色列表
	/// </summary>
	public List<Chara> CharasSameArea { get; private set; }
	/// <summary>
	/// 当前进行中的对话
	/// </summary>
	public List<SingleMeeting> CurMeetings { get; private set; }

	public ConversationSystem()
	{
		ConverPolicy = 0;
		ConverGO = new List<GameObject>();
		ConverGO.Add(this.transform.GetChild(0).gameObject);
		CharasSameArea = new List<Chara>();
		CurMeetings = new List<SingleMeeting>();
	}

	/// <summary>
	/// 检测可连线角色中是否有符合距离的角色
	/// </summary>
	public void CheckAround()
	{
		switch (ConverPolicy)
		{
			case 0:
				break;
			case 1:
				foreach (Chara chara in CharasSameArea)
				{
					// 进行判断是否可以会面
					if (chara.isAbleToConncent == false)
						continue;
					if (true)
					{
						StartAMeeting(chara);
					}
				}
				break;
			case 2:
				foreach (Chara chara in CharasSameArea)
				{
					// 进行判断是否可以会面
					if (chara.isAbleToConncent == false)
						continue;
					if (true)
					{
						StartAMeeting(chara);
					}
				}
				break;
			case 3:
				foreach (Chara chara in CharasSameArea)
				{
					// 进行判断是否可以会面
					if (chara.isAbleToConncent == false)
						continue;
					if (true)
					{
						StartAMeeting(chara);
					}
				}
				break;
			default:
				break;
		}
		// 整理视频框位置
		SuffleMeetings();
	}
	/// <summary>
	/// 判断单个角色是否可进行连线，如果可以，进行会议连线尝试
	/// </summary>
	/// <param name="chara"></param>
	void CheckSingleChara(Chara chara)
	{
		
	}
	/// <summary>
	/// 开始一场对话
	/// </summary>
	/// <param name="chara">对话对象</param>
	void StartAMeeting(Chara chara)
	{
		SingleMeeting newMeeting = new SingleMeeting();
		if (CurMeetings.Count == 0)
			this.ConverGO[0].SetActive(true);
		GameObject ConverGO = Instantiate<GameObject>(this.ConverGO[0], this.ConverGO[0].transform);
		newMeeting.Initailize(ConverGO, chara);
		CurMeetings.Add(newMeeting);
		CharasSameArea.Remove(chara);
	}

	/// <summary>
	/// 整理meeting列表
	/// </summary>
	void SuffleMeetings()
	{

	}
}
