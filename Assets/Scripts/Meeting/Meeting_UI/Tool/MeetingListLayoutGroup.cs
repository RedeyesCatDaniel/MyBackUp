using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 应对Meeting中，两种子物体同时出现的自动布局
/// </summary>

public class MeetingListLayoutGroup : MonoBehaviour
{
    [HeaderAttribute("子物体1")]
    public GameObject Child1;

    [HeaderAttribute("子物体1的高度")]
    public float Height1=23;

    [HeaderAttribute("子物体2")]
    public GameObject Child2;

    [HeaderAttribute("子物体2的高度")]
    public float Height2=88;


    private int childrenCount;              //子物体数量
    private int children1;                  //1类子物体的数量
    private int children2;                  //2类子物体的数量
    private int[] childrenType;             //子物体类型列表
    private RectTransform oriPanelRT;

	private void Start()
	{
        childrenCount = 0;
        oriPanelRT = transform.GetComponent<RectTransform>();
	}


	/// <summary>
	/// 对此面板排版布局进行动态调整
	/// </summary>
	public void Refresh()
	{
        // 更新子物体数据
        childrenCount = 0;
        childrenType = new int[transform.childCount];
        children1 = children2 = 0;
		for (int id =0;id< transform.childCount; id++)
		{
            GameObject child = transform.GetChild(id).gameObject;
            if(!child.activeSelf)
			{
                childrenCount++;
                continue;
			}
			if (child.name.Contains(Child1.name))
			{
                childrenType[id] = 1;
                children1++;
			}
			else
			{
                childrenType[id] = 2;
                children2++;
			}
            childrenCount++;
		}

        // 判断调整面板长度
        float TargetHeight = Height1 * children1 + Height2 * children2;
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, TargetHeight);

        // 遍历调整子物体Rect数据
        float curHeight = 0;
        for(int i = 0; i < childrenCount; i++)
		{
            float height;
            if(childrenType[i] == 0)
                continue;
            else if (childrenType[i] == 1)
                height = Height1;
            else
                height = Height2;
            transform.GetChild(i).transform.localPosition = new Vector2(0, -curHeight);
            curHeight += height;
		}
	}
}
