using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
	public class ExamplePanel : BasePanel
	{
		public ExamplePanel() : base()
		{
			// 必要面板信息
			Path = "UIFramework/ExamplePanel";
			PanelType = UIPanelTypeEnum.Default;

			// 需保留语句
			Config = new UIPanelConfig(PanelType);

			// 自定义面板类型，如使用预设留空
			Config.IsSingleton = true;
			Config.IsEnterFrequently = false;

		}

		public override void Initialize()
		{
			// 不必须，可在自己的面板逻辑中处理，仅用作示例
			Button ConfirmBtn = PanelGo.transform.Find("ConfirmBtn").GetComponent<Button>();
			Button CancelBtn = PanelGo.transform.Find("CancelBtn").GetComponent<Button>();

			ConfirmBtn.onClick.AddListener(() => ClosePanel());
			CancelBtn.onClick.AddListener(() => ClosePanel());

		}

		// 无需定制修改的内容可直接删除，无需覆写
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

		// 自己需要的函数
		void ClosePanel()
		{
			DisablePanel();
		}
	}
}
