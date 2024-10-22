using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OtherUserProfileManager : MonoBehaviour
{
    public static OtherUserProfileManager Instance;
    
    public GetUserVideosRoot VideosResponce = new GetUserVideosRoot();
    public GameObject UserProfilePanel;
    public GameObject VideoThumbnailContent;
    public GameObject VideoThumbnailPrefab;
    public Image ProfilePic;
    public Button FollowFriendBtn;
    public Sprite DefaultSprite;
    public Text Name;
    //public Text Age;
    public Text PokogramsCount;

    private int ThumbnailIndex = 0;
    //private DateTime DOB;
    //private TimeSpan Time_Span;
    private UpdatedUserInfo.Root CurrentVideoUser;

    private int PageNumber = 1;
    public int FriendsPerPage = 20;

    private void Awake()
    {
        if (Instance!=null)
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
        PageNumber = 1;
        FollowFriendBtn.interactable = true;
        UserProfilePanel.SetActive(false);
    }

    public void InitUserProfile(Video User)
    {
        Debug.Log("InitUserProfile User.name " + User.name);
        Debug.Log("InitUserProfile User.id " + User.id);
        Debug.Log("InitUserProfile User.user_id " + User.user_id);

        ProfilePic.sprite = DefaultSprite;
        UserProfilePanel.SetActive(true);
        LoadingManager.Instance.Loading.SetActive(true);
        //StartCoroutine(PostGetVideoUserInfo("167"));
        //LoadingManager.Instance.Loading.SetActive(true);
        //StartCoroutine(PostGetUserVideos("167"));
        //ChecksUserFollowingStatus(User.id);
        StartCoroutine(PostGetVideoUserInfo(User.user_id.ToString()));
        LoadingManager.Instance.Loading.SetActive(true);
        PageNumber = 1;
        VideosResponce.user_videos.Clear();
        VideosResponce.user_videos = new List<UserVideo>();
        StartCoroutine(PostGetUserVideos(User.user_id.ToString()));

        
        //StartCoroutine(GetThumbnail(uri));
    }
    private void ChecksUserFollowingStatus(int UserID)
    {
        bool FriendFound = false;
        foreach (var item in FriendsListManager.Instance.FriendsList.users)
        {
            Debug.Log("item.id "+ item.id);
            if (UserID == item.id)
            {
                Debug.Log("FriendFound " + FriendFound);
                FriendFound = true;
                break;
            }
        }
        if (FriendFound)
        {
            Debug.Log("FollowFriendBtn.interactable false");
            FollowFriendBtn.interactable = false;
        }
        else if (UserID == ProfileManager.UserID)
        {
            Debug.Log("FollowFriendBtn.interactable false");
            FollowFriendBtn.interactable = false;
        }
        else
        {
            Debug.Log("FollowFriendBtn.interactable true");
            FollowFriendBtn.interactable = true;
        }
    }
    public void SendFriendRequest()
    {
        StartCoroutine(PostSendFollowrequest(CurrentVideoUser.user.id, "Request sent", "Request not sent", "Request already sent"));
    }
    private void DisplayUserInfo(UpdatedUserInfo.Root CurrentVideoUser)
    {
        Name.text = CurrentVideoUser.user.name;
        //if (CurrentVideoUser.user.birthday!="")
        //{
        //    Age.text = "Age " + AgeCalculator(CurrentVideoUser.user.birthday);
        //}
        //else
        //{
        //    Age.text = "Age not provided";
        //}
        if (CurrentVideoUser.user.image_url!="" || CurrentVideoUser.user.image_url != null)
        {
            StartCoroutine(GetThumbnail(CurrentVideoUser.user.image_url));
        }
        else
        {
            ProfilePic.sprite = DefaultSprite;
        }
        ChecksUserFollowingStatus(CurrentVideoUser.user.id);
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
        if (VideosResponce.user_videos.Count>0)
        {
            ThumbnailIndex = 0;
            PokogramsCount.text = "" + VideosResponce.user_videos.Count;
            //foreach (UserVideo Video in VideosResponce.user_videos)
            //{
            //    GameObject refUser = Instantiate(VideoThumbnailPrefab, VideoThumbnailContent.transform);
            //    refUser.GetComponent<ThumbnailItem>().Init(Video);
            //    refUser.GetComponent<ThumbnailItem>().ThumbnailItemIndex = ThumbnailIndex;
            //    ThumbnailIndex++;
            //}
            for (int i = (VideosResponce.user_videos.Count - 1); i >= 0; i--)
            {
                GameObject refUser = Instantiate(VideoThumbnailPrefab, VideoThumbnailContent.transform);
                refUser.GetComponent<ThumbnailItem>().Init(VideosResponce.user_videos[i]);
                refUser.GetComponent<ThumbnailItem>().ThumbnailItemIndex = i;
            }

        }
        else
        {
            ConsoleManager.instance.ShowMessage("No video Found");
        }
        LoadingManager.Instance.Loading.SetActive(false);
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
        string requestName = "api/v1/users/" + ID + "/get_users_videos?page="+ PageNumber;

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
                GetUserVideosRoot UserVideosResponce = JsonUtility.FromJson<GetUserVideosRoot>(www.downloadHandler.text);
                for (int i = 0; i < UserVideosResponce.user_videos.Count; i++)
                {
                    VideosResponce.user_videos.Add(UserVideosResponce.user_videos[i]);
                }
                if (UserVideosResponce.user_videos.Count == FriendsPerPage)
                {
                    PageNumber++;
                    StartCoroutine(PostGetUserVideos(ID));
                }
                else
                {
                    CreateVideosThumbnailList(VideosResponce);
                    //LoadingManager.Instance.Loading.SetActive(false);
                    Debug.Log("Deleted ");
                }
                
            }
        }
    }
    IEnumerator PostGetVideoUserInfo(string UserID)
    {

        string requestName = "api/v1/users?user_id="+ UserID;
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
    public IEnumerator PostSendFollowrequest(int ID,string Message, string FailureMessage,string Duplication)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", ID);

        string requestName = "api/v1/follows";

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
                GeneralResponceRoot Responce = JsonUtility.FromJson<GeneralResponceRoot>(www.downloadHandler.text);
                if (Responce.success)
                {
                    FollowFriendBtn.interactable = false;
                    ConsoleManager.instance.ShowMessage(Message);
                }
                else
                {
                    if (Responce.msg== "Follower has already been taken")
                    {
                        FollowFriendBtn.interactable = false;
                        ConsoleManager.instance.ShowMessage(Duplication);
                    }
                    else
                    {
                        ConsoleManager.instance.ShowMessage(FailureMessage);
                    }                }
                LoadingManager.Instance.Loading.SetActive(false);
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
