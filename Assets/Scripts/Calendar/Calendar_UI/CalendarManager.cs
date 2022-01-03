using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarManager : MonoBehaviour
{
    public GameObject CalendarGO;
    public GameObject MeetingListGO;

    /// <summary>
    /// 当前日历显示的月份
    /// </summary>
    public System.DateTime curMonth;

    bool isInitailized = false;

    Transform DateTrans;
    Transform MonthTrans;
    
    // Start is called before the first frame update
    void Start()
    {
        // 数据的初始化
        curMonth = System.DateTime.Now;

        // 会议列表面板的初始化
        MeetingListGO.SetActive(false);

        // 初始化日期面板
       

        // 初始化完毕
        isInitailized = true;

        // 更新View
        RefreshCalendar();

    }

	private void OnEnable()
	{
        if(isInitailized)
            RefreshCalendar();
	}

    /// <summary>
    /// 更新日历视图
    /// </summary>
    public void RefreshCalendar()
	{
        int DaysLength = DateTimeTool.GetDaysInMonth(curMonth);
        int FirstInWeek = (int)DateTimeTool.GetFirstInWeek(curMonth);
	}
}
