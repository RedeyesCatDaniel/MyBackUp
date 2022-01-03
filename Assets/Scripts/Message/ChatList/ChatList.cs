using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatList 
{
    private static ChatList instance;
    public static ChatList Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChatList();
            }
            return instance;
        }
    }


    public List<string> channelLists = new List<string>();

    public Dictionary<string, ChatListMember> chatListMembers = new Dictionary<string, ChatListMember>();

    public Dictionary<string, GameObject> chatListObj = new Dictionary<string, GameObject>();
    public void InitList()
    {
       
        ChatListMember clm = new ChatListMember("anhaoming","last word", "2021-12-15", 5, true);
        AddListMember("channel1", clm);
        ChatListMember clm1 = new ChatListMember("ashaoming", "byebye", "2021-12-16", 3, false);
        AddListMember("channel2", clm1);
        ChatListMember clm2 = new ChatListMember("anhaohh", "hello", "2021-12-14", 1, false);
        AddListMember("channel3", clm2);

    }

   
    public void AddListMember(string channel,ChatListMember chatListMember)
    {
        channelLists.Add(channel);
        chatListMembers.Add(channel, chatListMember);
    }

    public void AddChatListObj(string channel,GameObject obj)
    {
        chatListObj.Add(channel,obj);
    }

    public void ChangeChatListMember(string channel,ChatListMember chatListMember)
    {
        if (!channelLists.Contains(channel))
        {
            return;
        }
        else
        {
            chatListMembers[channel] = chatListMember;
        }
    }
   

}
