using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice.Framework;
namespace LGUVirtualOffice
{
    public class MapMoveClicked : AbstractController
    {
        private MapMoveClickedView clickedView;
        // Start is called before the first frame update
        void Start()
        {
            clickedView = GetComponent<MapMoveClickedView>();
            this.SubscribeEvent<JoinPhotonRoomSuccessEvent>((e)=>{ LogUtil.LogInfo("Join"); });
            clickedView.rigth.onClick.AddListener(() => 
            {
                
               
                gameObject.SetActive(false);
                SetWorkeSpace();

            });//Save
            clickedView.left.onClick.AddListener(() => { //Close
                gameObject.SetActive(false);
            });
        }
        // Update is called once per frame
        public void SetWorkeSpace()
        {
            FavoriteWorkSpaceList.Instance.Updata(UserInfo.Instance);
            MapMove.Instance.gameObject.SetActive(false);
            //LogUtil.LogDebug("set as:" + teamModel.teamName);
            //this.GetService<INetworkSyncService>().JoinWorkSpace(teamModel.teamName);
        }
    }
}