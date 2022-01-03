using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatListMember 
{
    public string Name { get; private set; }

    public string LastWord { get; private set; }

    public string LastWordTime { get; set; }

    public int UnReadNum { get; private set; }

    public bool IsOnline { get; private set; }


    public ChatListMember(string Name,string LastWord,string LastWordTime,int UnReadNum,bool IsOnline)
    {
        this.Name = Name;
        this.LastWord = LastWord;
        this.LastWordTime = LastWordTime;
        this.UnReadNum = UnReadNum;
        this.IsOnline = IsOnline;
    }


}
