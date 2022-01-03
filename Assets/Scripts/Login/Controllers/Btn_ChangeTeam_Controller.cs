using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice;

public class Btn_ChangeTeam_Controller
{
    //public delegate void My_delegate(); // 声明一个委托类型
    //public My_delegate my_delegate;     // 声明了委托类型的实例
    //public void Test_Login(My_delegate my_del)
    //{
    //    my_delegate = my_del;
    //    my_delegate();
    //}

    private static Btn_ChangeTeam_Controller instance;
    public static Btn_ChangeTeam_Controller Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Btn_ChangeTeam_Controller();
            }
            return instance;
        }
    }

    // 按钮事件方法_正确状态时展开面板
    public void Unfold_TeamName_Correctstatu()
    {
        // 如果错误的用户状态边框是打开的，关闭
        if (Btn_ChangeTeam_View.Instance.obj_ErrorUserName.activeSelf)
            Btn_ChangeTeam_View.Instance.obj_ErrorUserName.SetActive(false);

        // 点击时如果面板是打开的，则关闭。反之亦然
        if (Btn_ChangeTeam_View.Instance.Obj_TeamNamePanel.activeSelf == false)
            Btn_ChangeTeam_View.Instance.Obj_TeamNamePanel.SetActive(true);
        else 
            Btn_ChangeTeam_View.Instance.Obj_TeamNamePanel.SetActive(false);


    }

    // 按钮事件方法_错误状态时展开面板
    public void Unfold_TeamName_Errorstatu()
    {
        // 面板打开
        Btn_ChangeTeam_View.Instance.Obj_TeamNamePanel.SetActive(true);
        // 错误按钮关闭
        Btn_ChangeTeam_View.Instance.Obj_UnfoldTeamName_Error.SetActive(false);
        // 正确按钮打开，恢复原样
        Btn_ChangeTeam_View.Instance.Obj_UnfoldTeamName_Correct.SetActive(true);

        // 如果错误的用户状态边框是打开的，关闭
        if (Btn_ChangeTeam_View.Instance.obj_ErrorUserName.activeSelf)
            Btn_ChangeTeam_View.Instance.obj_ErrorUserName.SetActive(false);

    }
}
