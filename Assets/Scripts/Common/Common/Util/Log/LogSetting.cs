using Sentry;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LGUVirtualOffice
{
   
    //[CreateAssetMenu(fileName = "LogSetting", menuName = "LogSetting", order = 1)]
    public class LogSetting : Singleton<LogSetting>
    {
        private LogSetting() { }
        //[Tooltip("enable sentry")]
        public bool enableSentry = false;
        //[Tooltip("sentry url")]
        public string sentryurl = "https://5510077d9a0348dca1b17e0df2e3a829@sentry.work.uplus.co.kr/3";
        //[Tooltip("sentry leave")]
        public SentryLevel sentryLevel = SentryLevel.Info;
        //[Tooltip("sentry Username")]
        public string username = "Default Client";
        //[Tooltip("sentry Email")]
        public string email = "";
        //[Tooltip("sentry Tags")]
        public List<SentryTags> tags=new List<SentryTags>();
        
        
       
       

    } 
   
     public class SentryTags 
        {
        public string Key;
        public string Value;
        }
}