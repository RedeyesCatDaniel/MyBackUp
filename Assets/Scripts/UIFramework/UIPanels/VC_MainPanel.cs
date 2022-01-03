using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
	public class VC_MainPanel : BasePanel
	{
		public VC_MainPanel() : base()
		{
			// 必要面板信息
			Path = "VideoChat/VC_MainPanel";
			PanelType = UIPanelTypeEnum.Static;

			// 自定义面板类型，如使用预设留空
			Config.IsStatic = true;

		}

		public override void Initialize()
		{
			base.Initialize();
		}
		public override void ActivePanel()
		{
			base.ActivePanel();
		}
		public override void DisablePanel()
		{
			base.DisablePanel();
		}
		public override void OnPause()
		{
			base.OnPause();
		}
		public override void OnResume()
		{
			base.OnResume();
		}
	}
}
	
