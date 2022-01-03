using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 在展开的面板里选择需要的团队名
public class ChangeTeam_Controller
{
    private static ChangeTeam_Controller instance;
    public static ChangeTeam_Controller Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChangeTeam_Controller();
            }
            return instance;
        }
    }

    public List<TextMeshProUGUI> TMPs = new List<TextMeshProUGUI>(); // 创建列表储存所有的Text
    public List<GameObject> prefabList1 = new List<GameObject>();  
    public List<GameObject> prefabList2 = new List<GameObject>();

    // 输入框变化
    public void ValueChangeCheck()
    {
        // 下面两个for循环是为了初始化
        // 预制体全部显示开启
        for (int i = 0; i < prefabList1.Count; i++)
        {
            prefabList1[i].SetActive(true);
        }
        // 关闭颜色变化
        for (int i = 0; i < TMPs.Count; i++)
        {
            TMPs[i].text = TMPs[i].text.Replace("<color=blue><b>", "");
            TMPs[i].text = TMPs[i].text.Replace("</b></color>", "");
        }

        if (ChangeTeam_View.Instance.input_search.text != "")
        {
            // 将搜索中出现的重复项给到My_Team
            for (int i = 0; i < TMPs.Count; i++)
            {
                #region
                // 关闭颜色变化————11.24临时注释，目测可弃用
                //ChangeTeam_view.TMPs[i].text = ChangeTeam_view.TMPs[i].text.Replace("<color=blue><b>", "");
                //ChangeTeam_view.TMPs[i].text = ChangeTeam_view.TMPs[i].text.Replace("</b></color>", "");
                #endregion

                // 如果在Text列表中
                if (TMPs[i].text.Contains(ChangeTeam_View.Instance.input_search.text))
                {
                    // 字体变色
                    TMPs[i].text = TMPs[i].text.Replace(ChangeTeam_View.Instance.input_search.text, "<color=blue><b>" + ChangeTeam_View.Instance.input_search.text + "</b></color>");
                    // 将这个预制体加入prefabList2
                    prefabList2.Add(TMPs[i].transform.parent.gameObject);
                    // 将这个预制体移除prefabList1
                    prefabList1.Remove(TMPs[i].transform.parent.gameObject);
                }
            }
        }
        UpdataInitialize();
    }


    // 每一次更新
    private void UpdataInitialize()
    {
        if (prefabList2.Count != 0)
        {
            // 非匹配的全部关闭---prefabList1列表
            for (int i = 0; i < prefabList1.Count; i++)
            {
                prefabList1[i].SetActive(false);
            }
        }

        // 重置列表
        prefabList1.AddRange(prefabList2);
        prefabList2 = new List<GameObject>();

    }
}
