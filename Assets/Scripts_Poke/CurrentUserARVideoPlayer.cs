using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class CurrentUserARVideoPlayer : MonoBehaviour
{
    public static CurrentUserARVideoPlayer instance;
    public MediaPlayer[] mediaPlayers;

    private MediaPlayer activeMediaPlayer;
    private MediaPlayer loadingMediaPlayer;

    public GameObject LoadingPanel;

    private ApplyToMesh applyToMesh;

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

  

    // Start is called before the first frame update
    void Start()
    {
        activeMediaPlayer = mediaPlayers[0];
        loadingMediaPlayer = mediaPlayers[1];
        LoadVideo(UserVideoItem.selectedUrl);
    }

    public void LoadVideo(string videoURL)
    {
        Debug.Log(videoURL);
        try
        {
            activeMediaPlayer.OpenMedia(new MediaPath(videoURL, MediaPathType.AbsolutePathOrURL));
            activeMediaPlayer.Events.AddListener(HandleEvent);
        }
        catch
        {

        }

        Debug.Log("Video Loading");
    }

    public void LoadVideo(string videoURL,bool play)
    {
        Debug.Log(videoURL);
        try
        {
            loadingMediaPlayer.OpenMedia(new MediaPath(videoURL, MediaPathType.AbsolutePathOrURL));
            applyToMesh.Player = loadingMediaPlayer;
        }
        catch
        {

        }

        Debug.Log("Video Loading");
    }

    // This method is called whenever there is an event from the MediaPlayer
    void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        Debug.Log("MediaPlayer " + mp.name + " generated event: " + eventType.ToString());
        if (eventType == MediaPlayerEvent.EventType.Error)
        {
            Debug.LogError("Error: " + code);
        }
        if (eventType == MediaPlayerEvent.EventType.FirstFrameReady)
        {
            LoadingPanel.SetActive(false);
            mp.Pause();
        }
    }

    public void PlayVideo(GameObject videoObject)
    {
        applyToMesh = videoObject.GetComponentInChildren<ApplyToMesh>();
        applyToMesh.Player = activeMediaPlayer;
        activeMediaPlayer.Play();
    }

    public void PlayVideo()
    {
        applyToMesh.Player = activeMediaPlayer;
        activeMediaPlayer.Play();
    }


}
