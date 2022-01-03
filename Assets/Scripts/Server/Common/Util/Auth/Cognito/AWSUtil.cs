using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Amazon;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace LGUVirtualOffice
{
    public class AWSUtil : Singleton<AWSUtil>
    {
        private AWSUtil()
        {
            InitiateCognitoUserPool();
        }
        private AWSSetting _settings;
        private CognitoRegion _regionInfo;
        public CognitoUser _user { get; private set; }
        private CognitoUserPool _userPool;
        private CognitoAWSCredentials _credentials;
        private AmazonCognitoIdentityProviderClient _provider;

        #region Public Methods Region
        public CognitoAWSCredentials GetCognitoAWSCredentials(string userId, bool unAuthSupport = false)
        {
            //check if user logged in
            if (!unAuthSupport && GetIdToken(userId) == null)
            {
                _credentials = null;
                return null;
            }
            if (_credentials == null)
            {
                _credentials = new CognitoAWSCredentials(_settings.identityPoolId, AWSRegionMapper.regionMapper[_settings.region]);
            }
            if (!unAuthSupport)
            {
                _credentials.RemoveLogin(_settings.providerName);
                _credentials.AddLogin(_settings.providerName, _settings.idToken);
            }
            return _credentials;
        }

        public string GetUserSessionId(string userId)
        {
            //every time after user authenticated, CognitoUserSession will be refreshed;
            CheckAWSSettings();
            if (_settings.userSessionId == null)
            {
                return null;
            }
            if (CheckUserChange(userId))
            {
                return null;
            }
            if (_settings.sessionExpirationTime.CompareTo(DateTime.Now) < 0)
            {
                return null;
            }
            return _settings.userSessionId;
        }

        public string GetIdToken(string userId)
        {
            CheckAWSSettings();
            if (_settings.idToken == null)
            {
                return null;
            }
            if (CheckUserChange(userId))
            {
                return null;
            }
            if (TokenExpired())
            {
                return null;
            }
            return _settings.idToken;
        }

        public string GetAccessToken(string userId)
        {
            CheckAWSSettings();
            if (_settings.accessToken == null)
            {
                return null;
            }
            if (CheckUserChange(userId))
            {
                return null;
            }
            if (TokenExpired())
            {
                return null;
            }
            return _settings.accessToken;
        }

        public void UpdateUserSessionId(string sessionId)
        {
            _settings.userSessionId = sessionId;
            _settings.sessionExpirationTime = DateTime.UtcNow.AddSeconds(_settings.sessionExpiresIn);
        }

        public void UpdateTokens(string idToken, string accessToken, string refreshToken, double expiredIn)
        {
            _settings.idToken = idToken;
            _settings.accessToken = accessToken;
            _settings.refreshToken = refreshToken;
            _settings.tokenExpirationTime = DateTime.UtcNow.AddSeconds(expiredIn);
        }

        public AWSSetting GetAWSSetting()
        {
            CheckAWSSettings();
            return _settings;
        }
        //Authenticate user with AWS Cognito SRP Auth flow
        public Task<AuthFlowResponse> StartSRPAuthAsync(UserInfo userInfo)
        {
            InitiateCognitoUserPool();
            InitiateSrpAuthRequest req = new InitiateSrpAuthRequest()
            {
                Password = userInfo.Password
            };
            var task = GetCognitoUser(userInfo.UserId).StartWithSrpAuthAsync(req);
            return task;
        }

        /*public async Task<AuthFlowResponse> StartAdminAuthAsync()
        {
            CheckAWSSettings();
            AmazonCognitoIdentityProviderClient provider =
                new AmazonCognitoIdentityProviderClient(new BasicAWSCredentials(_settings.accessKey, _settings.secretKey),
                AWSRegionMapper.regionMapper[_settings.region]);
            CognitoUserPool userpoop = new CognitoUserPool(_settings.userPoolId, _settings.clientId, provider);
            CognitoUser cognitoUser = new CognitoUser(_settings.adminUserName, _settings.clientId, userpoop, provider);
            InitiateAdminNoSrpAuthRequest req = new InitiateAdminNoSrpAuthRequest() { Password = _settings.adminPassword};
            try
            {
                AuthFlowResponse response = await cognitoUser.StartWithAdminNoSrpAuthAsync(req).ConfigureAwait(false);
                return response;
            }
            catch (Exception e)
            {
                LogUtil.LogDebug(e.Message);
                return null;
            }
        }*/

        public bool NeedToChangePassword(AuthFlowResponse res)
        {
            if (res.AuthenticationResult != null)
            {
                return false;
            }
            if (!res.ChallengeName.Value.Equals(_settings.reset_password_challenge))
            {
                return false;
            }
            return true;
        }

        //Respond To New_Password_Required challenge
        public async Task<AuthFlowResponse> StartResetPasswordAsync(UserInfo userInfo,string sessionId=null)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = _settings.userSessionId;
            }
            RespondToNewPasswordRequiredRequest req = new RespondToNewPasswordRequiredRequest()
            {
                SessionID = sessionId,
                NewPassword = userInfo.Password
            };
            //Need all attributes have selected in the AWS Cognito user pool ,except the attribute which used as username
            Dictionary<string, string> userAttributes = new Dictionary<string, string>() {
                /*{"gender",userInfo.Gender},*/
                //{ "name",userInfo.Name}
            };
            return await GetCognitoUser(userInfo.UserId).RespondToNewPasswordRequiredAsync(req, userAttributes);
        }
        public void ClearCachedUserInfo()
        {
            CheckAWSSettings();
            _user = null;
            _credentials = null;
            _settings.userSessionId = null;
            _settings.sessionExpirationTime = DateTime.UtcNow;
            _settings.idToken = null;
            _settings.accessToken = null;
            _settings.refreshToken = null;
            _settings.tokenExpirationTime = DateTime.UtcNow;
        }

        #endregion

        #region Private Methods Region
        //check if the _settings refrence exists.
        private void CheckAWSSettings()
        {
            if (_settings)
            {
                return;
            }
            _regionInfo = ScriptableObject.CreateInstance<CognitoRegion>();
            _settings= ScriptableObject.CreateInstance<AWSSetting>();
            _settings.hideFlags = HideFlags.HideAndDontSave;
            _regionInfo.hideFlags = HideFlags.HideAndDontSave;
            _settings.userPoolId = _regionInfo.regionToUserPoolIdMapper[_settings.region];
            _settings.identityPoolId = _regionInfo.regionToIdentityPoolIdMapper[_settings.region];
            _settings.clientId = _regionInfo.userPoolIdToClientIdMapper[_settings.userPoolId];
            _settings.poolRegion = _settings.userPoolId.Substring(0, _settings.userPoolId.IndexOf("_"));
            _settings.providerName = "cognito-idp." + _settings.poolRegion + ".amazonaws.com/" + _settings.userPoolId;
        }

        private bool CheckUserChange(string userId)
        {
            if (_user == null || !_user.UserID.Equals(userId))
            {
                return true;
            }
            return false;
        }

        public string GeneratePassword(string userName)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(userName + _settings.password_generate_salt)));
        }

        public string GenerateUserId(UserInfo userInfo)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            string originalString = userInfo.UserName + userInfo.TeamInfo.TeamCode + _settings.userid_generate_salt;
            return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(originalString)));
        }
        private bool TokenExpired()
        {
            if (_settings.tokenExpirationTime.CompareTo(DateTime.UtcNow) < 0)
            {
                ClearCachedUserInfo();
                return true;
            }
            try
            {
                //if tokens will expire in five minutes,refresh tokens
                if (DateTime.UtcNow.AddMinutes(5).CompareTo(_settings.tokenExpirationTime) > 0)
                {
                    StartRefreshTokenAsync();
                }
            }
            catch (Exception e)
            {
                LogUtil.LogInfo("RefreshTokenAsync Failed," + e.Message);
            }
            return false;
        }

        private void InitiateCognitoUserPool()
        {
            if (_userPool == null)
            {
                CheckAWSSettings();
                AnonymousAWSCredentials credentials = new AnonymousAWSCredentials();
                _provider = new AmazonCognitoIdentityProviderClient(credentials, AWSRegionMapper.regionMapper[_settings.region]);
                _userPool = new CognitoUserPool(_settings.userPoolId, _settings.clientId, _provider);
            }
        }
        private CognitoUser GetCognitoUser(string userId)
        {
            if (_user == null)
            {
                _user = new CognitoUser(userId, _settings.clientId, _userPool, _provider);
                return _user;
            }
            if (!_user.UserID.Equals(userId))
            {
                _user = new CognitoUser(userId, _settings.clientId, _userPool, _provider);
                return _user;
            }
            if (!_user.ClientID.Equals(_settings.clientId))
            {
                _user = new CognitoUser(userId, _settings.clientId, _userPool, _provider);
                return _user;
            }
            return _user;
        }

        /*private Dictionary<string, string> GetUserAttributes(UserInfo userInfo) {
            List<string> attributeNameList = _regionInfo.userPoolIdToAttributesMapper[_settings.userPoolId];
            if (attributeNameList == null || attributeNameList.Count == 0) {
                return null;
            }
            Dictionary<string, string> userAttributes = new Dictionary<string, string>();
            *//*PropertyInfo  pi=userInfo.GetType().GetProperty("");
            pi.GetValue() as string*//*
            foreach (string attr in attributeNameList)
            {
                userAttributes.Add(attr,"");
            }
            return userAttributes;
        }*/

        //refresh idToken and accessToken
        private async void StartRefreshTokenAsync()
        {
            InitiateAuthRequest initiateAuthRequest = new InitiateAuthRequest()
            {
                AuthFlow = _settings.refresh_token_challenge,
                ClientId = _settings.clientId,
                AuthParameters = new Dictionary<string, string>()
                {
                    {"USERNAME", _user.UserID },
                    {"REFRESH_TOKEN", _settings.refreshToken }
                }
            };
            InitiateAuthResponse initiateResponse =
                await _provider.InitiateAuthAsync(initiateAuthRequest).ConfigureAwait(false);
            if (string.IsNullOrEmpty(initiateResponse.ChallengeName) &&
                string.IsNullOrEmpty(initiateResponse.AuthenticationResult.RefreshToken))
            {
                UpdateTokens(initiateResponse.AuthenticationResult.IdToken,
                    initiateResponse.AuthenticationResult.AccessToken,
                    _settings.refreshToken,
                    initiateResponse.AuthenticationResult.ExpiresIn);
            }
        }
        #endregion
    }
}
