using LGUVirtualOffice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LGUVirtualOffice.Framework;

public class Click_Login_View : AbstractController
{
    private UserInfo my_Info;            // 用户

    public Button btn_Login;              // 登录按钮
    public TMP_InputField text_UserName;  // 用户名_input
    public TextMeshProUGUI text_TeamName; // Team名
    public TextMeshProUGUI inputTip_Text;  // 用户名输入提示框_Obj

    public Button btn_deleteUserName;  // 用户输入框右侧 X 删除按钮

    public GameObject errorUserName;  // 错误状态的用户输入边框
    public GameObject errorTeamName;  // 错误状态的Team按钮边框
    public GameObject TeamName;       // 正确状态的Team按钮边框

    public TextMeshProUGUI noUserName; // 错误状态用户底部提示框

    private bool controlValueChange;   // 用于临时关闭输入框变化

    #region
    //public UnityAction myAction;
    //public UnityEvent myEvent;
    #endregion

    public static Click_Login_View Instance;
    private Click_Login_Controller click_Login_Controller;

    public int ErrorUserNameStatu= (int)ErrorUserNameStatus.Zero;
    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        text_UserName.onValueChanged.AddListener(delegate { ValueChangeCheck(); });       // 输入框变化监听
        btn_Login.onClick.AddListener(Onclick_Login);
        btn_deleteUserName.onClick.AddListener(DeleteUser);
        click_Login_Controller = this.GetService<Click_Login_Controller>();
        this.SubscribeEvent<UserLoginFailedEvent>(OnUserLoginFailed).UnSubScribeWhenGameObjectDestroyed(gameObject);
        #region
        ////添加监听方法1:将myAction和SendEvent方法绑定
        //myAction = new UnityAction(SendEvent);
        ////为Event添加监听者myAction
        //myEvent.AddListener(myAction);
        #endregion
    }

    void SendEvent()
    {
        ErrorUserNameStatu =(int) ErrorUserNameStatus.One;
       
    }

    // 输入框变化监听
    private void ValueChangeCheck()
    {
        click_Login_Controller.ValueChangeCheck(errorUserName, errorTeamName,
            controlValueChange, inputTip_Text.text, text_UserName.text);
        #region
        //if (!controlValueChange)
        //{
        //    if (errorUserName.activeSelf)
        //        errorUserName.SetActive(false);

        //    // 用户名提示输入关闭颜色变化
        //    if (inputTip_Text.text.Contains("<color=red>"))
        //    {
        //        // Debug.Log("用户名提示输入关闭颜色变化");
        //        inputTip_Text.text = inputTip_Text.text.Replace("<color=red>", "");
        //        inputTip_Text.text = inputTip_Text.text.Replace("</color>", "");
        //    }

        //    // 用户名输入关闭颜色变化
        //    if (text_UserName.text.Contains("<color=red>"))
        //    {
        //        // Debug.Log("用户名输入关闭颜色变化");
        //        text_UserName.text = text_UserName.text.Replace("<color=red>", "");
        //        text_UserName.text = text_UserName.text.Replace("</color>", "");
        //    }
        //}
        #endregion
    }

    private void Onclick_Login()
    {
        click_Login_Controller.Onclick_Login(TeamName, errorUserName, errorTeamName,
                                                 my_Info, controlValueChange,
                                                 text_TeamName.text, text_UserName.text,
                                                 inputTip_Text.text, noUserName.text);
        #region
        //// 登录前把所有 Error 窗口关闭
        //errorUserName.SetActive(false);
        //errorTeamName.SetActive(false);

        //// 每一次登录都创建一个对象
        //my_Info = new UserInfo()
        //{
        //    TeamInfo = new TeamModel()
        //};

        //// 选择了组织
        //if (text_TeamName.text != "팀 선택")
        //{
        //    if (text_UserName.text.Contains("<color=red>"))
        //    {
        //        text_UserName.text = text_UserName.text.Replace("<color=red>", "");
        //        text_UserName.text = text_UserName.text.Replace("</color>", "");
        //    }
        //    my_Info.UserName = text_UserName.text;
        //    my_Info.TeamInfo.TeamName = text_TeamName.text;
        //    my_Info.TeamInfo.TeamCode = Btn_ChangeTeam.Instance.dicTeam[text_TeamName.text];
        //}

        //// 接受网络服务器判断的是否登录成功
        //bool login = logLoginManager.Instance.Login(my_Info).OperationSucceed;

        //// 1、未选择组织名
        //if (text_TeamName.text == "팀 선택")
        //{
        //    TeamName.SetActive(false);
        //    errorTeamName.SetActive(true);
        //}
        //// 2、未输入用户名
        //if (text_UserName.text == "")
        //{
        //    errorUserName.SetActive(true);
        //    inputTip_Text.text = inputTip_Text.text.Replace(inputTip_Text.text, "<color=red>" + inputTip_Text.text + "</color>");  // 用户名提示输入字体变红
        //    noUserName.text = "이름을 입력하지 않았습니다.";
        //}
        //// 3、用户名错误
        //if (!login && text_TeamName.text != "팀 선택" && text_UserName.text != "")
        //{
        //    errorUserName.SetActive(true);
        //    controlValueChange = true;  // 临时关闭变化
        //    text_UserName.text = text_UserName.text.Replace(text_UserName.text, "<color=red>" + text_UserName.text + "</color>");  // 字体变红
        //    controlValueChange = false;
        //    noUserName.text = "일치하는 이름이 없습니다. 다시 입력해 주세요.";
        //}
        //// 4、登录成功"
        //if (login)
        //{
        //    Debug.Log("登录成功");
        //}
        #endregion
    }

    /// <summary>
    /// 点击 X 号，删除用户名
    /// </summary>
    private void DeleteUser()
    {
        text_UserName.text = "";
    }


    enum ErrorUserNameStatus { Zero,One,Two};

    void Update()
    {
        #region
        //switch (ErrorUserNameStatu)
        //{
        //    case (int)ErrorUserNameStatus.Zero:

        //        break;
        //    case (int)ErrorUserNameStatus.One:
        //        //Debug.Log("用户名错误");
        //        //Debug.Log(errorUserName.activeSelf.ToString());
        //        // 3、用户名错误
        //        if (text_TeamName.text != "팀 선택" && text_UserName.text != "")
        //        {
        //            //Debug.Log("用户名错误");
        //            //Debug.Log("开关" + errorUserName.activeSelf);
        //            errorUserName.SetActive(true);
        //            // Debug.Log("开" + errorUserName.activeSelf);
        //            controlValueChange = true;  // 临时关闭变化
        //            text_UserName.text = text_UserName.text.Replace(text_UserName.text, "<color=red>" + text_UserName.text + "</color>");  // 字体变红
        //            controlValueChange = false;
        //            noUserName.text = "일치하는 이름이 없습니다. 다시 입력해 주세요.";
        //        }
        //        Debug.Log("用户名..");
        //        break;


        //    case (int)ErrorUserNameStatus.Two:

        //        break;
        //    default:
        //        break;
        //}
        #endregion
    }

    private void OnUserLoginFailed(UserLoginFailedEvent e)
    {
        #region
        ////触发myEvent事件
        //Click_Login_View.Instance.myEvent.Invoke();
        #endregion
        // 3、用户名错误
        if (text_TeamName.text != "팀 선택" && text_UserName.text != "")
        {
            errorUserName.SetActive(true);
            controlValueChange = true;  // 临时关闭变化
            text_UserName.text = text_UserName.text.Replace(text_UserName.text, "<color=red>" + text_UserName.text + "</color>");  // 字体变红
            controlValueChange = false;
            noUserName.text = "일치하는 이름이 없습니다. 다시 입력해 주세요.";
        }
    }
}

