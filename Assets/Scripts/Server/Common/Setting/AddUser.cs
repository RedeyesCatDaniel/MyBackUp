using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace LGUVirtualOffice {
    [CreateAssetMenu(menuName = "ScriptableObjects/AddUser", fileName = "AddUser", order = 3)]
    public class AddUser : ScriptableObject
    {
        public string UserName = "";
        public string TeamName = "";
        public string Password = "12345678";

        [ContextMenu("AddNewUser")]
        public void AddNewUser()
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Password = Password;
            userInfo.UserName = UserName;
            userInfo.UserId = GenerateUserId();
            userInfo.TeamInfo = new TeamModel();
            userInfo.TeamInfo.TeamCode = GenerateTeamCode();
            userInfo.TeamInfo.TeamName = TeamName;
            var authTask = AWSUtil.Instance.StartSRPAuthAsync(userInfo);
            var awaiter = authTask.GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                if (authTask.IsFaulted) 
                {
                    Debug.Log("authTask Faulted!"+authTask.Exception.GetBaseException().Message);
                }
                Debug.Log("authTask Completed!");
                if ("NEW_PASSWORD_REQUIRED".Equals(awaiter.GetResult().ChallengeName))
                {
                    Debug.Log("NEW_PASSWORD_REQUIRED!");
                    userInfo.Password = GeneratePassword(UserName);
                    var resetPwdTask = AWSUtil.Instance.StartResetPasswordAsync(userInfo, awaiter.GetResult().SessionID);
                    resetPwdTask.GetAwaiter().OnCompleted(() =>
                    {
                        if (resetPwdTask.IsFaulted)
                        {
                            Debug.Log("resetPwdTask Faulted"+ resetPwdTask.Exception.GetBaseException().Message);
                        }
                        else 
                        {
                            Debug.Log("resetPwdTask Completed");
                            var authResponse = resetPwdTask.Result;
                            AWSUtil.Instance.UpdateTokens(authResponse.AuthenticationResult.IdToken,
                    authResponse.AuthenticationResult.AccessToken,
                    authResponse.AuthenticationResult.RefreshToken,
                    authResponse.AuthenticationResult.ExpiresIn);
                            AddUserToDB(userInfo);
                        }
                    });
                }
            });
            Debug.Log("111");
        }
        private void AddUserToDB(UserInfo user)
        {
            Debug.Log("AddUserToDB");
            DynamoDBUpdateModel condition = new DynamoDBUpdateModel()
            {
                TableName = "LGU_User_Member_Info",
                PartitionKey = new DynamoDBKeyModel() { Name = "TeamCode", Value = user.TeamInfo.TeamCode },
                SortKey = new DynamoDBKeyModel() { Name = "UserId", Value = user.UserId },
                items = GetUserInfo()
            };
            var result = DynamoDBUtil.Instance.AddItem(condition);
            result.OnFailed(() => { Debug.Log("add failed"); });
            result.OnCompleted((result) =>
            {
                Debug.Log("add success:");
            });
        }
        private Dictionary<string, object> GetUserInfo()
        {
            Dictionary<string, object> member = new Dictionary<string, object>();
            member.Add("TeamName", TeamName);
            member.Add("UserName", UserName);

            member.Add("Gender", 0);
            member.Add("AvatarColorData", "");
            member.Add("AvatarData", "");
            member.Add("AvatarPhoto", null);
            member.Add("UserStatus", 0);
            member.Add("WorkSpacenNowIn", "");
            member.Add("AreaStaying", "");
            member.Add("FavoriteWorkSpaceList", "null");
            member.Add("Signature", "");
            member.Add("VocationStartTime", "");
            member.Add("VocationEndTime", "");
            return member;
        }
        [ContextMenu("GenerateUserId")]
        public string GenerateUserId()
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            string s = Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(UserName + GenerateTeamCode() + "LGUVirtualOffice_USERID")));
            Debug.Log(s);
            return s;
        }
        private string GeneratePassword(string userName)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            string s = Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(userName + "LGUVirtualOffice_PASSWORD")));
            return s;
        }
        private string GenerateTeamCode()
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            string s = Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(TeamName)));
            return s;
        }
    }
}