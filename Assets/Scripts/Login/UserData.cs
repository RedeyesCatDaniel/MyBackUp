using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice;

public class UserData
{
    private static UserData instance;
    public static UserData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UserData();
            }
            return instance;
        }
    }

    private string userName;  // 用户名
    private string status;    // 状态
    private string signature; // 个性签名 


    public string UserName { get => userName; set => userName = value; }
    public string Status { get => status; set => status = value; }
    public string Signature { get => signature; set => signature = value; }
   

}
