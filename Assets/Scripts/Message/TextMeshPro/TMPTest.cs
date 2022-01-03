using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMPTest : MonoBehaviour
{
    //public GameObject Test;
    public GameObject content;
    public TMP_InputField inputmessage;
    public GameObject bubble;
    float scroll_width = 400;
    float icon_width = 35;
    float content_Height;
    float content_between = 5;
    int maxWord = 15;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SendMessage();
    }

    void SendMessage()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (GetText() != "")
            {
                if (GetText().Length <= maxWord)
                {
                    GameObject chat_bubble = bubble;
                    TextMeshProUGUI tmp = chat_bubble.GetComponentInChildren<TextMeshProUGUI>();
                    RectTransform rt = tmp.GetComponent<RectTransform>();
                    Vector2 v = rt.rect.size;

                    chat_bubble.GetComponentInChildren<TextMeshProUGUI>().text = GetText();
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tmp.preferredWidth);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tmp.preferredHeight);

                    float p_width = tmp.GetComponent<RectTransform>().sizeDelta.x;
                    float p_height = tmp.GetComponent<RectTransform>().sizeDelta.y;
                    chat_bubble.GetComponent<RectTransform>().sizeDelta = new Vector2(p_width, p_height);

                    AddItem(chat_bubble);
                }
                else
                {
                    GameObject chat_bubble = bubble;
                    TextMeshProUGUI tmp = chat_bubble.GetComponentInChildren<TextMeshProUGUI>();
                    RectTransform rt = tmp.GetComponent<RectTransform>();
                    Vector2 v = rt.rect.size;

                    chat_bubble.GetComponentInChildren<TextMeshProUGUI>().text = GetText();
                    /*int lineCount = tmp.GetTextInfo(tmp.text).lineCount;
                    print(lineCount);*/
                    
                    //tmp.GetPreferredValues();
             
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 130);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tmp.preferredHeight);
                    float p_width = tmp.GetComponent<RectTransform>().sizeDelta.x;
                    float p_height = tmp.GetComponent<RectTransform>().sizeDelta.y;

                    //print(p_height);
                    chat_bubble.GetComponent<RectTransform>().sizeDelta = new Vector2(p_width, p_height);

                    AddItem(chat_bubble);

                    
                }
            }


            inputmessage.text = "";
        }
    }

    void AddItem(GameObject obj)
    {
        int childcount = content.transform.childCount;
        

        if (childcount == 0)
        {
            GameObject clone0 = Instantiate(obj, content.transform);
            clone0.transform.localPosition = new Vector3(0, 0, transform.localPosition.z);

        }
        else
        {
            GameObject last_obj = content.transform.GetChild(childcount - 1).gameObject;
            float l_height = last_obj.GetComponent<RectTransform>().sizeDelta.y;
            float l_PosY = last_obj.transform.localPosition.y;

            GameObject clone = Instantiate(obj, content.transform);
            //print(clone.transform.localPosition.x);
            float bubble_width = clone.GetComponent<RectTransform>().sizeDelta.x;
            clone.transform.localPosition = new Vector3(scroll_width-icon_width-bubble_width, l_PosY - l_height - content_between, transform.localPosition.z);
            //print(clone.transform.localPosition.x);

            float sum_height = Mathf.Abs(clone.transform.localPosition.y - clone.GetComponent<RectTransform>().sizeDelta.y - content_between);

            if (sum_height > content.GetComponent<RectTransform>().sizeDelta.y)
            {
                content_Height += clone.GetComponent<RectTransform>().sizeDelta.y + content_between;
                content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, content_Height);

            }
        }
    }

    string GetText()
    {
        string str = "";
        str = inputmessage.text;
        return str;
    }
}
