using System;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(fileName = "AWSSetting", menuName = "ScriptableObjects/AWSSetting", order = 2)]
    public class AWSSetting : ScriptableObject
    {
        [Tooltip("AWS Cognito User Pool Id")]
        public string userPoolId = "ap-northeast-2_cXeGRmfe4";
        [Tooltip("AWS Cognoto IdentityPool Id")]
        public string identityPoolId = "ap-northeast-2:6cf84a4d-86a1-4473-a877-feec4fa9c2c3";
        [Tooltip("Id of App Client In the relevant AWS Cognito User Pool")]
        public string clientId = "1uqmf4bftmbjooeh55r7d74oul";

        [Tooltip("Number of messages to achieve each request")]
        public int SQS_Max_Message_Num = 10;
        [Tooltip("The count of maximun cached deleted MessageId,used to avoid duplicate message receiving")]
        public int SQS_Max_Delete_Count = 20;
        [Tooltip("Flags how many items should be delete when deleted MessageId cache reached the maximum")]
        public int SQS_Delete_Message_Count = 10;
        [Tooltip("Flags If a message sent how many seconds ago,it shoud be delete")]
        public int SQS_Message_Delete_Time = 3;
        [Tooltip("The interval of each GetMessage request")]
        public float SQS_Message_Process_interval = 1.0f;
        public string SQS_Queue_Url = "https://sqs.ap-northeast-2.amazonaws.com/074097323854/TestQueue";


        [HideInInspector, NonSerialized]
        public readonly string password_generate_salt = "LGUVirtualOffice_PASSWORD";
        [HideInInspector, NonSerialized]
        public readonly string userid_generate_salt = "LGUVirtualOffice_USERID";
        [HideInInspector, NonSerialized]
        public readonly string dateTime_convert_format = "yyyy-MM-ddThh:mm:ss.sss";

        //Asia Pacific (Seoul)
        public string region = "Asia Pacific (Seoul)";


        [HideInInspector, Tooltip("Session Id Got from AWS Cognito")]
        public string userSessionId;

        [HideInInspector, Tooltip("idToken Got from AWS Cognito After successful Authentication")]
        public string idToken;

        [HideInInspector, Tooltip("accessToken Got from AWS Cognito After successful Authentication")]
        public string accessToken;

        [HideInInspector, Tooltip("refreshToken Got from AWS Cognito After successful Authentication, being used to refresh the idToken and accessToken")]
        public string refreshToken;

        [HideInInspector, Tooltip("The Time User Session Id Expired")]
        public DateTime sessionExpirationTime;

        [HideInInspector, Tooltip("session will be expired in 150 seconds")]
        public int sessionExpiresIn = 150;

        [HideInInspector, Tooltip("The Time idToken and accessToken Expired")]
        public DateTime tokenExpirationTime;

        [HideInInspector]
        public string poolRegion = "";
        [HideInInspector]
        public string providerName = "";
        public readonly string reset_password_challenge = "NEW_PASSWORD_REQUIRED";
        public readonly string refresh_token_challenge = "REFRESH_TOKEN";
    }
}
