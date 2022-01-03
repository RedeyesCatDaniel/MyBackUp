using Amazon.Extensions.CognitoAuthentication;
using LGUVirtualOffice.Framework;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LGUVirtualOffice
{
    public class LoginService : AbstractService,ILoginService
    {
        private UserInfo _userInfo;
        private DateTime loginTime;
        private IQueueMessageService queueMessageService;
        private INetworkSyncService networkSyncService;
        MD5 md5;

        #region Public Methods Region

        protected override void OnInit()
        {
            md5 = new MD5CryptoServiceProvider();
            queueMessageService = this.GetService<IQueueMessageService>();
            networkSyncService = this.GetService<INetworkSyncService>();
            //single sign-on
            this.SubscribeEvent<UserLoginSuccessSyncEvent>((e) => {
                if (!e.UserId.Equals(_userInfo.UserId))
                {
                    return;
                }
                //only new Message will be processed
                if (loginTime.CompareTo(e.LoginTime) > 0)
                {
                    return;
                }
                if (e.TokenMD5.Equals(this.GenerateIdTokenMD5Str(AWSUtil.Instance.GetIdToken(e.UserId))))
                {
                    LogUtil.LogDebug("Same Token");
                    return;
                }
                LogUtil.LogDebug("have been kicked out!");
                this.TriggerEvent<AllopatricLoginEvent>();
            });
        }
        /// <summary>
        /// Login. 
        /// </summary>
        /// <param name="userInfo">only username and password needed </param>
        /// <returns>result of login operation,true means success</returns>
        public void Login(UserInfo userInfo)
        {
            string checkResult;
            if (!string.IsNullOrEmpty(checkResult = CheckLoginUserInfo(userInfo)))
            {
                LogUtil.LogInfo("Login Parameter InCorrect!Login User:" + userInfo.UserName);
                this.TriggerEvent<UserLoginFailedEvent>();
            }
            userInfo.UserId = AWSUtil.Instance.GenerateUserId(userInfo);
            //check if the user already logged in,if can achieve sessionId,user have logged in;
            if (!string.IsNullOrEmpty(AWSUtil.Instance.GetIdToken(userInfo.UserId)))
            {
                return;
            }
            if (string.IsNullOrEmpty(userInfo.Password))
            {
                userInfo.Password = AWSUtil.Instance.GeneratePassword(userInfo.UserName);
            }
            var authTask = AWSUtil.Instance.StartSRPAuthAsync(userInfo);
            var awaiter = authTask.GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                if (authTask.IsFaulted)
                {
                    LogUtil.LogError(authTask.Exception.GetBaseException().Message, authTask.Exception.GetBaseException());
                    this.TriggerEvent<UserLoginFailedEvent>();
                }
                else
                {
                    UpdateToken(authTask.Result);
                    var getUserResult = logDBManager.Instance.GetUserInfo(userInfo);
                    getUserResult.OnFailed((errorMessage) => {
                        this.TriggerEvent<UserLoginFailedEvent>();
                    });
                    getUserResult.OnSuccess((userInfo) =>
                    {
                        LogUtil.LogDebug("GetUserInfo success");
                        _userInfo = userInfo;
                        bool canLogin = true;
                        //used for single-point login implementation
                        /*loginTime = DateTime.Now;
                        string idTokenMD5 = GenerateIdTokenMD5Str(authTask.Result.AuthenticationResult.IdToken);
                        queueMessageService.PushEventMessage(new UserLoginSuccessSyncEvent() 
                        { 
                            UserId = userInfo.UserId, 
                            TokenMD5 = idTokenMD5, 
                            LoginTime=loginTime
                        }, 
                        () =>
                        {
                            canLogin = false;
                            Logout();
                            this.TriggerEvent<UserLoginFailedEvent>();
                        });*/
                        if (canLogin)
                        {
                            this.TriggerEvent<UserLoginSuccessEvent>();
                        }
                    });
                }
            });
        }
        public void Logout(string userId=null)
        {
            AWSUtil.Instance.ClearCachedUserInfo();
            _userInfo.ClearCache();
            networkSyncService.DisConnectFromPhontonServer();
        }
        /// <summary>
        /// Reset user's password,need to invoke this method before user first time Login. 
        /// </summary>
        /// <param name="userInfo">all of the attributes needed </param>
        /// <returns>result of reset password operation,true means success</returns>
        public void ResetPassword(UserInfo userInfo)
        {
            /*if (!CheckResetPasswordUserInfo(userInfo)) {
                LogUtil.LogInfo("ResetPassword Parameter InCorrect!Login User:" + userInfo.UserName);
                return false;
            }
            try
            {
                //check if the user already logged in.
                string sessionId = AWSUtil.Instance.GetUserSessionId(userInfo.UserId);
                if (sessionId == null) { 
                    Task<AuthFlowResponse> authTask = AWSUtil.Instance.StartSRPAuthAsync(userInfo);
                    sessionId = authTask.Result.SessionID;
                    AWSUtil.Instance.UpdateUserSessionId(sessionId);
                    if (!AWSUtil.Instance.NeedToChangePassword(authTask.Result)) {
                        LogUtil.LogInfo("User "+userInfo.UserName+" No Need to Reset Password,"+ authTask.Result.ChallengeName);
                        return false;
                    }
                }
                Task<AuthFlowResponse> resetPwdTask = AWSUtil.Instance.StartResetPasswordAsync(userInfo);
                AWSUtil.Instance.UpdateTokens(resetPwdTask.Result.AuthenticationResult.IdToken,
                        resetPwdTask.Result.AuthenticationResult.AccessToken,
                        resetPwdTask.Result.AuthenticationResult.RefreshToken,
                        resetPwdTask.Result.AuthenticationResult.ExpiresIn);
                return true;
            }
            catch (Exception e)
            {
                LogUtil.LogError(userInfo.UserName + " Reset Password Failed,",e);
                return false;
            }*/
        }
        #endregion

        #region Private Methods Region
        private string CheckLoginUserInfo(UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(userInfo.UserName))
            {
                return ReturnMessageConst.log_UserName_Incorrect;
            }
            if (string.IsNullOrEmpty(userInfo.TeamInfo.TeamCode))
            {
                return ReturnMessageConst.log_Team_Incorrect;
            }
            return null;
        }

        private bool CheckResetPasswordUserInfo(UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(CheckLoginUserInfo(userInfo)))
            {
                return false;
            }
            /*if (string.IsNullOrEmpty(userInfo.Gender))
            {
                return false;
            }*/
            /*if (string.IsNullOrEmpty(userInfo.Name)) {
                return false;
            }*/
            return true;
        }
        private void UpdateToken(AuthFlowResponse authResponse)
        {
            if (authResponse.AuthenticationResult != null)
            {
                //cache tokens if login process succeed
                AWSUtil.Instance.UpdateTokens(authResponse.AuthenticationResult.IdToken,
                    authResponse.AuthenticationResult.AccessToken,
                    authResponse.AuthenticationResult.RefreshToken,
                    authResponse.AuthenticationResult.ExpiresIn);
            }
        }
        public string GenerateIdTokenMD5Str(string idToken)
        {
            
            return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(idToken)));
        }
        #endregion
    }
}
