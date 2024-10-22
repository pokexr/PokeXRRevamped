using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    public Chat.User ChatUser;
    public ImageController imageController;
    public GameObject ImageStatus;
    public GameObject UnreadMsgImage;
    public GameObject OnlineFriendItem;
    public Text ChatUsername;
    public string ChatChannel;
    public string Tempusername;

    public void Init(Chat.User user)
    {
        Debug.Log(" FriendItem id and URL " + ChatUser.id + ", " + ChatUser.image_url);
        if (ChatUser.image_url=="" || ChatUser.image_url==null)
        {
            Debug.Log("There is no image url");
        }
        else
        {
            imageController.Init(ChatUser.id, ChatUser.image_url);
            CheckOnlineStatus();
        }
        

    }
    private void CheckOnlineStatus()
    {
        ImageStatus.SetActive(false);
        //if (ChatUser.status_online)
        //{
        //    ImageStatus.SetActive(true);
        //}
        //else
        //{
        //    ImageStatus.SetActive(false);
        //}
    }
    public void SetRecieverName()
    {
        ChatManager.Instance.ChatPanel.SetActive(true);
        //ChatManager.Instance.BottomBar.SetActive(false);
        ChatManager.Instance.SetChannelName(ChatUser.id.ToString());
        ChatManager.Instance.Set_User(ChatUser);
        ChatManager.Instance.GenerateChatPanel(ChatUser);
        ChatManager.Instance.CheckPanelExistance();
        OnlineFriendItem.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        LoadingManager.Instance.Loading.SetActive(true);
        ChatManager.Instance.IsPanelOpened = true;
        ChatManager.Instance.RefreshChat();
    }

}
