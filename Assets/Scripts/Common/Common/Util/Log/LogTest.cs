using LGUVirtualOffice.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sentry;
using Sentry.Unity;

namespace LGUVirtualOffice
{
    public class LogTest : AbstractController
    {

        
        //private void Awake()
        //{
        //    LogUtil.LogInfo("");
        //}
        // Start is called before the first frame update
        void Awake()
        {
            //ExampleUpdataJsondata();
            //LogUtil.LogDebug("aasdd");
            //LogUtil.Assert(false, "sss");
            //LogUtil<SentryLog>.Logobj.Switch(Sentry.SentryLevel.Error);
            //LogSetting.Instance.enableSentry = true;
            //LogUtil.LogInfo("Test Add Tags");
            Debug.Log(Application.streamingAssetsPath);
            //Example1();
            //LogSetting.Instance.enableSentry = false;
            //LogSetting.Instance.username = "Test Tag";
            //LogSetting.Instance.tags.Add(new SentryTags() {Key="key1" ,Value ="Value1"});
            //Example1();
        }
        private void ExampleUpdataJsondata()
        {
            Login userlogin = new Login()
            {
                type = "1",
                name = "user1",
                team_name = "team1",
                user_id = "ass",
                team_id = "",
                time = "12:00",

            };
            LogUtil.StatisticsLog<Login>(userlogin);
            LogUtil.StatisticsLog(userlogin);
        }
        private void Example1()
        {
            Exception e=new Exception("test");
            LogUtil.LogException(e,"Message");

        }
    }
}