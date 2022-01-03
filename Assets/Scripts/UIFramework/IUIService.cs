using agora_gaming_rtc;
using LGUVirtualOffice.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
	public interface IUIService : IService
	{

		public void SetCanvasList(List<Transform> CanvasTrans);

		/// <summary>
		/// 激活一个UI面板
		/// </summary>
		/// <param name="basePanel"></param>
		public void ActiveUIPanel(BasePanel BasePanel);
		/// <summary>
		/// 关闭一个UI面板
		/// </summary>
		/// <param name="basePanel"></param>
		public void DisableUIPanel(BasePanel BasePanel);
		/// <summary>
		/// 强制销毁一个UI面板
		/// </summary>
		/// <param name="basePanel"></param>
		public void DestoryUIPanel(BasePanel BasePanel);

	}
}
