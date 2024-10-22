using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FriendProfileManager : MonoBehaviour
{
    public static FriendProfileManager Instance;

    public GetUserVideosRoot VideosResponce = new GetUserVideosRoot();
    public GameObject UserProfilePanel;
    public GameObject VideoThumbnailContent;
    public GameObject VideoThumbnailPrefab;
    public Image ProfilePic;

    public Sprite DefaultSprite;
    public Text Name;
    //public Text Age;
    public Text PokogramsCount;

    public Chat.User MessageUser;
    public bool FriendProfileMessage = false;

    private int ThumbnailIndex = 0;
    public int FriendsPerPage = 20;
    private int UserVideosPageNumber = 0;
    private DateTime DOB;
    private TimeSpan Time_Span;
    private UpdatedUserInfo.Root CurrentVideoUser;

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
    void Start()
    {
        UserProfilePanel.SetActive(false);
    }
    public void EnableFriendProfileChat(bool value)
    {
        FriendProfileMessage = value;
    }
    public void InitUserProfile(FriendList.User User)
    {
        Debug.Log("InitUserProfile User.name " + User.name);
        Debug.Log("InitUserProfile User.id " + User.id);
        Debug.Log("InitUserProfile User.user_id " + User.id);

        MessageUser.id = User.id;
        MessageUser.username = User.username;
        MessageUser.name = User.name;
        MessageUser.color = User.color;
        MessageUser.image_url = User.image_url;
        MessageUser.email = User.email;
        MessageUser.chat_id = User.chat_id;

        ProfilePic.sprite = DefaultSprite;
        UserProfilePanel.SetActive(true);
        LoadingManager.Instance.Loading.SetActive(true);
        //StartCoroutine(PostGetVideoUserInfo("167"));
        //LoadingManager.Instance.Loading.SetActive(true);
        //StartCoroutine(PostGetUserVideos("167"));
        UserVideosPageNumber = 1;
        VideosResponce.user_videos = new List<UserVideo>();
        StartCoroutine(PostGetVideoUserInfo(User.id.ToString()));
        LoadingManager.Instance.Loading.SetActive(true);
        StartCoroutine(PostGetUserVideos(User.id.ToString()));


        //StartCoroutine(GetThumbnail(uri));
    }
    private void DisplayUserInfo(UpdatedUserInfo.Root CurrentVideoUser)
    {
        Name.text = CurrentVideoUser.user.name;
        //if (CurrentVideoUser.user.birthday != "")
        //{
        //    Age.text = "Age "; + AgeCalculator(CurrentVideoUser.user.birthday);
        //}
        //else
        //{
        //    Age.text = "Age not provided";
        //}
        if (CurrentVideoUser.user.image_url != "" || CurrentVideoUser.user.image_url != null)
        {
            StartCoroutine(GetThumbnail(CurrentVideoUser.user.image_url));
        }
        else
        {
            ProfilePic.sprite = DefaultSprite;
        }
    }
    //private int AgeCalculator(string Birthday)
    //{
    //    DOB = DateTimeOffset.Parse(Birthday).DateTime;
    //    Time_Span = DateTime.Today.Subtract(DOB);
    //    return ((int)((Time_Span.Days) / 365.242199));
    //}
    private void CreateVideosThumbnailList(GetUserVideosRoot VideosResponce)
    {
        ClearList(VideoThumbnailContent);
        //ThumbnailIndex = 0;
        PokogramsCount.text = "" + VideosResponce.user_videos.Count;
        //foreach (UserVideo Video in VideosResponce.user_videos)
        //{
        //    GameObject refUser = Instantiate(VideoThumbnailPrefab, VideoThumbnailContent.transform);
        //    refUser.GetComponent<ThumbnailItem>().Init(Video);
        //    refUser.GetComponent<ThumbnailItem>().ThumbnailItemIndex = ThumbnailIndex;
        //    ThumbnailIndex++;
        //}

        for (int i = (VideosResponce.user_videos.Count-1); i >=0; i--)
        {
            GameObject refUser = Instantiate(VideoThumbnailPrefab, VideoThumbnailContent.transform);
            refUser.GetComponent<ThumbnailItem>().Init(VideosResponce.user_videos[i]);
            refUser.GetComponent<ThumbnailItem>().ThumbnailItemIndex = i;
        }
    }
    public void ClearList(GameObject Content)
    {
        if (Content.transform.childCount > 0)
        {
            foreach (Transform Child in Content.transform)
            {
                Destroy(Child.gameObject);
            }
        }
    }
    IEnumerator PostGetUserVideos(string ID)
    {
        WWWForm form = new WWWForm();
        string requestName = "api/v1/users/" + ID + "/get_users_videos?page=" + UserVideosPageNumber;

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
                GetUserVideosRoot ResponcedVideos = JsonUtility.FromJson<GetUserVideosRoot>(www.downloadHandler.text);
                //VideosResponce = JsonUtility.FromJson<GetUserVideosRoot>(www.downloadHandler.text);
                for (int i = 0; i < ResponcedVideos.user_videos.Count; i++)
                {
                    VideosResponce.user_videos.Add(ResponcedVideos.user_videos[i]);
                }
                if (ResponcedVideos.user_videos.Count == FriendsPerPage)
                {
                    UserVideosPageNumber++;
                    StartCoroutine(PostGetUserVideos(ID));
                }
                else
                {
                    CreateVideosThumbnailList(VideosResponce);
                    LoadingManager.Instance.Loading.SetActive(false);
                    Debug.Log("Deleted ");
                }
                
            }
        }
    }
    IEnumerator PostGetVideoUserInfo(string UserID)
    {

        string requestName = "api/v1/users?user_id=" + UserID;
        string request = AuthManager.BASE_URL + requestName;
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
                    LoadingManager.Instance.Loading.SetActive(false);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    LoadingManager.Instance.Loading.SetActive(false);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    CurrentVideoUser = JsonUtility.FromJson<UpdatedUserInfo.Root>(webRequest.downloadHandler.text);
                    DisplayUserInfo(CurrentVideoUser);
                    LoadingManager.Instance.Loading.SetActive(false);
                    break;
            }
        }
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
            ProfilePic.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
