using LGUVirtualOffice;
using LGUVirtualOffice.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPosition : AbstractService
{
    private INetworkSyncService photonService;
    protected override void OnInit()
    {
        photonService = this.GetService<INetworkSyncService>();
    }

    public void Jump_Position(string str_WorkSpacenNowIn)
    {
        photonService.JoinWorkSpace(str_WorkSpacenNowIn);
        Debug.Log("跳转空间到："+ str_WorkSpacenNowIn);
    }
}
