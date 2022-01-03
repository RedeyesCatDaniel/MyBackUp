using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 在展开的面板里选择需要的团队名，脚本放在展开TeamPanel的面板上
public class ChangeTeam_View : MonoBehaviour
{
    public TMP_InputField input_search;  // 输入框---用于监听
   
    public GameObject root; // 预制体父节点

    public static ChangeTeam_View Instance;

    public GameObject ErrorTeamName; // 如果List为空，显示红色框
    public TextMeshProUGUI noUserName; // 错误状态TeamName底部提示框

    private bool controlTeamNameList;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // FirstInitialize(); // 将预制体加载到场景

        controlTeamNameList = true;

        // 输入框变化时
        input_search.onValueChanged.AddListener(delegate {  ValueChangeCheck(); });
    }

    // 将预制体加载到场景
    void FirstInitialize() 
    {
        if (UIPanelData_Model.Instance.my_Team.Count == 0)
        {
            ErrorTeamName.SetActive(true);
            noUserName.text = "Loading, please wait";
            return;
        }

        for (int i = 0; i < UIPanelData_Model.Instance.my_Team.Count; i++)
        {
            if (ErrorTeamName.activeSelf == true)
            {
                ErrorTeamName.SetActive(false);
                noUserName.text = "팀선택을 하지 않았습니다";
            }

            // 加载预设体资源
            GameObject my_prefab = (GameObject)Resources.Load("Prefabs/UI/" + "TeamName");
            // 实例化预制体并设置父对象
            my_prefab = Instantiate(my_prefab, root.transform);
            // 预制体改名
            my_prefab.name = UIPanelData_Model.Instance.my_Team[i].TeamName;
            // 加入列表
            ChangeTeam_Controller.Instance.prefabList1.Add(my_prefab);
            // 预制体子物体 Text 更改 string 数据
            my_prefab.GetComponent<Click_TeamName_View>().tMP.text = UIPanelData_Model.Instance.my_Team[i].TeamName;  
        }

        ChangeTeam_Controller.Instance.TMPs.AddRange(root.transform.GetComponentsInChildren<TextMeshProUGUI>());

        controlTeamNameList = false;
    }

    // 输入框变化
    private void ValueChangeCheck()
    {
        ChangeTeam_Controller.Instance.ValueChangeCheck();
    }

    void Update()
    {
        if (controlTeamNameList)
        {
            FirstInitialize();
        }
    }
}
