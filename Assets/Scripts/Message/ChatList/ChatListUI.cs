using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatListUI : MonoBehaviour
{
    public GameObject chatListPrefab;
    public GameObject content;
    public TMP_InputField search_InputField;

    

    List<GameObject> closeObj = new List<GameObject>();

    void Start()
    {
        ChatList.Instance.InitList();
        for(int i = 0; i < ChatList.Instance.chatListMembers.Count; i++)
        {
            GameObject obj = Instantiate(chatListPrefab, content.transform);
            string channel = ChatList.Instance.channelLists[i];
            ChangeStatus(obj, channel);
            ChatList.Instance.AddChatListObj(channel, obj);
        }

        search_InputField.onValueChanged.AddListener(SearchChatMember);
    }

    // Update is called once per frame
    void Update()
    {
        //SearchChatMember();
        /*if (Input.GetKeyDown(KeyCode.K))
        {
            ChatListMember newData = new ChatListMember("changeName", "cLast", "2021-12-20", 1, true);
            NewMessageRefresh("channel2", newData);
        }*/
    }


    void ChangeStatus(GameObject obj,string channel)
    {
        GameObject NameText = obj.transform.Find("ChatText/NameAndTime/NameText").gameObject;
        NameText.GetComponent<TextMeshProUGUI>().text = ChatList.Instance.chatListMembers[channel].Name;
        GameObject LastContent = obj.transform.Find("ChatText/ChatContent/LastContent").gameObject;
        LastContent.GetComponent<TextMeshProUGUI>().text = ChatList.Instance.chatListMembers[channel].LastWord;
        GameObject LastTime = obj.transform.Find("ChatText/NameAndTime/TimeText").gameObject;
        LastTime.GetComponent<TextMeshProUGUI>().text = ChatList.Instance.chatListMembers[channel].LastWordTime;
        GameObject UnReadNum = obj.transform.Find("ChatText/ChatContent/Notification/UnReadNumber").gameObject;
        UnReadNum.GetComponent<TextMeshProUGUI>().text = ChatList.Instance.chatListMembers[channel].UnReadNum.ToString();

    }


    void NewMessageRefresh(string channel,ChatListMember chatListMember)
    {
        chatListMember.LastWordTime = System.DateTime.Now.ToString();
        ChatList.Instance.chatListMembers[channel] = chatListMember;
        ChangeStatus(ChatList.Instance.chatListObj[channel], channel);
        ChatList.Instance.chatListObj[channel].transform.SetSiblingIndex(0);
    }


    void SearchChatMember(string s)
    {
        if (s == "")
        {
            for (int i = 0; i < content.transform.childCount; i++)
            {
                content.transform.GetChild(i).gameObject.SetActive(true);
            }
            return;
        }
        for (int i = 0; i < ChatList.Instance.chatListObj.Count; i++)
        {
            string channel = ChatList.Instance.channelLists[i];
            if (!ChatList.Instance.chatListMembers[channel].Name.Contains(s))
            {
                ChatList.Instance.chatListObj[channel].SetActive(false);
            }
            else
            {
                ChatList.Instance.chatListObj[channel].SetActive(true);
            }
        }
    }

}
