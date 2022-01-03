using LGUVirtualOffice.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
	public class VCController : AbstractController
	{
		IVCService VCService;
		IUIService UIService;
		public UnityEvent<int> playAnim;

		public GameObject VideoPanelPre;
		public Transform Canvas;
		GameObject VCPanel;


		private void Start()
		{
			VCService = this.GetService<IVCService>();
			UIService = this.GetService<IUIService>();
			VCService.InitailizeVCSystem();

			this.SubscribeEvent<PlayerCreatSuccessedEvent>((e) => CreatUI());
			this.SubscribeEvent<PlayAnimationEvent>((e) => PlayAnimation(e.animID));

			UIService.ActiveUIPanel(new ExamplePanel());
		}

		void CreatUI()
		{
			//UIService.ActiveUIPanel(new VC_MainPanel());
			//VCPanel = GameObject.Instantiate<GameObject>(VideoPanelPre, Canvas);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				this.GetService<IDeviceManager>().QuickSwitchCamDevice();
			}
		}

		public void PlayAnimation(int id)
		{
			id++;
			if (id > 1) return;
			playAnim.Invoke(id);
		}
	}
}
