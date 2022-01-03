using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 这个脚本好像没用，后期删除
    /// </summary>
    public class CompleteView_UserMemberTest:Singleton<CompleteView_UserMemberTest>
    {

        /// <summary>
        /// 生成面板
        /// </summary>
        /// <param name="obj">所依据位置的物体</param>
        /// <param name="controlPanel">要控制的面板</param>
        public void CreatePersonalInformation(GameObject obj,GameObject controlPanel)
        {
            float x = obj.GetComponent<RectTransform>().position.x;
            float y = obj.GetComponent<RectTransform>().position.y;
            if (controlPanel.activeSelf == false)
                controlPanel.SetActive(true);
            // 设置加载的坐标
            controlPanel.GetComponent<RectTransform>().localPosition = new Vector3(PanelPosition.Instance.ControllerCompleteView_UserMemberPersonalInformationPosition(x, y)[0],
                                                                                   PanelPosition.Instance.ControllerPersonalInformationPosition(x, y)[1], 0);
           
        }

    }
}

