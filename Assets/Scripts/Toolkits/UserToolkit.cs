using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "Toolkit/UserToolkit")]
    public class UserToolkit:ScriptableObject
    {
        public static UserInfo GetDefaultInfo() {
            FillUserInfo();
            return UserInfo.Instance;
        }

        public static void DefaultLogin() {
            ServerReturnModel<UserInfo> rs = logLoginManager.Instance.Login(GetDefaultInfo());
            rs.OnSuccess((x)=> {
                Debug.Log("I am in");
            });
            rs.OnFailed((x) => {
                Debug.Log("I am not in");
            });
            Debug.Log("I started a request to login");
        }

        public static void DefaultLogin(System.Action onSucess, System.Action onFail)
        {
            ServerReturnModel<UserInfo> rs = logLoginManager.Instance.Login(GetDefaultInfo());
            rs.OnSuccess((x) => {
                onSucess?.Invoke();
                //Debug.Log("I am in");
            });
            rs.OnFailed((x) => {
                onFail?.Invoke();
                //Debug.Log("I am not in");
            });
            Debug.Log("I started a request to login");
        }


        public static void FillUserInfo() {
            UserInfo.Instance.UserName = "이정재";

            UserInfo.Instance.TeamInfo = new TeamModel()
            {
                TeamCode = "6LjfBRDXrRXUdN78EvpB3Q==",
                TeamName = "차세대기술발굴2팀",
            };
        }
    }
}