using agora_gaming_rtc;
using LGUVirtualOffice.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 视频窗口的控制
/// 每次更新需进行初始化，启用时默认状态为接受视频推流，显示关闭，声音关闭,完全隐藏
/// </summary>

namespace LGUVirtualOffice
{
	public class VC_UI_WindowCtr : AbstractController, IPointerClickHandler
	{
		IVCService VCService;
		IVCUserService VCUserService;
		IDeviceManager DeviceManager;
		// 组件绑定
		public GameObject windowTools;
		public VideoSurface videoSurface;
		public RawImage videoRaw;
		public TextMeshProUGUI NameTxt;
		public bool isLocalWindow;
		public Image avatarImg;

		public Button camBtn;
		public Button micBtn;
		public Button emjBtn;
		public GameObject CamMuteImg;
		public GameObject MicMuteImg;
		public GameObject EmjOnImg;
		public GameObject EmojiList;

		// 属性
		public VC_UserInfo UserInfo { get; private set; }
		/// <summary>
		/// 0：空闲，1：暂存，2：工作
		/// </summary>
		public int WorkingState { get; private set; }

		private void Awake()
		{
			VCUserService = this.GetService<IVCUserService>();
			VCService = this.GetService<IVCService>();
			DeviceManager = this.GetService<IDeviceManager>();
		}

		private void Start()
		{
			this.SubscribeEvent<RefreshViewEvent>((e) => CheckVideoShow(false));
		}

		// 每次启用状态刷新
		void OnEnable()
		{
			// 元件初始化
			// 关闭工具栏
			windowTools.SetActive(false);
		}

		/// <summary>
		/// 重置视频播放窗口状态
		/// </summary>
		public void ResetWindow()
		{
			StopAllCoroutines();
			this.UserInfo = null;
			WorkingState = 0;
		}


		#region 工具栏相关
		/// <summary>
		/// 点击视频窗口时，切换工具栏的状态
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerClick(PointerEventData eventData)
		{
			if (isLocalWindow)
			{
				windowTools.SetActive(!windowTools.activeSelf);
				if (windowTools.activeSelf)
					ResetToolState();
			}
		}

		// 进行初始化
		public void ResetToolState()
		{
			EmjOnImg.SetActive(false);
			EmojiList.SetActive(false);
			// 调整各UI元件
			RefreshBtnImg();
		}
		public void ClickCamBtn()
		{
			List<string> CameraList = DeviceManager.GetCameraDeviceList();
			if (CameraList == null) VCService.MuteLocalVideoStream(true);
			bool isCamOn = VCUserService.GetLocalPushState(true);
			VCService.MuteLocalVideoStream(isCamOn);
			CheckVideoShow(true);
			RefreshBtnImg();
		}

		public void ClickMicBtn()
		{
			bool isMicOn = VCUserService.GetLocalPushState(false);
			isMicOn = VCUserService.GetLocalPushState(false);

			// 本地
			VCService.MuteLocalAudioStream(!isMicOn);

			RefreshBtnImg();
		}
		public void ClickEmjBtn()
		{
			EmjOnImg.SetActive(!EmjOnImg.activeSelf);
			EmojiList.SetActive(EmjOnImg.activeSelf);

		}
		/// <summary>
		/// 根据视频控制状态，更新按钮视图
		/// </summary>
		void RefreshBtnImg()
		{
			CamMuteImg.SetActive(!VCUserService.GetLocalPushState(true));
			MicMuteImg.SetActive(!VCUserService.GetLocalPushState(false));
		}
		#endregion


		/// <summary>
		/// 为窗口绑定注册一个VCUserInfo
		/// </summary>
		/// <param name="UID"></param>
		public void RegisterUser(VC_UserInfo info)
		{
			if (info == null)
			{
				Debug.LogError("当前窗口注册的信息为空！");
				return;
			}
			if (info.isLocalPlayer)
			{
				NameTxt.transform.parent.gameObject.SetActive(false);
				UserInfo = VCUserService.GetLocalInfo();
			}
			else
			{
				this.UserInfo = info;
				NameTxt.text = info.GetName();
				//获取并修改avatarImg

			}

			videoSurface.SetForUser(info.GetUID());
			videoSurface.enabled = true;
			videoSurface.SetEnable(true);
			if (!isLocalWindow)
				VCService.SetUserVolume(info.GetUID(), 100);
			SetWorkState(0);
			videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
			CheckVideoShow(true);
		}



		/// <summary>
		/// 设置视频显示
		/// </summary>
		public void SetVideoShow(float alphaRate)
		{
			VC_Tool.SetAlpha(videoRaw, alphaRate);
		}

		/// <summary>
		/// 判断视频显示状态，在对方推流且本地接收的情况下才显示
		/// </summary>
		public void CheckVideoShow(bool isIncludeLocal)
		{
			VCUserService = this.GetService<IVCUserService>();
			// 判断是否为本地
			if (isLocalWindow)
			{
				if (!isIncludeLocal)
					return;
				List<string> CameraList = DeviceManager.GetCameraDeviceList();
				if (CameraList == null) VCService.MuteLocalVideoStream(true);
				VCService.MuteLocalVideoStream(!VCUserService.GetLocalPushState(true));
				if (VCUserService.GetLocalPushState(true))
					SetVideoShow(1);
				else
					SetVideoShow(0);
			}
			else
			{
				bool isOn = !UserInfo.isVideoMuted;
				if (!isOn)
					SetVideoShow(0);
				else
				{
					//根据规则判断
					float alpha = 1;
					if (AreaManager.CurRule == VCRuleEnum.Fade)
						alpha = VCUserService.GetUserAlpha(UserInfo);
					SetVideoShow(alpha);
				}
			}
		}

		/// <summary>
		/// 更改工作状态标识
		/// 0,空闲 1,暂存中 2,工作中
		/// </summary>
		/// <param name="state">0,空闲 1,暂存中 2,工作中</param>
		public void SetWorkState(int state)
		{
			WorkingState = state;
		}

		public void PlayGesture(int id)
		{
			PlayAnimationCMD cmd = new PlayAnimationCMD
			{
				animID = id
			};
			this.SendCommand<PlayAnimationCMD>(cmd);

		}

	}
}
