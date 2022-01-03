using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice 
{
    public class UIPanelConfig
    {
        /// <summary>
        /// 是否可被穿透，可穿透UI不影响玩家操作
        /// </summary>
        public bool IsAbleToThrough;
        /// <summary>
        /// 是否为栈类型，当新生成一个栈类型UI时，其余栈类型UI会被暂停
        /// </summary>
        public bool IsStackType;
        /// <summary>
        /// 是否为静态，此UI是否始终出现
        /// </summary>
        public bool IsStatic;
        /// <summary>
        /// 是否唯一，唯一UI会避免出现多个同类型UI
        /// </summary>
        public bool IsSingleton;
        /// <summary>
        /// 是否频繁进出，非频繁进出UI会采用销毁形式
        /// </summary>
        public bool IsEnterFrequently;
        /// <summary>
        /// 目标画布名称，如填写请在对应Scene的UIController中注册此画布
        /// </summary>
        public string CanvasName;


        public UIPanelConfig(UIPanelTypeEnum panelType)
        { 
            IsAbleToThrough = IsStackType = IsStatic = IsSingleton =  false;
            IsEnterFrequently = true;
            CanvasName = "Default";
            switch (panelType)
			{
                case UIPanelTypeEnum.Default:
                    break;
                case UIPanelTypeEnum.Static:
                    IsSingleton = IsSingleton = IsEnterFrequently = true;
                    break;
                case UIPanelTypeEnum.PopUp:
                    IsStackType = true;
                    break;
                case UIPanelTypeEnum.World:
                    IsAbleToThrough = true;
                    break;
			}
		}
        public UIPanelConfig()
		{
            IsAbleToThrough = IsStackType = IsStatic = IsSingleton = false;
            IsEnterFrequently = true;
            CanvasName = "Default";
        }
    }
}


