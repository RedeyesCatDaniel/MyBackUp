using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class MyModeViewSizeControl : MonoBehaviour
{
    public TMP_InputField URLInputField;
    public TMP_InputField NameInputField;

    public RectTransform content;
    public GameObject prefeb;
    public RectTransform ImageSize;

    float buttonWidth = 71f;
    float buttonBetweenLength = 20f;
    float StaticLength = 70f;
    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CreatButton()
    {
        if (IsURL(URLInputField.text) == true)
        {
            GameObject clone = prefeb;
            clone.GetComponentInChildren<OpenURL>().url = URLInputField.text;
            if (NameInputField.text != "")
            {
                clone.GetComponentInChildren<TextMeshProUGUI>().text = NameInputField.text;
            }
            AddButton(clone);
        }
    }

    void Refresh()
    {
        int buttonCount = content.childCount - 1;
        print(buttonCount);
        float backGroundWidth = buttonWidth * buttonCount + (buttonCount - 1) * buttonBetweenLength + StaticLength;
        ImageSize.sizeDelta = new Vector2(backGroundWidth, 166);
    }

    bool IsURL(string InputURL)
    {
        Regex reg = new Regex(@"^(https?|ftp|file|ws)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$");
        return reg.IsMatch(InputURL);
    }

    void AddButton(GameObject obj)
    {
        int buttonCount = content.childCount - 1;
        if (buttonCount >= 7)
        {
            return;
        }
        GameObject btnObj = Instantiate(obj, content.transform);


        btnObj.transform.localPosition = new Vector3(buttonCount * buttonWidth + buttonCount * buttonBetweenLength - 292 + StaticLength / 2, 83, 0);
        Refresh();
    }


}
