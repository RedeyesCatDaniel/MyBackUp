using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LGUVirtualOffice.Framework;


namespace LGUVirtualOffice
{
	public abstract class BasePanel
	{
		public UIPanelTypeEnum PanelType { get; protected set; }
		public UIPanelConfig Config { get; protected set; }
		public string Path { get; protected set; }
		public GameObject PanelGo { get; private set; }

		public BasePanel()
		{
			PanelType = UIPanelTypeEnum.Default;
			Config = new UIPanelConfig(PanelType);
		}
		public string GetName()
		{
			if (Path == null) return "";
			return Path.Substring(Path.LastIndexOf('/') + 1);
		}
		public void OnCreat(GameObject panelGO)
		{
			PanelGo = panelGO;
		}
		/// <summary>
		/// 初始化，仅初次生成时作用
		/// </summary>
		public virtual void Initialize()
		{

		}

		/// <summary>
		/// 进场
		/// </summary>
		public virtual void ActivePanel()
		{
			PanelGo.SetActive(true);
		}

		/// <summary>
		/// 禁用
		/// </summary>
		public virtual void DisablePanel()
		{
			PanelGo.SetActive(false);
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public virtual void OnPause()
		{

		}

		/// <summary>
		/// 继续
		/// </summary>
		public virtual void OnResume()
		{

		}


	}
}
