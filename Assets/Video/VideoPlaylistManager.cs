using System;
using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Logger = UnityEngine.XR.ARFoundation.Samples.Logger;

public class VideoPlaylistManager : MonoBehaviour
{
    public static VideoPlaylistManager instance;
    public GameObject ProfilePanel;
    public GameObject MyProfilePanel;
    public GameObject FriendListPanel;
    public GameObject FriendProfilePanel;
    public GameObject BottomBar;
    public GameObject CloseUserVideoBtn;
    public GameObject VideoDeletionPopup;
    public GameObject VideoDeletionBtn;
    public Button PlayPauseBtn;
    public Button PlayPauseUserBtn;
    public Sprite PlaySprite;
    public Sprite PauseSprite;
    public ProfileManager profileManager;

    [Header("Video List")]

    public bool FilterVideoList;

    [SerializeField]
    public VideosList videosList;
    private int currentIndex;
    private int currentVideoIndex;
    private int UserVideoIndex;
    public int CurrentVideoListIndex;
    private int MediaPlayerIndex;
    private int PlayingVideoIndex;
    private int ProfileUserVideoID;

    public int CurrentUserID;
    public int CurrentVideoID;
    public bool IsFriendProfileOpened = false;
    public bool PauseOtherVideosBool = false;
    public bool OtherUserVideosBool = true;
    private bool PlayPauseBool = false;
    private bool PlayPauseUserBool = false;

    public Video CurrentVideoItem;
    public TextMeshProUGUI UsernameText;
    public TextMeshProUGUI DescriptionText;

    public int numPlayers = 1;

    // public GameObject mediaPlayerDisplay;
    public GameObject[] mediaPlayers;

    public int[] videoTestPool;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (FilterVideoList)
        {
            videoTestPool = new int[] {
                55,
                74,
                6,
                84,
                61,
                78,
                85,
                5,
                108,
                110,
                13,
                7,

                65,
                111,
                106,
                81,
                8,
                9,
                97,
                66,
                67,
                56,
                114,
                60,
                31,
                3,
                77,
                26,
                98
            };
        }

