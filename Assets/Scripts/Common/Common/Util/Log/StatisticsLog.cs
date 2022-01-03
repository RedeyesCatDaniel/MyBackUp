using System;
using UnityEngine;
namespace LGUVirtualOffice.Log
{
    public class StatisticsLog :Singleton<StatisticsLog>,ILog
    {
        readonly string url = "https://squhjdtenf.execute-api.ap-northeast-2.amazonaws.com/dev/";
        readonly string APIkey = "WXjWAaT8vs92oplzBzQ8R5VIFFLaP3gsaQSRDaW0";
        private StatisticsLog() { }
        public void LogDebug(object debugMessage)
        {
            LogUtil.LogDebug(debugMessage);
        }

        public void LogException(Exception e, string message)
        {
            LogUtil.LogException(e, message);
        }

        public void LogException(Exception e)
        {
            LogUtil.LogException(e);
        }
        public void Log(object message)
        {
            LogUtil.LogInfo(message);
        }
        public void Log<T>(T message)where T:JsonData
        {
                LogToJson(message);
           
        }
        private void LogToJson(JsonData t)
        {
            LogUtil.LogInfo(t);
            Loghttp.Post(url, APIkey, t.ToJson());

        }

        public void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        public void LogError(object message)
        {
            LogUtil.LogError(message);
        }

        public void Assert(bool condition, object message)
        {
            LogUtil.Assert(condition, message);
        }
    }
}