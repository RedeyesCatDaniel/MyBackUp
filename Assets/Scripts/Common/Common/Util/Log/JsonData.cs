using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace LGUVirtualOffice.Log
{
    public class JsonData
    {
        //json
        /*        {
            "Login":{"user_id":"xxxx", "name":"xxxx", "team_id":"xxxx", "team_name":"xxxxx", "type":"in/out", "time":"MM/DD/YY-HH:mm:ss"},
        "Chat":{"user_id":"xxxxxx","name":"xxxxx", "team_id":"xxxx", "team_name":"xxxxx","time":"MM/DD/YY","chat_type":"xxxxx","duration":"xxx"},
          "Menu":{"name":"xxxx", "action":"open/close", "time":"MM/DD/YY-HH:mm:ss"},
          "Camera":{"user_id":"xxxxx","name":"xxxx", "team_id":"xxxx", "team_name":"xxxxx","OpenTime":"xxxx"},
          "Avatar":{"user_id":"xxxx", "name":"xxxx", "team_id":"xxxx", "team_name":"xxxxx","action":"Changed/NonChange","faceId":"xx", "hairId":"xx", "eyeId":"xx","noseId":"xx", "mouthId":"xx", "blowId":"xx", "beardId":"xx", "time":"MM/DD/YY"},
          "Recall":{"user_id":"xxxx", "name":"xxxx", "team_id":"xxxx", "team_name":"xxxxx", "org_time":"MM/DD/YY-hh:mm", "recall_time":"MM/DD/YY-hh:mm"},
        "Status":{"user_id":"xxxx", "name":"xxxx", "team_id":"xxxx", "team_name":"xxxxx", "status_type":"xxxx"},
          "Message":{"user_id":"xxxx", "name":"xxxx", "team_id":"xxxx", "team_name":"xxxxx", "time":"MM/DD/YY"},
        "Movement":{"user_id":"xxxx", "name":"xxxx", "team_id":"xxxx", "team_name":"xxxxx", "action_type":"keyboard/mouse", "duration":"xxxx"},
        "Gesture":{"user_id":"xxxx", "name":"xxxx", "team_id":"xxxx", "team_name":"xxxxx", "gesture_type":"xxxx"}
        }*/
        public string ToJson()
        {
            string[] type=base.ToString().Split('.');
            return "{\""+type[type.Length-1]+"\":"+ JsonConvert.SerializeObject(this)+"}";
        }
        public override string ToString()
        {
            return ToJson();
        }

    }
   
    public class Login:JsonData
    {
        
        public string user_id { get; set; }
        public string name { get; set; }
        public string team_id { get; set; }
        public string team_name { get; set; }
        public string type { get; set; }
        public string time { get; set; }
    }
    public class Chat : JsonData
    {
        
        public string user_id { get; set; }
        public string name { get; set; }
        public string team_id { get; set; }
        public string team_name { get; set; }
        public string time { get; set; }
        public string chat_type { get; set; }
        public string duration { get; set; }
    }
    public class Menu : JsonData
    {
        public string name { get; set; }
        public string action { get; set; }
        public string time { get; set; }
    }
    public class Camera : JsonData
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public string team_id { get; set; }
        public string team_name { get; set; }
        public string OpenTime { get; set; }
    }
    public class Avatar : JsonData
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public string team_id { get; set; }
        public string team_name { get; set; }
        public string action { get; set; }
        public string faceId { get; set; }
        public string hairId { get; set; }
        public string eyeId { get; set; }
        public string noseId { get; set; }
        public string mouthId { get; set; }
        public string blowId { get; set; }
        public string beardId { get; set; }
        public string time { get; set; }
    }
    public class Recall : JsonData
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public string team_id { get; set; }
        public string team_name { get; set; }
        public string org_time { get; set; }
        public string recall_time { get; set; }
    }
    public class Status : JsonData
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public string team_id { get; set; }
        public string team_name { get; set; }
        public string status_type { get; set; }
    }
    public class Message : JsonData
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public string team_id { get; set; }
        public string team_name { get; set; }
        public string time { get; set; }
    }
    public class Movement : JsonData
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public string team_id { get; set; }
        public string team_name { get; set; }
        public string action_type { get; set; }
        public string duration { get; set; }
    }
    public class Gesture : JsonData
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public string team_id { get; set; }
        public string team_name { get; set; }
        public string gesture_type { get; set; }
    }
}