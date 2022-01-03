using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    interface IPosition
    {
        float ControlPosition(GameObject obj, float f); // 根据对应面板和偏移量返回坐标值
        float[] OffsetPosition(GameObject obj1, GameObject obj2); // 两个物体之间的偏移量
    }
}

