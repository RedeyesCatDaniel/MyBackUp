using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelList 
{
    List<string> channellist = new List<string>();


    public void AddChannel(string channel)
    {
        channellist.Add(channel);
    }
}
