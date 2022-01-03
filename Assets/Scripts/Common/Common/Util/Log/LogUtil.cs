using LGUVirtualOffice.Log;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LGUVirtualOffice {
    #region LogUtil<T>选用不同的log方式
    public static class LogUtil<T> where T:Singleton<T>,ILog
    {
        private static T logobj;
        public static T LogObject
        {
            get
            {
                if (logobj==null)
                {
                    logobj = Singleton<T>.Instance;
                }
                return logobj;
            }
        }
        public static void LogDebug(object debugMessage)
        {
            LogObject.LogDebug(debugMessage);
        }
        public static void LogInfo(object infoMessage)
        {
            LogObject.Log(infoMessage);
        }

        public static void LogException(string errorMessage, Exception e)
        {
            LogObject.LogException(e, errorMessage);
        }
        public static void LogException(Exception e)
        {
            LogObject.LogException(e);
        }
        public static void LogError(object message) 
        {
            LogObject.LogError(message);
        }
        public static void LogInfo<K>(K message) where K : JsonData
        {
            LogObject.Log(message);
        }
        public static void LogWarning(object message) 
        {
            LogObject.LogWarning(message);
        }
        public static void Assert(bool condition, object message) 
        {
            LogObject.Assert(condition, message);
        }
    }
    #endregion
    public static class LogUtil 
    {
        private static LogSetting logSetting;
        public static LogSetting LogSetting
        {
            get
            {
                if (logSetting==null)
                {
                    logSetting = LogSetting.Instance;
                }
                return logSetting;
            } 
            set { 
                logSetting = value; 
            }
        }
        public static void LogDebug(object debugMessage)
        {
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
            //string s = st.GetFrame(0).GetFileName() +":"+ st.GetFrame(0).GetFileLineNumber();
            //Debug.Log(debugMessage);
            LogUtil<SentryLog>.LogDebug(debugMessage);
        }
        public static void LogException(Exception exception)
        {
            LogUtil<SentryLog>.LogException(exception);
        }
        public static void LogWarning(object message) 
        {
            //Debug.LogWarning(message);
            LogUtil<SentryLog>.LogWarning(message);
        }
        public static void LogInfo(object infoMessage)
        {
            //Debug.Log(infoMessage);
            LogUtil<SentryLog>.LogInfo(infoMessage);
        }
        public static void LogError(string errorMessage, Exception e)
        {
            LogUtil<SentryLog>.LogException(errorMessage ,e);
        }
        public static void LogException(Exception e, string errorMessage) 
        {
            LogUtil<SentryLog>.LogException(errorMessage, e);
        }
        public static void Assert(bool condition,object message) 
        {
            LogUtil<SentryLog>.Assert(condition, message);
        }
        public static void StatisticsLog<T>(T message)where T:JsonData 
        {
            LogUtil<StatisticsLog>.LogInfo(message);
        }
        public static void LogError(object message) 
        {
            //Debug.LogError(message);
            LogUtil<SentryLog>.LogError(message);
        }
       
    }
}