        currentIndex = 0;
        MediaPlayerIndex = 0;
        PlayingVideoIndex = 0;
        currentVideoIndex = 0;
        PlayPauseBool = false;
        OtherUserVideosBool = true;
        PauseOtherVideosBool = false;
        PlayPauseUserBool = true;
        CurrentVideoListIndex = 0;
    }

    // TODO: Do your best to obsolete this bullshit, it runs from the VideoAPIManager for no reason.
    // Likely a poor attempt at multi video support.
    public void StartList(VideosList videosList)
    {
        Logger.Log("VideoPlaylistManager: StartList: videosList.videos.Count: " + videosList.videos.Count);

        this.videosList = videosList;

        /*
        int totalLoadingPlayers = numPlayers;
        if (this.videosList.videos.Count < numPlayers)
        {
            totalLoadingPlayers = this.videosList.videos.Count;
        }   
        
        for (int i = 0; i < totalLoadingPlayers; i++)
        {
            Debug.Log("VideoPlaylistManager: StartList: LoadVideo: name " + this.videosList.videos[i].name + " (id " + this.videosList.videos[i].id + ") URL: " + this.videosList.videos[i].result_video_url);
            mediaPlayers[i].GetComponent<VideoManager>().LoadVideo(this.videosList.videos[i].result_video_url);
            mediaPlayers[i].GetComponent<MediaPlayer>().AutoOpen = false;
            mediaPlayers[i].GetComponent<MediaPlayer>().AutoStart = false;
            mediaPlayers[i].GetComponent<VideoItem>().VideoUserItem = videosList.videos[i];
        }
        currentIndex = 0;
        currentVideoIndex = 0;
        */
    }

    public void PlayList(GameObject mpDisplay)
    {
        PlayVideo();
    }
    public void PlayVideo()
    {
        // Choose a random index for the videoList array
        int poolIndex = -1;

        if (FilterVideoList)
            poolIndex = videoTestPool[UnityEngine.Random.Range(0, videoTestPool.Length)];
        else
            poolIndex = UnityEngine.Random.Range(0, videosList.videos.Count);

        PlayVideo(poolIndex);
    }

    public void PlayVideo(int videoIndex)
    {
        var videoItem = videosList.videos[videoIndex];
        string videoName = videoItem.original_video_name;
        string videoUrl = videoItem.result_video_url;

        Logger.Log("VideoPlaylistManager: PlayVideo: videoIndex: " + videoIndex + " URL: " + videoUrl);

        PlayVideoAtUrl(videoUrl);
        UpdateActiveMediaUI(videoItem);
    }

    public void UpdateCurrentMediaUI()
    {
        var videoItem = videosList.videos[CurrentVideoListIndex];
        UpdateActiveMediaUI(videoItem);
    }

    public void UpdateActiveMediaUI(Video videoItem)
    {
        CurrentVideoItem = videoItem;
        var userId = CurrentVideoItem.user_id;
        var userHandle = CurrentVideoItem.user_name;
        var videoDescription = CurrentVideoItem.original_video_name;
        Logger.Log("VideoPlaylistManager: UpdateActiveMediaUI: userId: " + userId + ", userHandle: " + userHandle + ", videoDescription: " + videoDescription);

        UsernameText.text = "@" + userHandle;
        DescriptionText.text = videoDescription;
    }

    public void PlayVideoAtUrl(string url)
    {
        var player = mediaPlayers[0].GetComponent<MediaPlayer>();
        bool isOpening = player.OpenMedia(new MediaPath(url, MediaPathType.AbsolutePathOrURL), autoPlay: true);

        // Play via the current AvPro player
        player.Play();
    }
    public void LoadUserVideo(string Url, int VideoID)
    {
        Debug.LogFormat("VideoPlaylistManager: LoadUserVideo: Url: {0}, VideoID: {1}", Url, VideoID);
        ProfileUserVideoID = VideoID;
        if (Url != "" || Url != null)
        {
            //PauseAllMediaPlayers();
            ProfilePanel.SetActive(false);
            BottomBar.SetActive(false);
            CloseUserVideoBtn.SetActive(true);
            PlayPauseUserBtn.gameObject.SetActive(true);
            PauseAllMediaPlayers();
            if (currentVideoIndex > 0)
            {
                UserVideoIndex = currentVideoIndex - 1;
                mediaPlayers[currentVideoIndex - 1].GetComponent<VideoManager>().LoadVideo(Url);
                mediaPlayers[currentVideoIndex - 1].GetComponent<VideoManager>().PlayVideo();
            }
            else
            {
                UserVideoIndex = mediaPlayers.Length - 1;
                mediaPlayers[mediaPlayers.Length - 1].GetComponent<VideoManager>().LoadVideo(Url);
                mediaPlayers[mediaPlayers.Length - 1].GetComponent<VideoManager>().PlayVideo();
            }
        }
        else
        {
            ConsoleManager.instance.ShowMessage("Video Not Found");
        }
        if (profileManager.IsLoggedInUser)
        {
            VideoDeletionBtn.SetActive(true);
        }
        else
        {
            VideoDeletionBtn.SetActive(false);
        }
    }
    public void DeleteUserVideo()
    {
        Debug.Log("Video ID " + ProfileUserVideoID);
        LoadingManager.Instance.Loading.SetActive(true);
        //StartCoroutine(RemoveideoCoroutine(30));
        StartCoroutine(RemoveideoCoroutine(ProfileUserVideoID));
    }
    public void PlayPauseUserVideo()
    {
        PauseAllMediaPlayers();
        if (!PlayPauseUserBool)
        {
            PlayPauseUserBool = true;
            PlayPauseUserBtn.image.sprite = PauseSprite;
            mediaPlayers[UserVideoIndex].GetComponent<VideoManager>().PlayVideo();
        }
        else
        {
            PlayPauseUserBool = false;
            PlayPauseUserBtn.image.sprite = PlaySprite;
            mediaPlayers[UserVideoIndex].GetComponent<VideoManager>().PauseVideo();
        }
    }
    public void PlayPauseVideo()
    {
        if (!PlayPauseBool)
        {
            //PlayPauseBool = true;
            //PlayPauseBtn.image.sprite = PauseSprite;
            PlayPausedVideo();
        }
        else
        {
            PlayPauseBool = false;
            PlayPauseBtn.image.sprite = PlaySprite;
            PauseAllMediaPlayers();
            PausedVideo();
        }
    }
    public void SetBoolOfVideoPlayer(bool value)
    {
        OtherUserVideosBool = value;
    }
    public void CloseUserVideoScreen()
    {
        PauseAllMediaPlayers();
        if (IsFriendProfileOpened)
        {
            FriendProfilePanel.SetActive(true);
        }
        else if (OtherUserVideosBool)
        {
            ProfilePanel.SetActive(true);
        }
        else
        {
            //FriendListPanel.SetActive(true);
            MyProfilePanel.SetActive(true);
        }
        BottomBar.SetActive(true);
        if (IsFriendProfileOpened)
        {
            BottomBar.SetActive(false);
        }
        PlayPauseUserBtn.gameObject.SetActive(false);
        CloseUserVideoBtn.SetActive(false);
        if (currentVideoIndex > 0)
        {
            mediaPlayers[currentVideoIndex - 1].GetComponent<VideoManager>().LoadVideo(videosList.videos[CurrentVideoListIndex - 1].result_video_url);
            mediaPlayers[currentVideoIndex - 1].GetComponent<VideoManager>().PlayVideo();
        }
        else
        {
            mediaPlayers[mediaPlayers.Length - 1].GetComponent<VideoManager>().LoadVideo(videosList.videos[CurrentVideoListIndex - 1].result_video_url);
            mediaPlayers[mediaPlayers.Length - 1].GetComponent<VideoManager>().PlayVideo();
        }
    }
    public void PauseAllMediaPlayers()
    {
        foreach (GameObject mp in mediaPlayers)
        {
            mp.GetComponent<MediaPlayer>().Pause();
        }
    }
    private void LoadNextVideo()
    {
        mediaPlayers[MediaPlayerIndex].GetComponent<VideoManager>().LoadVideo(videosList.videos[currentIndex].result_video_url);
        //mediaPlayers[MediaPlayerIndex].GetComponent<MediaPlayer>().AutoOpen = false; 
        //mediaPlayers[MediaPlayerIndex].GetComponent<MediaPlayer>().AutoStart = false; 
        MediaPlayerIndex++;
        if (MediaPlayerIndex == 5)
        {
            MediaPlayerIndex = 0;
        }
        //PauseAllMediaPlayers();
        //mediaPlayers[PlayingVideoIndex].GetComponent<VideoManager>().PlayVideo();
        Debug.Log("MediaPlayerIndex after" + MediaPlayerIndex);
        //mediaPlayers[VideoIndex].GetComponent<VideoManager>().PlayVideo();
        //if (VideoIndex>0)
        //{
        //    mediaPlayers[MediaPlayerIndex].GetComponent<VideoManager>().LoadVideo(videosList.videos[currentIndex].result_video_url);
        //}
        //else
        //{
        //    mediaPlayers[4].GetComponent<VideoManager>().LoadVideo(videosList.videos[currentIndex].result_video_url);
        //}
        //Debug.Log("Stopped");
        //StartCoroutine(PauseMediaPlayers());
        //mediaPlayers[CurrentPlayingVideoIndex].GetComponent<VideoManager>().PlayVideo();
    }
    public void ShowUserProfile()
    {
        PauseAllMediaPlayers();
        IsFriendProfileOpened = false;
        OtherUserProfileManager.Instance.InitUserProfile(videosList.videos[CurrentVideoListIndex - 1]);
    }
    public void PausedVideo()
    {
        if (currentVideoIndex > 0)
        {
            mediaPlayers[currentVideoIndex - 1].GetComponent<VideoManager>().PauseVideo();
        }
        else
        {
            mediaPlayers[mediaPlayers.Length - 1].GetComponent<VideoManager>().PauseVideo();
        }
    }
    public void PlayPausedVideo()
    {
        PauseAllMediaPlayers();
        if (currentVideoIndex > 0)
        {
            mediaPlayers[currentVideoIndex - 1].GetComponent<VideoManager>().PlayVideo();
        }
        else
        {
            mediaPlayers[mediaPlayers.Length - 1].GetComponent<VideoManager>().PlayVideo();
        }
        PlayPauseBool = true;
        PlayPauseBtn.image.sprite = PauseSprite;
    }
    IEnumerator RemoveideoCoroutine(int ID)
    {
        string requestName = "api/v1/videos/" + ID;
        // Assuming you have authentication token or user credentials
        string authToken = AuthManager.Token; // Replace with actual authentication token

        // UnityWebRequest to send a DELETE request to the API endpoint
        using (UnityWebRequest request = UnityWebRequest.Delete(AuthManager.BASE_URL + requestName))
        {
            // Set the authorization header
            request.SetRequestHeader("Authorization", "Bearer " + authToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Account deletion request successful");

                ConsoleManager.instance.ShowMessage("Video successfully deleted.");
                Debug.Log("Deleted ");
                VideoDeletionPopup.SetActive(false);
                CloseUserVideoScreen();
                profileManager.IsVideosThumbnailLoaded = false;
                profileManager.GetUserVideos();

                //You may want to handle the response from the server here
                //GeneralResponce responce = JsonUtility.FromJson<GeneralResponce>(request.downloadHandler.text);
                //if (responce.success)
                //{
                //    ConsoleManager.instance.ShowMessage("Video successfully deleted.");
                //    Debug.Log("Deleted ");
                //    VideoDeletionPopup.SetActive(false);
                //    //CloseUserVideoScreen();
                //    profileManager.IsVideosThumbnailLoaded = false;
                //    profileManager.GetUserVideos();
                //}
                //else
                //{
                //    ConsoleManager.instance.ShowMessage("Video deletion error.");
                //}
                LoadingManager.Instance.Loading.SetActive(false);
            }
            else
            {
                LoadingManager.Instance.Loading.SetActive(false);
                ConsoleManager.instance.ShowMessage("Video deletion error.");
                Debug.LogError("Account deletion request failed: " + request.error);
                // Handle the error or display a message to the user
            }
            LoadingManager.Instance.Loading.SetActive(false);
        }

        //using (UnityWebRequest www = UnityWebRequest.Delete(AuthManager.BASE_URL + requestName))
        //{
        //    www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
        //    yield return www.SendWebRequest();

        //    if (www.isNetworkError)
        //    {
        //        ConsoleManager.instance.ShowMessage("Network Error!");
        //        LoadingManager.Instance.Loading.SetActive(false);
        //        Debug.Log(www.error);
        //    }
        //    else if (www.isHttpError)
        //    {
        //        ConsoleManager.instance.ShowMessage(www.error);
        //        LoadingManager.Instance.Loading.SetActive(false);
        //        Debug.Log(www.error);
        //    }
        //    else
        //    {
        //        GeneralResponce responce = JsonUtility.FromJson<GeneralResponce>(www.downloadHandler.text);
        //        if (responce.success)
        //        {
        //            ConsoleManager.instance.ShowMessage("Video successfully deleted.");
        //            Debug.Log("Deleted ");
        //            VideoDeletionPopup.SetActive(false);
        //            CloseUserVideoScreen();
        //            profileManager.IsVideosThumbnailLoaded = false;
        //            profileManager.GetUserVideos();
        //        }
        //        else
        //        {
        //            ConsoleManager.instance.ShowMessage("Video deletion error.");
        //        }
        //        LoadingManager.Instance.Loading.SetActive(false);
        //    }
        //}
    }
}
