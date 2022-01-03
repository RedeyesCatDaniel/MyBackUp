using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Click_TeamName_View : MonoBehaviour
{
    public TextMeshProUGUI tMP;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ClickTeam);
    }

    void ClickTeam()
    {
        Click_TeamName_Controller.Instance.ClickTeam(tMP);
        #region
        //string team = TMP.GetComponent<TextMeshProUGUI>().text;

        //// 将变色功能取消
        //team = team.Replace("<color=blue><b>", "");
        //team = team.Replace("</b></color>", "");
        //// 将 Team 的名字赋值给 Text 框
        //GameObject.Find("팀선택 [444:6569]").GetComponent<TextMeshProUGUI>().text = team;
        //// 关闭面板
        //GameObject.Find("team_select_dropdown [451:6229]").SetActive(false);
        #endregion
    }
}
