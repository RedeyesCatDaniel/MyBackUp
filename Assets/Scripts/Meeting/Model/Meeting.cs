using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meeting
{
    /// <summary>
    /// 会议编号
    /// </summary>
    public string roomNo { get; private set; }
    /// <summary>
    /// 会议标题
    /// </summary>
    public string title { get; private set; }
    /// <summary>
    /// 参与人员
    /// </summary>
    public Chara[] charas { get; private set; }
    /// <summary>
    /// 发起人
    /// </summary>
    public Chara creater { get; private set; }
    // 时间格式 YYYY-MM-DD hh:mm:ss 
    /// <summary>
    /// 开始时间
    /// </summary>
    public System.DateTime startDt { get; private set; }
    /// <summary>
    /// 结束时间
    /// </summary>
    public System.DateTime endDt { get; private set; }
    /// <summary>
    /// 会议状态：01.待机中，02.进行中，03.结束
    /// </summary>
    public int roomStatus { get; set; }

    public Meeting(string roomNo,string title,string startDt,string endDt)
	{
        this.roomNo = roomNo;
        this.title = title;
        this.startDt = DateTimeTool.GetDateTime(startDt);
        this.endDt = DateTimeTool.GetDateTime(endDt);
	}

}
