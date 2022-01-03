using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 左侧状态栏面板各个按钮的点击事件
    /// </summary>
    public class LeftStatusBarEachButton : AbstractController
    {
        public Button btn_PackUp;                   // 收起界面最下方按钮
        public Button btn_Unfold;                   // 展开界面最下方按钮
        public GameObject obj_LeftStatusBar_PackUp; // 收起状态状态栏
        public GameObject obj_LeftStatusBar_Unfold; // 展开状态状态栏


        public Button btn_linkman_PackUp;        // 联系人按钮_收起状态
        public Button btn_linkman_Unfold;        // 联系人按钮_展开状态
        public GameObject obj_linkman_Panel;     // 联系人（Member）面板

        public Button btn_ThreePoint;            // 收起界面三个点--按钮
        public Button btn_CompleteView;          // 收起界面完整试图--按钮
        public GameObject obj_CompleteView;      // 完整成员面板
        public GameObject obj_CompleteView_Up;   // 完整成员面板--上部分箭头
        public GameObject obj_CompleteView_Down; // 完整成员面板--下部分箭头

        public Button btn_ChangeStatus;     // 修改状态按钮
        public GameObject obj_ChangeStatus; // 修改状态面板

       
        void Start()
        {
            // 点击收起按钮
            btn_PackUp.onClick.AddListener(() => {
                OFFOrONGameObject(obj_LeftStatusBar_PackUp);
            });
            // 点击展开按钮
            btn_Unfold.onClick.AddListener(() => {
                OFFOrONGameObject(obj_LeftStatusBar_Unfold);
            });

            // 点击联系人按钮---收起状态
            btn_linkman_PackUp.onClick.AddListener(() => {
                obj_linkman_Panel.SetActive(true);// 面板打开

                float x = PanelPosition.Instance.ControlPosition(obj_LeftStatusBar_PackUp, RequiredStringManager.offset_linkman_Panel);
                float y = -250;
                obj_linkman_Panel.transform.localPosition = new Vector3(x, y, 0);
            });
            // 点击联系人按钮---展开状态
            btn_linkman_Unfold.onClick.AddListener(() => {
                obj_linkman_Panel.SetActive(true); // 面板打开

                float x = PanelPosition.Instance.ControlPosition(obj_LeftStatusBar_Unfold, RequiredStringManager.offset_linkman_Panel) ;
                float y = -250;
                obj_linkman_Panel.transform.localPosition = new Vector3(x, y, 0);
            });

            // 点击完整视图按钮---收起状态
            btn_ThreePoint.onClick.AddListener(() => {
                obj_CompleteView.SetActive(true);      // 面板打开
                UpOrDownGameObject(obj_CompleteView_Down);
               
                float x = PanelPosition.Instance.ControlPosition(obj_LeftStatusBar_PackUp, RequiredStringManager.offset_CompleteView_Unfold) ;
                float y = -150;
                obj_CompleteView.transform.localPosition = new Vector3(x, y, 0);
            });
            // 点击完整视图按钮---展开状态
            btn_CompleteView.onClick.AddListener(() => {
                obj_CompleteView.SetActive(true);       // 面板打开
                UpOrDownGameObject(obj_CompleteView_Up);

                float x = PanelPosition.Instance.ControlPosition(obj_LeftStatusBar_Unfold, RequiredStringManager.offset_CompleteView_PackUp);
                float y = -150;
                obj_CompleteView.transform.localPosition = new Vector3(x, y, 0);
                //obj_CompleteView.transform.position = new Vector3(btn_CompleteView.gameObject.transform.position.x, btn_CompleteView.gameObject.transform.position.y, 0);
            });

            // 点击修改状态按钮
            btn_ChangeStatus.onClick.AddListener(() => {
                obj_ChangeStatus.SetActive(true);
            });
        }
        
        /// <summary>
        /// 展开状态面板和收起状态面板之间的切换
        /// </summary>
        /// <param name="obj"></param>
        private void OFFOrONGameObject(GameObject obj)
        {
            obj_LeftStatusBar_PackUp.SetActive(false);
            obj_LeftStatusBar_Unfold.SetActive(false);

            obj.SetActive(true);
        }

        /// <summary>
        /// 完整视图中上下箭头的切换
        /// </summary>
        /// <param name="obj"></param>
        private void UpOrDownGameObject(GameObject obj)
        {
            obj_CompleteView_Up.SetActive(false);
            obj_CompleteView_Down.SetActive(false);

            obj.SetActive(true);
        }

    }

}

