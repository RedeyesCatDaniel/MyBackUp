using LGUVirtualOffice.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LGUVirtualOffice
{
	public class VC_UI_Controller : AbstractController
	{
		IVCService VCService;
		IVCUserService VCUserService;


		// 组件
		public VC_UI_WindowCtr localWindow;
		public Transform workTrans;
		public Transform tempTrans;
		public GameObject scrollTool;
		public TextMeshProUGUI workingMembersTxt;

		// 属性
		public List<VC_UI_WindowCtr> remoteWindows { get; private set; }
		Dictionary<VC_UserInfo, VC_UI_WindowCtr> Dic_InfoToWindow;




		private void Start()
		{
			VCService = this.GetService<IVCService>();
			VCUserService = this.GetService<IVCUserService>();
			// 数值初始化
			Dic_InfoToWindow = new Dictionary<VC_UserInfo, VC_UI_WindowCtr>();
			remoteWindows = new List<VC_UI_WindowCtr>();

			// 获取滚动窗口，进行初始化
			remoteWindows.AddRange(workTrans.GetComponentsInChildren<VC_UI_WindowCtr>());

			// 对本地窗口进行播放初始化
			localWindow.enabled = true;
			localWindow.RegisterUser(VCUserService.GetLocalInfo());

			VCUserService.SetUIController(this);

			// 刷新面板
			ResetPanel();
		}


		#region Public Function
		/// <summary>
		/// 重置面板
		/// </summary>
		public void ResetPanel()
		{
			// 将所有面板关闭，放入暂存区
			foreach (VC_UI_WindowCtr window in remoteWindows)
			{
				SetWindowFree(window);
			}
			// 重置数据
			Dic_InfoToWindow = new Dictionary<VC_UserInfo, VC_UI_WindowCtr>();
			// 关闭UI工具
			CheckScrollTool();
		}
		/// <summary>
		/// 用户退出附近
		/// </summary>
		public void UserLeaveAround(VC_UserInfo info)
		{
			// 将对应面板重置，放入暂存区
			SetWindowFree(info);
		}
		/// <summary>
		/// 调整排队队列
		/// </summary>
		public void CheckQueue()
		{
			List<VC_UserInfo> workUser = VCUserService.GetWorkUsers();
			List<VC_UserInfo> tempUser = VCUserService.GetTempUsers();
			// 工作用户数少于等于窗口数量
			if (workUser.Count <= remoteWindows.Count)
			{
				// 处理工作面板
				foreach (VC_UserInfo info in workUser)
				{
					VC_UI_WindowCtr window;
					if (!Dic_InfoToWindow.TryGetValue(info, out window))
					{
						if (!TryGetAFreeWindow(out window))
						{
							if (!TryGetATempWindow(out window))
								TryCaptureAWorkWindow(workUser, out window);
						}
					}
					if (window.WorkingState != 2)
					{
						SetWindowWork(info, window);
					}
					else
					{
						if (window.UserInfo != info)
							SetWindowWork(info, window);
					}
				}
				// 处理暂存面板
				for (int i = 0; i < tempUser.Count; i++)
				{
					VC_UserInfo info = tempUser[i];
					VC_UI_WindowCtr window = null;
					if (!Dic_InfoToWindow.TryGetValue(info, out window))
						TryGetAFreeWindow(out window);
					if (window != null)
					{
						if (window.WorkingState != 1)
							SetWindowTemp(info, window);
						else
						{
							if (window.UserInfo != info)
								SetWindowTemp(info, window);
						}
					}
					else
						break;
				}
			}
			// 工作用户数大于窗口数量
			else
			{
				// 处理工作面板
				for (int i = 0; i < remoteWindows.Count; i++)
				{
					VC_UserInfo info = workUser[i];
					VC_UI_WindowCtr window;
					if (!Dic_InfoToWindow.TryGetValue(info, out window))
					{
						if (!TryGetAFreeWindow(out window))
						{
							if (!TryGetATempWindow(out window))
								TryCaptureAWorkWindow(workUser, out window);
						}
					}
					if (window.WorkingState != 2)
					{
						SetWindowWork(info, window);
					}
					else
					{
						if (window.UserInfo != info)
							SetWindowWork(info, window);
					}
				}
			}

			// 检查更新其它元件
			CheckScrollTool();
			CheckWindowShow();
		}
		/// <summary>
		/// 调整视窗的显示
		/// </summary>
		public void CheckWindowShow()
		{
			this.SendCommand<RefreshViewCommand>(new RefreshViewCommand());
		}

		/// <summary>
		/// 彻底释放一个窗口，释放后进行调整队列
		/// </summary>
		/// <param name="uid"></param>
		public void SetWindowFree(VC_UserInfo info)
		{
			// 如果已经分配面板，面板归零放回暂存
			if (Dic_InfoToWindow.ContainsKey(info))
			{
				VC_UI_WindowCtr window = Dic_InfoToWindow[info];
				window.ResetWindow();

				window.gameObject.SetActive(false);
				window.transform.SetParent(tempTrans);
				Dic_InfoToWindow.Remove(info);
				window.ResetWindow();
			}
		}
		/// <summary>
		/// 彻底释放一个窗口，释放后进行调整队列
		/// </summary>
		/// <param name="uid"></param>
		public void SetWindowFree(VC_UI_WindowCtr window)
		{
			window.ResetWindow();
			window.gameObject.SetActive(false);
			window.transform.SetParent(tempTrans);
			window.ResetWindow();
		}
		/// <summary>
		/// 滚动视频面板
		/// </summary>
		/// <param name="isRight"></param>
		public void ScrollPanel(bool isRight)
		{
			// 对此面板进行数据调整
			VCUserService.ScrollQueue(isRight);
			CheckQueue();
		}

		#endregion
		#region Private Function
		/// <summary>
		/// 激活并展示一个视频窗口
		/// </summary>
		/// <param name="uid"></param>
		void SetWindowWork(VC_UserInfo info, VC_UI_WindowCtr window)
		{
			SetInfoToWindow(window, info);

			// 彻底激活一个窗口
			window.transform.SetParent(workTrans);
			window.gameObject.SetActive(true);
			window.SetWorkState(2);
		}
		/// <summary>
		/// 将一个视频窗口进行后台缓冲
		/// </summary>
		/// <param name="uid"></param>
		void SetWindowTemp(VC_UserInfo info, VC_UI_WindowCtr window)
		{
			SetInfoToWindow(window, info);

			// 彻底激活一个窗口
			window.transform.SetParent(tempTrans);
			window.gameObject.SetActive(true);

			window.SetWorkState(1);
		}
		/// <summary>
		/// 检查滚动控制器状态设置
		/// </summary>
		void CheckScrollTool()
		{
			if (VCUserService.GetWorkUsers().Count > 4)
			{
				scrollTool.SetActive(true);
				workingMembersTxt.text = VCUserService.GetWorkUsers().Count.ToString();
			}
			else
			{
				scrollTool.SetActive(false);
			}
		}
		/// <summary>
		/// 试图获取一个空闲的远端窗口，没有时返回false;
		/// </summary>
		/// <returns></returns>
		bool TryGetAFreeWindow(out VC_UI_WindowCtr freeWindow)
		{
			freeWindow = null;
			foreach (VC_UI_WindowCtr window in remoteWindows)
			{
				if (window.WorkingState == 0)
				{
					freeWindow = window;
					break;
				}
			}
			if (freeWindow == null)
				return false;
			else
				return true;
		}
		/// <summary>
		/// 试图获取一个空闲的远端窗口，没有时返回false;
		/// </summary>
		/// <returns></returns>
		bool TryGetATempWindow(out VC_UI_WindowCtr tempWindow)
		{
			tempWindow = null;
			foreach (VC_UI_WindowCtr window in remoteWindows)
			{
				if (window.WorkingState == 1)
				{
					tempWindow = window;
					break;
				}
			}
			if (tempWindow == null)
				return false;
			else
				return true;
		}
		/// <summary>
		/// 试图夺取一个工作的远端窗口，没有时返回false;
		/// </summary>
		bool TryCaptureAWorkWindow(List<VC_UserInfo> workUser, out VC_UI_WindowCtr workWindow)
		{
			workWindow = null;
			for (int i = remoteWindows.Count; i < workUser.Count; i++)
			{
				VC_UserInfo info = workUser[i];
				if (Dic_InfoToWindow.TryGetValue(info, out workWindow))
					return true;
			}
			return false;
		}

		/// <summary>
		/// 将一个窗口与UID绑定，并加入词典
		/// </summary>
		/// <param name="window"></param>
		/// <param name="uid"></param>
		void SetInfoToWindow(VC_UI_WindowCtr window, VC_UserInfo info)
		{
			window.RegisterUser(info);
			if (Dic_InfoToWindow.ContainsKey(info))
				Dic_InfoToWindow[info] = window;
			else
				Dic_InfoToWindow.Add(info, window);
		}

		#endregion

	}
}

