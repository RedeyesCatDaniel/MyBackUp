using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LGUVirtualOffice.Log
{
    public class ApplicationLog :Singleton<ApplicationLog>, ILog
    {
        string url = "https://mr9fviwnaf.execute-api.ap-northeast-2.amazonaws.com/dev";
        string ApiKey = "u6G0UnzX93aYQfWOTGrMV796xWg0ZTjt6igYAup6";
        private ApplicationLog() { }
        public void LogDebug(object debugMessage)
        {
            LogApplicaton(debugMessage, null, ApplicationCode.Debug);
        }
        public void LogException(Exception e, string message)
        {
            LogApplicaton(message, e, ApplicationCode.Error);
        }
        public void LogException(Exception e)
        {
            LogException(e, "default message");
        }
        public void Log(object message)
        {
            LogApplicaton(message, null, ApplicationCode.Success);
        }
        void LogApplicaton(object message,object data,ApplicationCode applicationCode) 
        {
            ApplicationData applicationData = new ApplicationData()
            {
                applicationCode = applicationCode,
                message = message.ToString(),
                data = data
            };
            Loghttp.Post(url, ApiKey, applicationData.ToJson());
            LogUtil.LogInfo(applicationData.ToJson());
        }
        public void LogWarning(object message)
        {
            LogApplicaton(message, null, ApplicationCode.Warning);
        }
        public void Log<T>(T message) where T : JsonData
        {
            Log(message.ToJson());
        }
        public void LogError(object message)
        {
            LogApplicaton(message, null, ApplicationCode.Error);
        }
        public void Assert(bool condition, object message)
        {
            if (!condition)
            {
                LogError(message);
            }
        }
    }
    public enum ApplicationCode 
    {
        Debug=1,
        Error=2,
        Log=123,
        Success=6,
        Warning=4
    }
    public class ApplicationData : JsonData 
    {
        public ApplicationCode applicationCode;
        public string message;
        public object data;
    }
}