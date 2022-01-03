using System;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using System.Security.Cryptography;
using System.Text;

namespace LGUVirtualOffice
{
    [Obsolete("Deprecated,Please Use LoginService")]
    public class logLoginManager : Singleton<logLoginManager>
    {
        private logLoginManager() { }
        private UserInfo _userInfo;

        #region Public Methods Region
        /// <summary>
        /// Login. 
        /// </summary>
        /// <param name="userInfo">only username and password needed </param>
        /// <returns>result of login operation,true means success</returns>
        public ServerReturnModel<UserInfo> Login(UserInfo userInfo)
        {
            ServerReturnModel<UserInfo> result = new ServerReturnModel<UserInfo>();
            string checkResult;
            if (!string.IsNullOrEmpty(checkResult = CheckLoginUserInfo(userInfo)))
            {
                LogUtil.LogInfo("Login Parameter InCorrect!Login User:" + userInfo.UserName);
                result.TriggerOnFailed(checkResult);
                return result;
            }
            userInfo.UserId = AWSUtil.Instance.GenerateUserId(userInfo);
            //check if the user already logged in,if can achieve sessionId,user have logged in;
            if (!string.IsNullOrEmpty(AWSUtil.Instance.GetIdToken(userInfo.UserId)))
            {
                result.TriggerOnSuccess(_userInfo);
                return result;
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
                    result.TriggerOnFailed(ReturnMessageConst.log_User_Not_Exist);
                }
                else 
                {
                    UpdateToken(authTask.Result);
                    var getUserResult = logDBManager.Instance.GetUserInfo(userInfo);
                    getUserResult.OnFailed((errorMessage) => { result.TriggerOnFailed(errorMessage); });
                    getUserResult.OnSuccess((userInfo) =>
                    {
                        LogUtil.LogDebug("GetUserInfo success");
                        if (userInfo == null)
                        {
                            AWSUtil.Instance.ClearCachedUserInfo();
                            userInfo.ClearCache();
                            result.TriggerOnFailed(ReturnMessageConst.log_User_Not_Exist);
                        }
                        else
                        {
                            _userInfo = userInfo;
                            result.TriggerOnSuccess(userInfo);
                        }
                    });
                }
            });
            return result;
        }
        public void Logout(string userId)
        {
            if (AWSUtil.Instance.GetIdToken(userId) == null)
            {
                return;
            }
            AWSUtil.Instance.ClearCachedUserInfo();

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
        #endregion
    }
}
