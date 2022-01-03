using LGUVirtualOffice.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class My_GuestList : AbstractService
    {
        private INetworkSyncService photonService;
        protected override void OnInit()
        {
            photonService = this.GetService<INetworkSyncService>();
            this.SubscribeEvent<GetCurrentWorkSpaceGuestListSuccessEvent>(GetCurrentWorkSpaceGuestListSuccess);
            this.SubscribeEvent<WorkSpaceGuestListUpdateEvent>(WorkSpaceGuestListUpdate);
        }

        /// <summary>
        /// 更新guest列表
        /// </summary>
        public void GetList()
        {
            photonService.GetCurrentWorkSpaceGuestList();
            Debug.Log("更新Guest列表");
        }

        /// <summary>
        /// 获取新的Guest列表
        /// </summary>
        /// <param name="e"></param>
        private void GetCurrentWorkSpaceGuestListSuccess(GetCurrentWorkSpaceGuestListSuccessEvent e)
        {
            List<UserInfo> a = e.GuestList;
            MemberUserInfo.Instance.TeamGuestList = a;
            Debug.Log("AAA"+a.Count);

            if(InitUserInfo.Instance.obj_GuestListParent_PackUp.transform.childCount!=0)
            {
                InitUserInfo.Instance.Des(InitUserInfo.Instance.obj_GuestListParent_PackUp);// 清除Guest
            }


            // 加载新的Guest
            InitUserInfo.Instance.MemberListPrefab(a, InitUserInfo.Instance.obj_GuestListParent_PackUp,9, "Prefabs/UI/Member/Unfold_Member");
        }

        /// <summary>
        /// 监听guest加入当前的工作空间
        /// </summary>
        /// <param name="e"></param>
        private void WorkSpaceGuestListUpdate(WorkSpaceGuestListUpdateEvent e)
        {
            GetList();
        }

    }

}
