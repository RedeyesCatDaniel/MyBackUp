using LGUVirtualOffice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 获取从服务器传来的 TeamNameList
public class GetServerTeamNameList_Model : MonoBehaviour
{
    // 创建字典用于存储 TeamName , TeamCode
    public Dictionary<string, string> dicTeam = new Dictionary<string, string>();

    public static GetServerTeamNameList_Model Instance;

    void Awake()
    {
        Instance = this;
        // 获取数据列表
        var result =logDBManager.Instance.GetTeamList();

        result.OnSuccess((teamList) => {
            UIPanelData_Model.Instance.my_Team = teamList;

            // 将 TeamName，TeamCode 存储起来
            for (int i = 0; i < UIPanelData_Model.Instance.my_Team.Count; i++)
            {
                dicTeam.Add(UIPanelData_Model.Instance.my_Team[i].TeamName,
                            UIPanelData_Model.Instance.my_Team[i].TeamCode);
            }
        });
        result.OnFailed((errorMessage)=> {
            
        });

       

       
    }

    private void Test(List<TeamModel> teamList) { 
            
    }
    
}
