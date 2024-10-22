using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FriendUser : MonoBehaviour
{
    public Text nameText;
    public Text UsernameText;
    //public ImageController imageController;
    private string ImageLocalPath;

    public FriendList.User User;
    public Image UserImage;
    public Sprite DefaultSprite;
    private string localURL;
    public void Init(FriendList.User user)
    {
        Debug.Log("name "+ user.name);
        Debug.Log("username "+ user.username);
        Debug.Log("image_url " + user.image_url);
        ResetImage();
        User = user;
        nameText.text = "" + user.name;
        UsernameText.text = "" + user.username;
        if (user.image_url=="" || User.image_url==null)
        {
            ResetImage();
        }
        else
        {
            Debug.Log("image_url not null" + user.image_url);
            GetImage(User.id, User.image_url);
        }
    }
    public void EnableFriendChat()
    {
        FriendProfileManager.Instance.MessageUser.id = User.id;
        FriendProfileManager.Instance.MessageUser.username = User.username;
        FriendProfileManager.Instance.MessageUser.name = User.name;
        FriendProfileManager.Instance.MessageUser.color = User.color;
        FriendProfileManager.Instance.MessageUser.image_url = User.image_url;
        FriendProfileManager.Instance.MessageUser.email = User.email;
        FriendProfileManager.Instance.MessageUser.chat_id = User.chat_id;

        FriendProfileManager.Instance.EnableFriendProfileChat(true);
        SceneManager.LoadScene("ChatScene");
    }
    public void ShowFriendProfile()
    {
        VideoPlaylistManager.instance.IsFriendProfileOpened = true ;
        ProfileManager.Instance.SetBoolOfLoggedInUser(false);
        FriendProfileManager.Instance.InitUserProfile(User);
    }
    public void RejectFriendRquest()
    {
        Debug.Log("Unfriend  clicked");
        LoadingManager.Instance.Loading.SetActive(true);
        StartCoroutine(ResponedToFriendRequest(User.id.ToString(), "canceled"));
        LoadingManager.Instance.Loading.SetActive(false);
        //FriendListCreator.instance.OpenConfirmUnfriendPanel(this);
    }
    public void AcceptFriendRquest()
    {
        Debug.Log("friend  clicked");
        LoadingManager.Instance.Loading.SetActive(true);
        StartCoroutine(ResponedToFriendRequest(User.id.ToString(), "accepted"));
        LoadingManager.Instance.Loading.SetActive(false);
        //FriendListCreator.instance.OpenConfirmUnfriendPanel(this);
    }
    public void ShowFriendRemovalPopup()
    {
        FriendsListManager.Instance.SetFriendForRemoval(gameObject);
    }
    public void RemoveFriend()
    {
        Debug.Log("friend  clicked");
        LoadingManager.Instance.Loading.SetActive(true);
        StartCoroutine(RemoveFriendCoroutine(User.id.ToString()));
        //FriendListCreator.instance.OpenConfirmUnfriendPanel(this);
    }
    public void OpenFriendInfoPanel()
    {
        Debug.Log("OpenFriendInfoPanel");
        //HunterProfile.instance.OpenProfile(user, UserImage.sprite);
    }

    public void ResetImage()
    {
        UserImage.sprite = DefaultSprite ;
    }
    IEnumerator RemoveFriendCoroutine(string ID)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", ID);
        string requestName = "api/v1/users/remove_friend";

        using (UnityWebRequest www = UnityWebRequest.Post(AuthManager.BASE_URL + requestName, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                ConsoleManager.instance.ShowMessage("Network Error!");
                LoadingManager.Instance.Loading.SetActive(false);
                Debug.Log(www.error);
            }
            else if (www.isHttpError)
            {
                ConsoleManager.instance.ShowMessage(www.error);
                LoadingManager.Instance.Loading.SetActive(false);
                Debug.Log(www.error);
            }
            else
            {
                GeneralResponce responce = JsonUtility.FromJson<GeneralResponce>(www.downloadHandler.text);
                if (responce.success)
                {
                    ConsoleManager.instance.ShowMessage("Friend removed successfully.");
                    Debug.Log("Removed ");
                    FriendsListManager.Instance.CloseFriendRemovalPopup();
                    FriendsListManager.Instance.RefereshFriendList();
                }
                else
                {
                    ConsoleManager.instance.ShowMessage("Friend not removed.");
                }
                LoadingManager.Instance.Loading.SetActive(false);
            }
        }
    }
    IEnumerator ResponedToFriendRequest(string user_id, string status)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", user_id);
        form.AddField("status", status);

        string requestName = "api/v1/follows/update_status";

        using (UnityWebRequest www = UnityWebRequest.Post(AuthManager.BASE_URL + requestName, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                ConsoleManager.instance.ShowMessage("Network Error!");
                LoadingManager.Instance.Loading.SetActive(false);
                Debug.Log(www.error);
            }
            else
            {
                FriendsListManager.Instance.RefereshFriendList();
                FriendsListManager.Instance.RefereshFriendRequestList();
            }
        }
    }
    public void GetImage(int id, string uri)
    {
        Debug.Log("id and URL " + id + ", " + uri);
        localURL = string.Format("{0}/{1}.jpg", Application.persistentDataPath, "" + id);

        if (File.Exists(localURL))
        {
            LoadLocalFile();
        }
        else
        {
            StartCoroutine(GetThumbnail(uri));
        }
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

    IEnumerator GetThumbnail(string uri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
        www.SetRequestHeader("Content-type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.responseCode);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            //UserImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            File.WriteAllBytes(localURL, texture.EncodeToPNG());
            Debug.Log("Image Downloaded and saved!");
            //LoadLocalFile();
            UserImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
