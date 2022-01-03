using LGUVirtualOffice;
using LGUVirtualOffice.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click_Login_Controller: AbstractService
{
    private ILoginService loginService;
    private INetworkSyncService photonService;
    private UserInfo localUser;
    private JumpPosition jumpPosition;

    public static string targetScene = "TransferStation";
    protected override void OnInit()
    {
        jumpPosition = this.GetService<JumpPosition>();
        loginService = this.GetService<ILoginService>();
        photonService = this.GetService<INetworkSyncService>();
        localUser = this.GetModel<UserInfo>();
        this.SubscribeEvent<UserLoginSuccessEvent>(OnUserLoginSuccess);
        this.SubscribeEvent<ConnectToServerSuccessEvent>(OnConnectToServerSuccess);
        this.SubscribeEvent<AllopatricLoginEvent>(OnAllopatricLogin);
    }

    // 输入框变化监听
    public void ValueChangeCheck(GameObject obj_errorUserName, GameObject obj_errorTeamName,
                                 bool controlValueChange,string inputTip_Text,string text_UserName )
    {
        if (!controlValueChange)
        {
            if (obj_errorUserName.activeSelf)
                obj_errorUserName.SetActive(false);

            // 用户名提示输入关闭颜色变化
            if (inputTip_Text.Contains("<color=red>"))
            {
                // Debug.Log("用户名提示输入关闭颜色变化");
                inputTip_Text = inputTip_Text.Replace("<color=red>", "");
                inputTip_Text = inputTip_Text.Replace("</color>", "");
            }

            // 用户名输入关闭颜色变化
            if (text_UserName.Contains("<color=red>"))
            {
                // Debug.Log("用户名输入关闭颜色变化");
                text_UserName = text_UserName.Replace("<color=red>", "");
                text_UserName = text_UserName.Replace("</color>", "");
            }
        }

    }

    // 用户点击登录时不同情况
    public void Onclick_Login(GameObject TeamName, GameObject errorUserName,GameObject errorTeamName, 
        UserInfo my_Info, bool controlValueChange,
        string text_TeamName ,string text_UserName,
        string inputTip_Text ,string noUserName)
    {
        my_Info = localUser;
        MemberUserInfo.Instance.My_UserInfo = localUser;
        // 登录前把所有 Error 窗口关闭
        errorUserName.SetActive(false);
        errorTeamName.SetActive(false);

        // 每一次登录都创建一个对象
        //my_Info = UserInfo.Instance;
        my_Info.TeamInfo = new TeamModel();

        // 选择了组织
        if (text_TeamName != "팀 선택")
        {
            if (text_UserName.Contains("<color=red>"))
            {
                text_UserName = text_UserName.Replace("<color=red>", "");
                text_UserName = text_UserName.Replace("</color>", "");
            }
            my_Info.UserName = text_UserName;
            my_Info.TeamInfo.TeamName = text_TeamName;
            my_Info.TeamInfo.TeamCode = GetServerTeamNameList_Model.Instance.dicTeam[text_TeamName];
        }

        // 1、未选择组织名
        if (text_TeamName == "팀 선택")
        {
            TeamName.SetActive(false);
            errorTeamName.SetActive(true);
        }
        // 2、未输入用户名
        if (text_UserName == "")
        {
            errorUserName.SetActive(true);
            inputTip_Text = inputTip_Text.Replace(inputTip_Text, "<color=red>" + inputTip_Text + "</color>");  // 用户名提示输入字体变红
            noUserName = "이름을 입력하지 않았습니다.";
        }

        loginService.Login(my_Info);
        #region
        // 登录成功
        /*result.OnSuccess((Success)=> {
            #region
            //Debug.Log("登录》》》");

            //// 连接服务器
            //PhotonManager.Instance.ConnectToServer(my_Info);
            //// 加入服务器
            //PhotonManager.Instance.JoinWorkSpace();

            //avSceneManagerToolkit.LoadScene("CharacterCustomization");
            //Debug.Log("成功》》》");
            #endregion
            Debug.Log("登录开始");
            // 连接服务器
            var result2 = PhotonManager.Instance.ConnectToServer(my_Info);
            result2.OnSuccess((Success) =>
            {
                Debug.Log("连接服务器成功");
                // 加入服务器
                *//*var result3 = PhotonManager.Instance.JoinWorkSpace();
                result3.OnSuccess((Success) =>
                {
                    Debug.Log("加入服务器成功");
                    avSceneManagerToolkit.LoadScene("CharacterCustomization");
                });*//*
                avSceneManagerToolkit.LoadScene("CharacterCustomization");
            });
            Debug.Log("登录成功");
            
            Debug.Log("加载场景");
        });*/

        // 登录失败
        /*result.OnFailed((Failed) => {

            #region
            ////触发myEvent事件
            //Click_Login_View.Instance.myEvent.Invoke();
            #endregion
            // 3、用户名错误
            if (text_TeamName != "팀 선택" && text_UserName != "")
            {
                errorUserName.SetActive(true);
                controlValueChange = true;  // 临时关闭变化
                text_UserName = text_UserName.Replace(text_UserName, "<color=red>" + text_UserName + "</color>");  // 字体变红
                controlValueChange = false;
                noUserName = "일치하는 이름이 없습니다. 다시 입력해 주세요.";
            }
        });*/



        #endregion
    }

    private void OnUserLoginSuccess(UserLoginSuccessEvent e) 
    {
        #region
        //Debug.Log("登录》》》");

        //// 连接服务器
        //PhotonManager.Instance.ConnectToServer(my_Info);
        //// 加入服务器
        //PhotonManager.Instance.JoinWorkSpace();

        //avSceneManagerToolkit.LoadScene("CharacterCustomization");
        //Debug.Log("成功》》》");
        #endregion
        Debug.Log("登录开始");

        localUser.UserStatus.Value = (int)UserStateEnum.Online;

        // 连接服务器
        photonService.ConnectToServer(localUser);        
        #region
        /*result2.OnSuccess((Success) =>
        {
            Debug.Log("连接服务器成功");
            // 加入服务器
            *//*var result3 = PhotonManager.Instance.JoinWorkSpace();
            result3.OnSuccess((Success) =>
            {
                Debug.Log("加入服务器成功");
                avSceneManagerToolkit.LoadScene("CharacterCustomization");
            });*//*
            avSceneManagerToolkit.LoadScene("CharacterCustomization");
        });*/
        #endregion
        Debug.Log("登录成功");

        Debug.Log("加载场景");
    }

    private void OnConnectToServerSuccess(ConnectToServerSuccessEvent e) 
    {
        Debug.Log("连接服务器成功");
        #region
        // 加入服务器
        /*var result3 = PhotonManager.Instance.JoinWorkSpace();
        result3.OnSuccess((Success) =>
        {
            Debug.Log("加入服务器成功");
            avSceneManagerToolkit.LoadScene("CharacterCustomization");
        });*/

        //localUser.GetTeamMateList(ServiceMemberInfo=> { 
        //    MemberUserInfo.Instance.TeamMemberList = ServiceMemberInfo;
        //    Debug.Log(MemberUserInfo.Instance.TeamMemberList.Count);

        //});
        #endregion

        //localUser.GetTeamMateList((S1) =>
        //{
        //    MemberUserInfo.Instance.TeamMemberList = S1;
        //    Debug.Log(MemberUserInfo.Instance.TeamMemberList.Count);
        //    avSceneManagerToolkit.LoadScene(targetScene);
        //});

        if (!photonService.IsChangeWorkspace())
        {
            //jumpPosition.Jump_Position(MemberUserInfo.Instance.My_UserInfo.TeamInfo.TeamName);
            localUser.GetTeamMateList((S1) =>
            {
                MemberUserInfo.Instance.TeamMemberList = S1;
                Debug.Log(MemberUserInfo.Instance.TeamMemberList.Count);
                avSceneManagerToolkit.LoadScene(targetScene);
            });
            ////此处代码用于测试 WorkSpace UI
            //localUser.GetTeamMateList((S1) =>
            //{
            //    MemberUserInfo.Instance.TeamMemberList = S1;
            //    Debug.Log(MemberUserInfo.Instance.TeamMemberList.Count);
            //    avSceneManagerToolkit.LoadScene("WorkSpace_Main_1228");
            //});
        }

    }
    private void OnAllopatricLogin(AllopatricLoginEvent e) 
    {
        LogUtil.LogDebug("Allopatric Login!");
        //log out ,clear the cache
        loginService.Logout();
    }
    // 点击 X 号，删除用户名
    //public void DeleteUser(string text_UserName)
    //{
    //    text_UserName = "";
    //}
}
