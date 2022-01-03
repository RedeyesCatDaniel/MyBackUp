using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMeeting
{
    /// <summary>
    /// 对话对象
    /// </summary>
    public Chara chara { get; private set; }
    /// <summary>
    /// 对应视频框物体
    /// </summary>
    public GameObject ConverGO { get; private set; }
    /// <summary>
    /// 对话ID
    /// </summary>
    public int MeetingID { get; private set; }

    /// <summary>
    /// 待添加属性：
    /// 视频
    /// 音频
    /// 对话地点（座位房间编号等）
    /// </summary>


    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="chara">对话对象</param>
    public void Initailize(GameObject ConverGO,Chara chara)
	{
        this.ConverGO = ConverGO;
        this.chara = chara;
	}
}
