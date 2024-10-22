using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;
    public GameObject EditProfilePanel;
    public Image EditPanelProfilePicture;
    public Image ProfileImage;
    public InputField FullNameInputField;
    //public InputField AgeInputField;
    public Text ProfileName;
    //public Text ProfileAge;
    public Text PokogramsCount;
    public Text FriendsCount;
    public GameObject VideoThumbnailContent;
    public GameObject VideoThumbnailPrefab;

    public bool IsLoggedInUser = false;

    public GetUserVideosRoot Videos = new GetUserVideosRoot();
    public int FriendsPerPage = 20;

    private int PageNumber = 1;
    

    //private int age;
    //private DateTime DOB;
    //private TimeSpan Time_Span;
    public bool IsVideosThumbnailLoaded = false;
    //public Text EmailInputField;
    //public InputField AgeInputField;
    //public InputField usernameProfileInputField;
    //public Text nameCompleteProfileText;

    public static string FullName
    {
        set
        {
            PlayerPrefs.SetString("FullName", value);
        }
        get
        {
            return PlayerPrefs.GetString("FullName");
        }
    }
    public static string UserName
    {
        set
        {
            PlayerPrefs.SetString("UserName", value);
        }
        get
        {
            return PlayerPrefs.GetString("UserName");
        }
    }
    public static string UserEmail
    {
        set
        {
            PlayerPrefs.SetString("UserEmail", value);
        }
        get
        {
            return PlayerPrefs.GetString("UserEmail");
        }
    }
    public static string UserPassword
    {
        set
        {
            PlayerPrefs.SetString("UserPassword", value);
        }
        get
        {
            return PlayerPrefs.GetString("UserPassword");
        }
    }
    public static string UserImageUrl
    {
        set
        {
            PlayerPrefs.SetString("UserImageUrl", value);
        }
        get
        {
            return PlayerPrefs.GetString("UserImageUrl");
        }
    }
    public static string UserAge
    {
        set
        {
            PlayerPrefs.SetString("UserAge", value);
        }
        get
        {
            return PlayerPrefs.GetString("UserAge");
        }
    }
    public static int UserID
    {
        set
        {
            PlayerPrefs.SetInt("UserID", value);
        }
        get
        {
            return PlayerPrefs.GetInt("UserID");
        }
    }
    private string localURL;
    private const string MatchDOBpattern = "^[0-9]{1,2}\\-[0-9]{1,2}\\-[0-9]{4}$";
    // Start is called before the first frame update

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
        PageNumber = 1;
        LoadUserProfileData();
        IsVideosThumbnailLoaded = false;
        //if (PlayerPrefs.GetInt("BottomBarButtonsIndex") == 4)
        //{
        //    GetUserVideos();
        //}
    }
    public void GetUserVideos()
    {
        FriendsCount.text = "" + FriendsListManager.Instance.FriendsList.users.Count;
        VideoPlaylistManager.instance.IsFriendProfileOpened = false;
        if (!IsVideosThumbnailLoaded)
        {
            PageNumber = 1;
            Videos.user_videos = new List<UserVideo>();
            LoadingManager.Instance.Loading.SetActive(true);
            StartCoroutine(PostGetUserVideos(UserID.ToString()));
            //StartCoroutine(PostGetUserVideos("15"));
        }
        
    }
    public void SetBoolOfLoggedInUser(bool Value)
    {
        IsLoggedInUser = Value;
    }
    public void Logout()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }
    private void LoadUserProfileData()
    {
        Debug.Log("UserAge "+ UserAge);
        //DOB = DateTimeOffset.Parse(UserAge).DateTime;
        //Time_Span = DateTime.Today.Subtract(DOB);
        //ProfileAge.text = "" + ((int)((Time_Span.Days) / 365.242199));
        ProfileName.text = FullName;

        if (UserImageUrl != null || UserImageUrl != "")
        {
            LoadingManager.Instance.Loading.SetActive(true);
            GetUserImage(UserID, UserImageUrl);
            LoadingManager.Instance.Loading.SetActive(false);
        }
    }
    //private bool ValidateDOB(string DOB)
    //{
    //    if (AgeInputField != null)
    //        return Regex.IsMatch(DOB, MatchDOBpattern);
    //    else
    //        return false;
    //}
    public static Texture2D textureFromSprite(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
    public void GetUserImage(int id, string uri)
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
        ProfileImage.sprite = thumbnail;
    }
    public void DeleteUserAccount()
    {
        LoadingManager.Instance.Loading.SetActive(true);
        StartCoroutine(DeleteAccount());
    }
    IEnumerator DeleteAccount()
    {
        WWWForm form = new WWWForm();
        string requestName = "api/v1/users/remove_account";

        using (UnityWebRequest www = UnityWebRequest.Post(AuthManager.BASE_URL + requestName, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                ConsoleManager.instance.ShowMessage("Account deletion error!");
                LoadingManager.Instance.Loading.SetActive(false);
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("User Videos " + www.downloadHandler.text);
                GeneralResponce AccountDeletionResponce = JsonUtility.FromJson<GeneralResponce>(www.downloadHandler.text);
                if (AccountDeletionResponce.success)
                {
                    ConsoleManager.instance.ShowMessage("Account deleted successfully.");
                    StartCoroutine(WaitToClearData());
                }
                else
                {
                    ConsoleManager.instance.ShowMessage("Account deletion error!");
                    LoadingManager.Instance.Loading.SetActive(false);
                }
            }
        }
    }
    IEnumerator WaitToClearData()
    {
        yield return new WaitForSeconds(2);
        Logout();
    }
    public void UpdateUserInformation()
    {
        if (FullNameInputField.text == "")
        {
            ConsoleManager.instance.ShowMessage("Name can't be empty");
            return;
            //LoadingManager.Instance.Loading.SetActive(true);
            //StartCoroutine(UpdateData(UserID.ToString(), "Fareed", "greene", "Male", "2002-02-02"));
            //StartCoroutine(UpdateData(UserID.ToString(), FullNameInputField.text, AgeInputField.text));
        }
        //string[] date = AgeInputField.text.Split("-");
        //if (!ValidateDOB(date[1]+"-"+ date[0]+"-"+ date[2]))
        //{
        //    ConsoleManager.instance.ShowMessage("Date of Birth not Valid! use(-) between Dates");
        //    return;
        //}
        LoadingManager.Instance.Loading.SetActive(true);
        //StartCoroutine(UpdateData(UserID.ToString(), FullNameInputField.text, date[1] + "-" + date[0] + "-" + date[2]));
        StartCoroutine(UpdateData(UserID.ToString(), FullNameInputField.text, ""));
        //StartCoroutine(UpdateData(UserID.ToString(), FullNameInputField.text, AgeInputField.text));
        //StartCoroutine(UpdateData(UserID.ToString(), "fareed", "green", "male", "2002-02-02"));
    }
    public void LoadPreviousChanges()
    {
        EditPanelProfilePicture.sprite = ProfileImage.sprite;
        FullNameInputField.text = FullName;
        //AgeInputField.text = ""+ DOB.Month+"-"+ DOB.Day+ "-"+ DOB.Year;
    }
    private void UpdateUserData(UpdatedUserInfo.Root UpdatedUserInforesponce)
    {
        FullName = ""+UpdatedUserInforesponce.user.name;
        UserName = ""+ UpdatedUserInforesponce.user.username;
        UserID = UpdatedUserInforesponce.user.id;
        UserEmail = ""+ UpdatedUserInforesponce.user.email;
        UserAge = ""+ UpdatedUserInforesponce.user.birthday;
        UserImageUrl = ""+ UpdatedUserInforesponce.user.image_url;

        ProfileImage.sprite = GetImage.instance.ProfileImage.sprite ;// this is not working right now

        LoadUserProfileData();
    }
    IEnumerator PostGetUserVideos(string ID)
    {
        WWWForm form = new WWWForm();
        string requestName = "api/v1/users/"+ ID + "/get_users_videos?page="+PageNumber;

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
                Debug.Log("User Videos " + www.downloadHandler.text);
                GetUserVideosRoot VideosResponce = JsonUtility.FromJson<GetUserVideosRoot>(www.downloadHandler.text);
                for (int i = 0; i < VideosResponce.user_videos.Count; i++)
                {
                    Videos.user_videos.Add(VideosResponce.user_videos[i]);
                }
                if (VideosResponce.user_videos.Count == FriendsPerPage)
                {
                    PageNumber++;
                    StartCoroutine(PostGetUserVideos(ID));
                }
                else
                {
                    LoadingManager.Instance.Loading.SetActive(true);
                    //CreateVideosThumbnailList(VideosResponce);
                    CreateVideosThumbnailList(Videos);
                    IsVideosThumbnailLoaded = true;
                    //LoadingManager.Instance.Loading.SetActive(false);
                    Debug.Log("Deleted ");
                }
            }
        }
    }
    private void CreateVideosThumbnailList(GetUserVideosRoot VideosResponce)
    {
        ClearList(VideoThumbnailContent);

        //for (int i = (VideosResponce.user_videos.Count-1); i >=0; i--)
        //{
        //    GameObject refUser = Instantiate(VideoThumbnailPrefab, VideoThumbnailContent.transform);
        //    refUser.GetComponent<ThumbnailItem>().Init(VideosResponce.user_videos[i]);
        //}
        if (VideosResponce.user_videos.Count>0)
        {
            PokogramsCount.text = ""+VideosResponce.user_videos.Count;
            for (int i = (VideosResponce.user_videos.Count - 1); i >= 0; i--)
            {
                GameObject refUser = Instantiate(VideoThumbnailPrefab, VideoThumbnailContent.transform);
                refUser.GetComponent<ThumbnailItem>().Init(VideosResponce.user_videos[i]);
            }
            //foreach (UserVideo Video in VideosResponce.user_videos)
            //{
            //    GameObject refUser = Instantiate(VideoThumbnailPrefab, VideoThumbnailContent.transform);
            //    refUser.GetComponent<ThumbnailItem>().Init(Video);
            //}
        }
        else
        {
            ConsoleManager.instance.ShowMessage("No video found");
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
    IEnumerator UpdateData(string user_id, string name, string birthday)
    {
        WWWForm form = new WWWForm();
        //form.AddField("name", "fareed");
        //form.AddField("color", "green");
        //form.AddField("gender", "male");
        //form.AddField("birthday", "2002-02-02");
        //form.AddField("enable_counter", "false");

        form.AddField("name", name);
        //form.AddField("color", color);
        //form.AddField("gender", gender);
        form.AddField("birthday", birthday);
        form.AddField("enable_counter", "false");
        if (GetImage.instance.ImageSelected)
        {
            byte[] imageBytes = GetImage.instance.FinalImage.EncodeToPNG();
            form.AddBinaryData("image", imageBytes, "profile.png");
            
            localURL = string.Format("{0}/{1}.jpg", Application.persistentDataPath, "" + UserID);     // uncomment this line if you want to save image locally before uploading to save downloading data(MBs) + time
            File.WriteAllBytes(localURL, GetImage.instance.FinalImage.EncodeToPNG());
        }

        //string requestName = "api/v1/users/"+ user_id+"?";
        string requestName = "api/v1/users";

        using (UnityWebRequest www = UnityWebRequest.Post(AuthManager.BASE_URL + requestName, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                //ConsoleManager.instance.ShowMessage("Network Error!");
                LoadingManager.Instance.Loading.SetActive(false);
                Debug.Log(www.error);
                Debug.Log("AuthManager.Token "+ AuthManager.Token);
            }
            else
            {
                UpdatedUserInfo.Root UpdatedUserInforesponce = JsonUtility.FromJson<UpdatedUserInfo.Root>(www.downloadHandler.text);
                UpdateUserData(UpdatedUserInforesponce);
                EditProfilePanel.SetActive(false);
                LoadingManager.Instance.Loading.SetActive(false);
                Debug.Log("Uploaded "+ www.downloadHandler.text);
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
            //UserImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            File.WriteAllBytes(localURL, texture.EncodeToPNG());
            Debug.Log("Image Downloaded and saved!");
            //LoadLocalFile();
            ProfileImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
