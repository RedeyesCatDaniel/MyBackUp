using LGUVirtualOffice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelData_Model 
{
    private static UIPanelData_Model instance;
    public static UIPanelData_Model Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIPanelData_Model();
            }
            return instance;
        }
    }

    public List<TeamModel> my_Team;// 创建 list 用于接收 从服务器传过来的list

    public UIPanelData_Model()
    {
        my_Team = new List<TeamModel>();
    }
}
