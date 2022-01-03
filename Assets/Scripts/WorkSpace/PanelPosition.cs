using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 控制各个 panel面板 的生成位置
    /// </summary>
    public class PanelPosition : Singleton<PanelPosition>, IPosition
    {
        private PanelPosition() { }

        /// <summary>
        /// 所对应面板的位置+面板的宽度
        /// </summary>
        /// <param name="obj">对应面板（要依靠的面板，非显示面板）</param>
        /// <returns></returns>
        public float ControlPosition(GameObject obj, float f)
        {
            float x = obj.GetComponent<RectTransform>().localPosition.x;
            float w = obj.GetComponent<RectTransform>().rect.width;

            return (x + w) + f;
        }

        public float[] OffsetPosition(GameObject obj1, GameObject obj2)
        {
            float x = obj1.transform.position.x - obj2.transform.position.x;
            float y = obj1.transform.position.y - obj2.transform.position.y;
            float[] xy = { x , y };
           
            return xy;
        }

        public float[] ControllerPersonalInformationPosition(float x, float y)
        {
            float[] xy = new float[2];
            x += 210;
            y -= 910;
            xy[0] = x;
            xy[1] = y;
            return xy;
        }

        public float[] ControllerCompleteView_UserMemberPersonalInformationPosition(float x, float y)
        {
            float[] xy = new float[2];
            x += 170;
            y -= 910;
            xy[0] = x;
            xy[1] = y;
            return xy;
        }

    }

}
