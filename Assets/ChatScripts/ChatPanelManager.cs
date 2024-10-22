using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ChatPanelManager : MonoBehaviour
{
    //public TMP_InputField TMP_MessageInputField;
    public InputField MessageInputField;
    public Scrollbar Scroll;
    public Image UserImage;
    public Chat.User PanelUser;
    public Text Username;
    private string localURL;

    

    //void Start()
    //{
    //    //MessageInputField.onEndEdit.AddListener(OnMobileInputDone);
    //}
    //private void OnMobileInputDone(string input)
    //{
    //    // This function will be called when the mobile input is done (when the user presses the "Done" button on the keyboard)
    //    Debug.Log("Mobile input done. Entered text: " + input);

    //    // Call any other functions or actions you want to execute when the input is done.
    //    // For example, you might want to process the input or trigger some event.
    //    SendPrivateMsg();
    //}
    public void Init(Chat.User user)
    {
        PanelUser = user;
        Username.text = user.username;
        if (user.image_url==""|| user.image_url==null)
        {
            Debug.Log("There is no Image");
        }
        else
        {
            GetChatUserImage();
        }
        
    }
    public void GetChatUserImage()
    {
        Debug.Log("id and URL " + PanelUser.id + ", " + PanelUser.image_url);
        localURL = string.Format("{0}/{1}.jpg", Application.persistentDataPath, "" + PanelUser.id);

        if (File.Exists(localURL))
        {
            LoadLocalFile();
        }
        else
        {
            StartCoroutine(GetThumbnail(PanelUser.image_url));
        }
        //if (PanelUser.prfile_image_url != "" || PanelUser.prfile_image_url != null)
        //{
        //    LoadingManager.instance.loading.SetActive(true);
        //    StartCoroutine(GetThumbnail(PanelUser.prfile_image_url));
        //}
    }
    public void LoadLocalFile()
    {
        byte[] bytes;
        bytes = File.ReadAllBytes(localURL);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        Sprite thumbnail = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        UserImage.sprite = thumbnail;
    }
    public void CloseChat()
    {
        if (FriendProfileManager.Instance.FriendProfileMessage)
        {
            SceneManager.LoadScene(1);;
        }
        else
        {
            ChatManager.Instance.IsPanelOpened = false;
            ChatManager.Instance.CloseChatPanel();
        }
    }
    public void Refresh_Chat()
    {
        //LoadingManager.instance.loading.SetActive(true);
        ChatManager.Instance.RefreshChat();
    }
    public void SendPrivateMsg()
    {
        if (MessageInputField.text != "")
        {
            LoadingManager.Instance.Loading.SetActive(true);
            ChatManager.Instance.SendMessage(PanelUser, MessageInputField.text);
        }
    }
    IEnumerator GetThumbnail(string uri)
    {
        Debug.Log(uri);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
        www.SetRequestHeader("Content-type", "application/json");
        //www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            LoadingManager.Instance.Loading.SetActive(false);
            Debug.Log(www.responseCode);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            Sprite thumbnail = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            UserImage.sprite = thumbnail;
            LoadingManager.Instance.Loading.SetActive(false);
        }
    }
}
//[Serializable]
//public class User
//{
//    public int id;
//    public string first_name;
//    public string last_name;
//    public string email;
//    public string prfile_image_url;
//    public string username;
//    public string notification_id;
//    public int bounty_count;
//    public int month_steps_count;
//    public bool status_online;
//    public bool any_chat;
//}
