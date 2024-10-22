using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageItem : MonoBehaviour
{
    public Image image;
    public GameObject ImageParent;
    public TextMeshProUGUI Message;
    public TextMeshProUGUI date;

    public void SetMessageInfo(string message, bool mine = false)
    {
        int lenght = message.Length;
        Debug.Log("mesaage length " + lenght);
        Message.text = message;
        this.date.text = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}";
        if (lenght > 49)
        {
            Message.alignment = TextAlignmentOptions.Justified;
        }
        if (mine)
        {
            image.rectTransform.sizeDelta = new Vector2(Math.Min(713, Message.GetPreferredValues().x + 30), 20);
        }
        else
        {
            image.rectTransform.sizeDelta = new Vector2(Math.Min(713, Message.GetPreferredValues().x + 60), 20);
        }
        Message.ForceMeshUpdate();
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(972, ((int)Message.GetPreferredValues().y + 100));
        gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(713, Message.GetPreferredValues().y + 10);
        Message.ForceMeshUpdate();
    }
}
