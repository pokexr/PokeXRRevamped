using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;
    public ChatFriendsManager chatFriendsManager;

    public GameObject MesaageItemLeft;
    public GameObject MesaageItemRight;
    public GameObject ChatPanel;
    //public GameObject BottomBar;
    public GameObject FriendsContent;
    public GameObject ChatPanelPrefab;
    
    public RectTransform ChatPanelsHead;

    public Chat.User SetUser;
    public bool IsPanelOpened = false;
    public string PrivateChannelName;
    public Scrollbar scroll;

    private float temp;
    private GameObject PanelToWork;
    private RectTransform content;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {

    }
    public void Set_User(Chat.User user)
    {
        SetUser = user;
    }
    public void SetChannelName(string RecieverName)
    {
        PrivateChannelName = RecieverName;
        Debug.Log("SetChannelName " + PrivateChannelName);

    }
    public void CloseChatPanel()
    {
        ChatPanel.SetActive(false);
        PrivateChannelName = "";
        chatFriendsManager.GetFriendsList();
        //BottomBar.SetActive(true);
    }
    public void GenerateChatPanelOnPrivateRecieved(Chat.User RecieverUser)
    {
        bool check = false;
        foreach (Transform ChildPanel in ChatPanel.transform)
        {
            if (ChildPanel.gameObject.name == RecieverUser.id.ToString())
            {
                check = true;
                break;
            }
            else
            {
                check = false;
            }
        }
        if (check == false)
        {
            GameObject panel;
            panel = Instantiate(ChatPanelPrefab, ChatPanelsHead);
            Debug.Log("recieverr name " + RecieverUser.id.ToString());
            Debug.Log("GenerateChatPanel " + RecieverUser.id.ToString());
            panel.name = RecieverUser.id.ToString();
        }
    }
    public void GenerateChatPanel(Chat.User RecieverUser)
    {
        bool check = false;
        foreach (Transform ChildPanel in ChatPanel.transform)
        {
            if (ChildPanel.gameObject.name == PrivateChannelName)
            {
                check = true;
                break;
            }
            else
            {
                check = false;
            }
        }
        if (check == false)
        {
            GameObject panel;
            panel = Instantiate(ChatPanelPrefab, ChatPanelsHead);
            panel.GetComponent<ChatPanelManager>().Init(RecieverUser);
            panel.name = PrivateChannelName;
            PanelToWork = panel;
            scroll = panel.GetComponent<ChatPanelManager>().Scroll;
        }
    }
    public void CheckPanelExistance()
    {
        foreach (Transform ChildPanel in ChatPanel.transform)
        {
            Debug.Log("childs");
            ChildPanel.gameObject.SetActive(false);
            if (ChildPanel.gameObject.name == PrivateChannelName)
            {
                Debug.Log("Panel Already exist");
                ChildPanel.gameObject.SetActive(true);
                PanelToWork = ChildPanel.gameObject;
                content = ChildPanel.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject.transform.GetComponent<RectTransform>();
                scroll = PanelToWork.transform.GetComponent<ChatPanelManager>().Scroll;
            }
        }
    }

    IEnumerator PushChat()
    {
        yield return new WaitForEndOfFrame();
        scroll.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        PushChatToBottom();
    }
    public void PushChatToBottom()
    {
        Debug.Log("scroll.value "+ scroll.value);
        temp = scroll.value / 10;
        StartCoroutine(PushToBottom());
    }
    public IEnumerator PushToBottom()
    {
        if (scroll.value > 0)
        {
            yield return new WaitForSeconds(0.01f);
            Debug.Log("value " + scroll.value);
            scroll.value = scroll.value - temp;
            StartCoroutine(PushToBottom());
        }
    }
    //private IEnumerator AutoScroll()
    //{
    //    LayoutRebuilder.ForceRebuildLayoutImmediate(container);
    //    yield return new WaitForEndOfFrame();
    //    yield return new WaitForEndOfFrame();
    //    scrollRect.verticalNormalizedPosition = addNewToTop ? 1 : 0;
    //}
    public void RefreshChat()
    {
        StartCoroutine(RetrieveChat(SetUser.id));
    }
    public IEnumerator RetrieveChat(int id)
    {
        string requestName = "api/v1/chats?receiver_id=" + id + "&status=seen";
        //string requestName = "api/v1/users/get_consumed_points?onbases=all&userid=72";
        using (UnityWebRequest www = UnityWebRequest.Get(AuthManager.BASE_URL + requestName))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                //ConsoleManager.instance.ShowMessage("Network Error!");
                Debug.Log(www.error);
                LoadingManager.Instance.Loading.SetActive(false);
            }
            else
            {
                Debug.Log("chat " + www.downloadHandler.text);
                RetrieveRoot RetrievedChat = JsonUtility.FromJson<RetrieveRoot>(www.downloadHandler.text);
                if (RetrievedChat.success)
                {
                    RetrievedChatManager(RetrievedChat);
                }
                else
                {
                    //ConsoleManager.instance.ShowMessage("No Messages");
                }
                LoadingManager.Instance.Loading.SetActive(false);
            }
        }
    }
    

    public void RetrievedChatManager(RetrieveRoot RetrievedChat)
    {
        GenerateChatPanelOnPrivateRecieved(SetUser);
        if (RetrievedChat.data.Count > 0)
        {
            ClearMessages();
            for (int i = RetrievedChat.data.Count - 1; i >= 0; i--)
            {
                GameObject obj;
                if (SetUser.id != RetrievedChat.data[i].receiver_id)
                {
                    obj = Instantiate(MesaageItemLeft, content);
                    obj.SetActive(true);
                    obj.GetComponent<MessageItem>().SetMessageInfo("" + RetrievedChat.data[i].message, false);
                }
                else
                {
                    // this is sender
                    obj = Instantiate(MesaageItemRight, content);
                    obj.SetActive(true);
                    obj.GetComponent<MessageItem>().SetMessageInfo(RetrievedChat.data[i].message, true);
                }
            }
            StartCoroutine(PushChat());
        }
    }
    private void ClearMessages()
    {
        foreach (Transform Message in content)
        {
            Destroy(Message.gameObject);
        }
    }
    public void SendMessage(Chat.User User, string Message)
    {
        StartCoroutine(SendMsg(User.id.ToString(), Message));
    }
    IEnumerator SendMsg(string receiver_id, string message)
    {
        WWWForm form = new WWWForm();
        form.AddField("receiver_id", receiver_id);
        form.AddField("message", message);

        string requestName = "api/v1/chats";
        using (UnityWebRequest www = UnityWebRequest.Post(AuthManager.BASE_URL + requestName, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                //ConsoleManager.instance.ShowMessage("Sending Error!");
                Debug.Log(www.error);
                LoadingManager.Instance.Loading.SetActive(false);
            }
            else
            {
                Debug.Log("chat " + www.downloadHandler.text);
                GeneralResponce SentMsgResponce = JsonUtility.FromJson<GeneralResponce>(www.downloadHandler.text);
                if (SentMsgResponce.success)
                {
                    SpawnSentMsg(message);
                    StartCoroutine(PushChat());
                }
                else
                {

                }
                LoadingManager.Instance.Loading.SetActive(false);
            }
        }
    }
    public void SpawnSentMsg(string message)
    {
        GameObject obj;
        obj = Instantiate(MesaageItemRight, content);
        obj.SetActive(true);
        scroll = PanelToWork.GetComponent<ChatPanelManager>().Scroll;
        obj.GetComponent<MessageItem>().SetMessageInfo(message, true);
        PanelToWork.transform.GetComponent<ChatPanelManager>().MessageInputField.text = "";
    }
}
//[Serializable]
//public class Datum
//{
//    public int id;
//    public int sender_id;
//    public int receiver_id;
//    public string message;
//    public DateTime created_at;
//    public DateTime updated_at;
//}
//// Chat Class
//[Serializable]
//public class RetrieveRoot
//{
//    public bool success;
//    public List<Datum> data;
//}
//[Serializable]
//public class GeneralResponce
//{
//    public bool success;
//    public string msg;
//}