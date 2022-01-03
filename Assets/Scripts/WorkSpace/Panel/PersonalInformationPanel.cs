using LGUVirtualOffice;
using LGUVirtualOffice.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
    /// <summary>
    /// User个人信息界面
    /// </summary>
    public class PersonalInformationPanel : AbstractController, IPointerEnterHandler, IPointerExitHandler
    {
        private bool controllerPanel;
        public void OnPointerEnter(PointerEventData eventData)
        {
            controllerPanel = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            controllerPanel = false;
        }

        public UserInfo my_UserInfo ;

        public TextMeshProUGUI text_UserName;
        public TextMeshProUGUI text_TeamName;
        public TextMeshProUGUI text_Siganature;
        public TextMeshProUGUI text_Position;

        public Image image_State;

        public Button btn_gofor;  // 去找他按钮
        public Button btn_char;   // 找他聊天按钮

        public GameObject character_Online;
        public GameObject character_Meeting;
        public GameObject character_Busy;
        public GameObject character_Offline;


        private IQueueMessageService queueMessageService;
        private JumpPosition jumpPosition;
        public static PersonalInformationPanel Instance;
        private void Awake()
        {
            this.gameObject.SetActive(false);
            Instance = this;
        }

        void Start()
        {
            Debug.Log("用户状态" + my_UserInfo.UserStatus.Value);
            Debug.Log("用户姓名" + my_UserInfo.UserName);
            Debug.Log("用户团队" + my_UserInfo.TeamInfo.TeamName);
            Debug.Log("用户签名" + my_UserInfo.Signature.Value);
            Debug.Log("用户位置" + my_UserInfo.WorkSpacenNowIn.Value);

            jumpPosition = this.GetService<JumpPosition>();

            my_UserInfo.WorkSpacenNowIn.Subscribe((newPosition) => // 获取新位置
            {
                text_Position.text = newPosition;
            });

            ImagePicture(my_UserInfo.UserStatus.Value);
            my_UserInfo.UserStatus.Subscribe((NewState) =>  // 获取新状态
            {
                ImagePicture(NewState);
            });

            my_UserInfo.Signature.Subscribe((newSigenature) =>  // 获取新签名
            {
                text_Siganature.text = newSigenature;
            });

            btn_gofor.onClick.AddListener(() => {
                Debug.Log("我要去找他");
                jumpPosition.Jump_Position(my_UserInfo.WorkSpacenNowIn.Value);
            });
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !controllerPanel)
            {
                this.gameObject.SetActive(false);
            }   
        }

        /// <summary>
        /// 对用户个人信息面板中的信息进行替换
        /// </summary>
        /// <param name="userInfo">点击不同用户所选择的不同--UserInfo</param>
        public void UpdatePersonalInformation(UserInfo userInfo)
        {
            text_UserName.text = userInfo.UserName;
            text_TeamName.text = userInfo.TeamInfo.TeamName;
            text_Siganature.text = userInfo.Signature.Value;
            text_Position.text = userInfo.WorkSpacenNowIn.Value;
            Debug.Log(1234);
            ImagePicture(userInfo.UserStatus.Value);
        }

        /// <summary>
        /// 个人信息面板出现的位置,依据Obj的位置确定
        /// </summary>
        /// <param name="obj">选中的那个物体</param>
        public void CreatePersonalInformationPanelPosition(GameObject obj)
        {
            float x = obj.GetComponent<RectTransform>().position.x;
            float y = obj.GetComponent<RectTransform>().position.y;
            if (this.gameObject.activeSelf == false)
                this.gameObject.SetActive(true);

            this.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(PanelPosition.Instance.ControllerCompleteView_UserMemberPersonalInformationPosition(x, y)[0],
                                                                                     PanelPosition.Instance.ControllerPersonalInformationPosition(x, y)[1], 0);
        }

        /// <summary>
        /// 加载预制体，所在团队的加载
        /// </summary>
        /// <param name="parent">预制体父物体</param>
        /// <param name="str_LoadPath">所加载预制体的路径</param>
        public void Instantiate_MyTeamMemberList_Prefabs(GameObject parent, string str_LoadPath, UserInfo my_UserInfo)
        {
            for (int i = 0; i < MemberUserInfo.Instance.TeamMemberList.Count; i++)
            {
                GameObject my_prefab = (GameObject)Resources.Load(str_LoadPath);
                if (MemberUserInfo.Instance.My_UserInfo.UserName != MemberUserInfo.Instance.TeamMemberList[i].UserName)
                {
                    // 实例化预制体并设置父对象
                    my_prefab = Instantiate(my_prefab, parent.transform);

                    my_prefab.name = MemberUserInfo.Instance.TeamMemberList[i].UserName;

                    my_UserInfo = MemberUserInfo.Instance.TeamMemberList[i];
                }
            }
        }

        private void ImagePicture(int status)
        {
            if (status == (int)UserStateEnum.Online)
            {
                image_State.sprite = Resources.Load<Sprite>(RequiredStringManager.str2_Online);
                btn_gofor.gameObject.SetActive(true);
                CharacterStateColor(character_Online);
            }
            else if (status == (int)UserStateEnum.Offline)
            {
                image_State.sprite = Resources.Load<Sprite>(RequiredStringManager.str2_Offline);
                btn_gofor.gameObject.SetActive(false);
                CharacterStateColor(character_Offline);
            }
            else if (status == (int)UserStateEnum.InMeeting)
            {
                image_State.sprite = Resources.Load<Sprite>(RequiredStringManager.str2_Meeting);
                btn_gofor.gameObject.SetActive(false);
                CharacterStateColor(character_Meeting);
            }
            else if (status == (int)UserStateEnum.Busy)
            {
                image_State.sprite = Resources.Load<Sprite>(RequiredStringManager.str2_Busy);
                btn_gofor.gameObject.SetActive(true);
                CharacterStateColor(character_Busy);
            }
        }

        private void CharacterStateColor(GameObject obj)
        {
            character_Online.SetActive(false);
            character_Meeting.SetActive(false);
            character_Busy.SetActive(false);
            character_Offline.SetActive(false);

            obj.SetActive(true);
        }
    }
}

   



