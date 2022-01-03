using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager 
{
    private static MessageManager instance;

    public static MessageManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new MessageManager();
                
            }
            return instance;
        }
    }

    public enum MessageType
    {
        sendMessage,recieveMessage
    }

    public void SendMessage()
    {

    }


    public void RecieveMessage()
    {

    }

    public void GetMessage()
    {

    }


    public void GetUnreadNum()
    {

    }

}
