using agora_gaming_rtc;
using LGUVirtualOffice.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace LGUVirtualOffice
{
	public class UIService : AbstractService, IUIService
	{
		static string UIPrefabsPath = "Prefabs/UIPanels/";

		List<UIType> AllPanels;                             // 所有已注册的面板

		List<Transform> CanvasTrans;						// 画布列表


		Stack<BasePanel> StackPanels;			// 栈式UI面板
		List<BasePanel> StaticPanels;           // 静态UI面板
		List<BasePanel> SingletonPanels;		// 唯一性UI面板
		List<BasePanel> DefaultPanels;			// 普通UI面板

		protected override void OnInit()
		{
			CanvasTrans = new List<Transform>();
			StackPanels = new Stack<BasePanel>();
			StaticPanels = new List<BasePanel>();
			SingletonPanels = new List<BasePanel>();
			DefaultPanels = new List<BasePanel>();

		}

		#region Public Functions
		public void SetCanvasList(List<Transform> CanvasTrans)
		{
			this.CanvasTrans = CanvasTrans;
		}




		

		/// <summary>
		/// 激活一个面板，如果未生成则生成并激活
		/// </summary>
		/// <param name="BasePanel"></param>
		public void ActiveUIPanel(BasePanel basePanel)
		{
			GameObject panelGO = GetPanelGO(basePanel);
			if(panelGO == null) return;
			basePanel.ActivePanel();

		}
		public void DisableUIPanel(BasePanel basePanel)
		{

		}
		public void DestoryUIPanel(BasePanel basePanel)
		{

		}

		#endregion
		#region Private Functions
		Transform GetCanvasTrans()
		{
			foreach(Transform canv in CanvasTrans)
			{
				if (canv.gameObject.name == "DefaultCanvas")
				{
					return canv;
				}
			}
			return null;
		}
		GameObject GetPanelGO(BasePanel basePanel)
		{
			// 已生成,返回目标物体
			if (basePanel.PanelGo != null)
				return basePanel.PanelGo;
			// 试图从唯一面板中寻找物体
			else if (basePanel.Config.IsSingleton)
			{
				foreach (BasePanel panel in SingletonPanels)
				{
					if (panel.GetName() == basePanel.GetName())
						return panel.PanelGo;
				}
			}

			// 未生成，试图生成并返回
			// 检测目标画布
			Transform canvas = GetCanvasTrans();
			if (canvas == null)
			{
				Debug.LogError("不存在目标画布 => " + "<画布名称>" + "\nUI面板为 => " + basePanel.GetName());
				return null;
			}
			// 试图生成
			string FullPath = UIPrefabsPath + basePanel.Path;
			GameObject panelGO = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(FullPath), canvas);
			basePanel.OnCreat(panelGO);
			basePanel.Initialize();
			panelGO.name = basePanel.GetName();
			// 加入字典
			return panelGO;
		}
		#endregion

	}
}

