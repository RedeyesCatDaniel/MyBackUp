using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 退出界面
    /// </summary>
    public class ExitPanel : AbstractController
    {
        public Button btn_No;  // 取消按钮
        public Button btn_Yes;  // 取消按钮
                                // Start is called before the first frame update
        void Start()
        {
            btn_No.onClick.AddListener(() => {
                this.gameObject.SetActive(false);
            });
            btn_Yes.onClick.AddListener(() => {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
            });

        }


    }

}
