using System;
using UnityEngine;
using Sentry;
using Sentry.Unity;
using System.Threading.Tasks;
using System.IO;
using UnityEditor;

namespace LGUVirtualOffice.Log
{
    public class SentryLog :Singleton<SentryLog>, ILog
    { 
        LogSetting logSetting = LogSetting.Instance;
        private SentryLog() 
        {
           
            Init();
        }
        void Init() 
        {
            SentryUnity.Init(o =>
            {
                o.Dsn = logSetting.sentryurl;
                o.Enabled = logSetting.enableSentry;
                o.DiagnosticLevel = logSetting.sentryLevel;
                foreach (var item in logSetting.tags)
                {
                    o.DefaultTags.Add(item.Key, item.Value);
                }
            });
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = new User
                {
                    Username = logSetting.username,
                };
            });
        }
        public void LogDebug(object debugMessage)
        {
            Init();
            Debug.Log(debugMessage);
        }

        public void LogException(Exception e, string message)
        {
            Init();
           
            Exception exception = new Exception(message + "\n" + e.Message);
            Debug.LogException(exception);
            //LogUtil.LogInfo(message);
            //SentrySdk.CaptureException(e);
        }

        public void LogException(Exception e)
        {
            Init();
            Debug.LogException(e);
            //SentrySdk.CaptureException(e);
        }

        public void Log(object message)
        {
            Init();
            Debug.Log(message);
        }
       
        public void ChangeUserName(string username) 
        {
            logSetting.username = username;
        }

        public void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        public void Log<T>(T message) where T : JsonData
        {
            Init();
            Log(message);
        }

        public void LogError(object message)
        {
            Init();
            Debug.LogError(message);
        }

        public void Assert(bool condition, object message)
        {
            Init();
            Debug.Assert(condition, message);
        }
        //static void Create()
        //{
        //    // 实例化类  Bullet
        //    ScriptableObject bullet = ScriptableObject.CreateInstance<LogSetting>();

        //    // 如果实例化 Bullet 类为空，返回
        //    if (!bullet)
        //    {
        //        Debug.LogWarning("Bullet not found");
        //        return;
        //    }

        //    // 自定义资源保存路径
        //    string path = Application.dataPath + "/BulletAeeet";

        //    // 如果项目总不包含该路径，创建一个
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    //将类名 Bullet 转换为字符串
        //    //拼接保存自定义资源（.asset） 路径
        //    path = string.Format("Assets/BulletAeeet/{0}.asset", (typeof(LogSetting).ToString()));

        //    // 生成自定义资源到指定路径
        //    AssetDatabase.CreateAsset(bullet, path);
        //}
        
    }
}