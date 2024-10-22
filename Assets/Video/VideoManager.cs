using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using Logger = UnityEngine.XR.ARFoundation.Samples.Logger;

public class VideoManager : MonoBehaviour
{
    public MediaPlayer mediaPlayer;

    private bool isPlayed = false;

    void Start()
    {
        // The method HandleEvent will be called whenever an event is triggered
        isPlayed = false;
        mediaPlayer = GetComponent<MediaPlayer>();
    }

    public void LoadVideo(string videoURL)
    {
        Debug.Log("VideoManager:LoadVideo: videoURL: " + videoURL);
        try
        {
            mediaPlayer.OpenMedia(new MediaPath(videoURL, MediaPathType.AbsolutePathOrURL),autoPlay:false);
            mediaPlayer.Events.AddListener(HandleEvent);
        }
        catch
        {

        }
        
        Debug.Log("Video Loading");
    }

    // This method is called whenever there is an event from the MediaPlayer
    void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        Logger.Log("MediaPlayer " + mp.name + " generated event: " + eventType.ToString());
        if (eventType == MediaPlayerEvent.EventType.Error)
        {
            Logger.Log("Error: " + code);
            ConsoleManager.instance.ShowMessage("Video loading error");
        }
        if (eventType == MediaPlayerEvent.EventType.FirstFrameReady)
        {
            mediaPlayer.Pause();
        }
    }

    /// <summary>
    /// Get video info from VideoItem on this object, feed it into Prefab controller to update UI text, play the vid.
    /// </summary>
    public void PlayVideo()
    {
        isPlayed = true;
        mediaPlayer.Play();
    }
    public void PauseVideo()
    {
        mediaPlayer.Pause();
    }
}
