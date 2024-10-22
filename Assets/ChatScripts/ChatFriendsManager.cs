using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatFriendsManager : MonoBehaviour
{
    public GameObject ChatFriendPrefab;
    public GameObject ChatFriendsPanel;
    public Transform ChatFriendListContent;
    public GameObject ChatFriendListParent;
    public InputField InputChatUsername;
    public int FriendsPerPage = 50;

    private string TempName;
    private int ChatFriendsPage;
    private GameObject TempUser;
    private Chat.FriendsList ChatFriendsList = new Chat.FriendsList();
    void Start()
    {
        if (FriendProfileManager.Instance.FriendProfileMessage)
        {
            EnableChat();
        }
        else
        {
            LoadingManager.Instance.Loading.SetActive(true);
            GetFriendsList();
        }
        //GetFriendsList();
    }
    private void EnableChat()
    {
        ChatManager.Instance.ChatPanel.SetActive(true);
        //ChatManager.Instance.BottomBar.SetActive(false);
        ChatManager.Instance.SetChannelName(FriendProfileManager.Instance.MessageUser.id.ToString());
        ChatManager.Instance.Set_User(FriendProfileManager.Instance.MessageUser);
        ChatManager.Instance.GenerateChatPanel(FriendProfileManager.Instance.MessageUser);
        ChatManager.Instance.CheckPanelExistance();
        LoadingManager.Instance.Loading.SetActive(true);
        ChatManager.Instance.IsPanelOpened = true;
        ChatManager.Instance.RefreshChat();
    }
    public void CloseChatSearch()
    {
        InputChatUsername.text = "";
        ClearFriendsList(ChatFriendListParent, true);
    }
    public void SearchFromChatFriendList()
    {
        SearchFriend(InputChatUsername, ChatFriendListParent);
    }

    public void GetFriendsList()
    {
        ChatFriendsPanel.SetActive(true);
        ClearChatFriendsList();
        ChatFriendsList.users = new List<Chat.User>();
        ChatFriendsPage = 1;
        LoadingManager.Instance.Loading.SetActive(true);
        StartCoroutine(PostGetFriendsList());

        //ChatFriendsPage = 1;
    }
    public void CloseSearch()
    {
        InputChatUsername.text = "";
        ClearFriendsList(ChatFriendListParent, true);
    }
    public void SearchFriend(InputField Username, GameObject ParentContent)
    {
        if (Username.text.Length > 2)
        {
            ClearFriendsList(ParentContent, false);
            foreach (Transform Friend in ParentContent.transform)
            {
                TempName = Friend.gameObject.GetComponent<FriendItem>().ChatUser.username.ToString();
                if (Username.text.Length <= TempName.Length)
                {
                    if (TempName.Substring(0, Username.text.Length).ToUpper() == Username.text.ToUpper())
                    {
                        Friend.gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            ClearFriendsList(ParentContent, true);
        }

    }
    public void ClearFriendsList(GameObject ParentContent, bool Value)
    {
        foreach (Transform Friend in ParentContent.transform)
        {
            Friend.gameObject.SetActive(Value);
        }
    }
    private IEnumerator PostGetFriendsList()
    {
        string requestName = "api/v1/users/chat_user_list?page="+ ChatFriendsPage;
        string request = AuthManager.BASE_URL + requestName;
        // Debug.Log(request);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(request))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = request.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    //ChatFriendsList = JsonUtility.FromJson<Chat.FriendsList>(webRequest.downloadHandler.text);
                    Chat.FriendsList ResponceFriendList = JsonUtility.FromJson<Chat.FriendsList>(webRequest.downloadHandler.text);
                    for (int i = 0; i < ResponceFriendList.users.Count; i++)
                    {
                        ChatFriendsList.users.Add(ResponceFriendList.users[i]);
                    }
                    if (ResponceFriendList.users.Count == FriendsPerPage)
                    {
                        ChatFriendsPage++;
                        LoadingManager.Instance.Loading.SetActive(true);
                        StartCoroutine(PostGetFriendsList());
                    }
                    else
                    {
                        GenerateChatfriendList(ChatFriendsList);
                        LoadingManager.Instance.Loading.SetActive(false);
                    }
                    //GenerateChatfriendList(ChatFriendsList);
                    //LoadingManager.Instance.Loading.SetActive(false);
                    break;
            }
        }
    }
    public void GenerateChatfriendList(Chat.FriendsList ChatFriendsList)
    {
        //ClearChatFriendsList();
        foreach (var i in ChatFriendsList.users)
        {
            //if (i.any_chat == true)
            //{

            //}
            TempUser = Instantiate(ChatFriendPrefab, ChatFriendListContent);
            TempUser.GetComponent<FriendItem>().Tempusername = i.username;
            TempUser.GetComponent<FriendItem>().ChatUsername.text = i.username;
            TempUser.GetComponent<FriendItem>().ChatUser = i;
            TempUser.name = i.id.ToString();
            TempUser.GetComponent<FriendItem>().Init(i);
        }
    }
    private void ClearChatFriendsList()
    {
        foreach (Transform ChatFriendItem in ChatFriendListContent.transform)
        {
            GameObject.Destroy(ChatFriendItem.gameObject);
        }
    }
}

//[Serializable]
//public class FriendsList
//{
//    public List<User> users;
//}