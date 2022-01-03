using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 展开 TeamNameList 面板
public class Btn_ChangeTeam_View : MonoBehaviour
{
    public Button Btn_UnfoldTeamName_Correct;     // 点击此按钮，可以展开 TeamName 面板
    public Button Btn_UnfoldTeamName_Error;       // 点击此按钮，可以展开 TeamName 面板(错误状态)
    public GameObject Obj_UnfoldTeamName_Correct; // Obj_正确状态的按钮（同上）
    public GameObject Obj_UnfoldTeamName_Error;   // Obj_错误状态的按钮（同上）

    public GameObject Obj_TeamNamePanel;         // TeamName 面板

    public GameObject obj_ErrorUserName; // 错误状态的用户名输入红色边框

    public static Btn_ChangeTeam_View Instance;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 错误状态和正确状态作用一样，都是为了展开面板
        Btn_UnfoldTeamName_Correct.onClick.AddListener(Unfold_TeamName_Correctstatu);
        Btn_UnfoldTeamName_Error.onClick.AddListener(Unfold_TeamName_Errorstatu);
    }

    // 正确按钮点击
    void Unfold_TeamName_Correctstatu()
    {
        Btn_ChangeTeam_Controller.Instance.Unfold_TeamName_Correctstatu();
    }

    // 错误按钮点击
    void Unfold_TeamName_Errorstatu()
    {
        Btn_ChangeTeam_Controller.Instance.Unfold_TeamName_Errorstatu();
    }
}
