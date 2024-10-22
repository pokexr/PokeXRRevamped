using System;
using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ARVideoPrefabController : MonoBehaviour
{
    public bool DebugMode;
    public bool BillboardToUser;
    public bool StartOnAwake;

    public bool IsOpening;
    public bool IsLoaded;
    public bool IsPlaying;
    public bool IsPaused;

    public TextMeshPro UserHandle3DText;
    public TextMeshPro VideoDescription3DText;
    public TextMeshPro DebugText;

    public MediaPlayer VideoPlayer;

    public GameObject VideoPlayerStateIndicator;

    public Color DisabledColor = Color.white;
    public Color IdleColor = Color.white;
    public Color LoadingColor = Color.yellow;
    public Color PlayingColor = Color.green;
    public Color ReadyColor = Color.blue;
    public Color ErrorColor = Color.red;

    public Color CurrentColor = Color.white;

    // Unity-driven audio source sink for outgoing audio
    public AudioSource AudioSource;

    public UnityEvent<string> onLoadedUrlMedia;

    public string TestMediaURL;

    void OnEnable()
    {
        // Attempt to register with the scene controller
        var sceneController = FindFirstObjectByType<TMARSceneController>();
        if (sceneController != null)
        {
            sceneController.RegisterARVideoPrefabController(this);
        }

        // Hook media events
        VideoPlayer.Events.AddListener(HandleMediaPlayerEvent);

        if (StartOnAwake)
        {
            // Load the desired URL
            LoadMediaAtUrl(TestMediaURL);
            // Unmute the player
            VideoPlayer.Control.MuteAudio(false);
            // Play the video
            TogglePlayPause();
        }

        // Start with the player disabled color until we find otherwise
        SetPlayerStateColor(DisabledColor);
    }

    void OnDisable()
    {
        // Attempt to unregister with the scene controller
        var sceneController = FindFirstObjectByType<TMARSceneController>();
        if (sceneController != null)
        {
            sceneController.UnregisterARVideoPrefabController(this);
        }
    }

    void Start()
    {
        if (!DebugMode && DebugText != null)
        {
            DebugText.gameObject.SetActive(false);
        }

        onLoadedUrlMedia.AddListener(HandleLoadedUrlMedia);

        // Can't free rotate with gestures in AR, so disable the billboard script
        if (!BillboardToUser)
        {
            GetComponent<LookAtCameraBillboard>().enabled = false;
        }

        // VideoPlayer = GetComponent<MediaPlayer>();
        SetPlayerStateColor(DisabledColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugMode)
        {
            UpdateDebugView();
        }
    }

    void HandleLoadedUrlMedia(string url)
    {
        IsOpening = false;
        IsLoaded = true;
        Debug.Log($"Loaded media at URL: {url}");
    }

    // This method is called whenever there is an event from the MediaPlayer
    void HandleMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        // Debug.Log($"MediaPlayer Event: {Enum.GetName(typeof(MediaPlayerEvent.EventType), et)} ErrorCode: {errorCode}");
        switch (et)
        {
            case MediaPlayerEvent.EventType.Closing:
                UpdateDebugText("Closing...");
                SetPlayerStateColor(DisabledColor);
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                UpdateDebugText("Finished playing, idle.");
                SetPlayerStateColor(IdleColor);
                break;
            case MediaPlayerEvent.EventType.Started:
                UpdateDebugText("Video Playing");
                SetPlayerStateColor(PlayingColor);
                break;
            // case MediaPlayerEvent.EventType.FirstFrameReady:
            // case MediaPlayerEvent.EventType.MetaDataReady:
            case MediaPlayerEvent.EventType.ReadyToPlay:
                SetPlayerStateColor(ReadyColor);
                UpdateDebugText("Ready.");
                break;
            case MediaPlayerEvent.EventType.StartedBuffering:
                SetPlayerStateColor(LoadingColor);
                UpdateDebugText("Buffering...");
                break;
            case MediaPlayerEvent.EventType.FinishedBuffering:
                SetPlayerStateColor(PlayingColor);
                UpdateDebugText("Playing...");
                break;
            case MediaPlayerEvent.EventType.Stalled:
                SetPlayerStateColor(LoadingColor);
                UpdateDebugText("Stalled...");
                break;
            case MediaPlayerEvent.EventType.Unstalled:
                SetPlayerStateColor(PlayingColor);
                UpdateDebugText("Unstalled...");
                break;
            case MediaPlayerEvent.EventType.Error:
                SetPlayerStateColor(ErrorColor);
                UpdateDebugText("Error: " + errorCode.ToString());
                break;
        }
    }

    public void SetPlayerStateColor(Color color)
    {
        if (VideoPlayerStateIndicator != null)
        {
            CurrentColor = color;
            VideoPlayerStateIndicator.GetComponent<Renderer>().material.color = color;
        }
    }

    public void SetVolume(float volume)
    {
        VideoPlayer.AudioVolume = volume;
    }

    /* --- Debugging --- */

    [ContextMenu("Test Set Volume 0")]
    public void TestSetVolumeZero()
    {
        SetVolume(0f);
    }

    [ContextMenu("Test Set Volume Max")]
    public void TestSetVolumeMax()
    {
        SetVolume(1f);
    }

    [ContextMenu("Toggle Mute")]
    public void ToggleMute()
    {
        VideoPlayer.Control.MuteAudio(!VideoPlayer.Control.IsMuted());
    }

    [ContextMenu("Toggle Play Pause")]
    public void TogglePlayPause()
    {
        if (VideoPlayer.Control.IsPlaying())
        {
            Pause();
        }
        else
        {
            Play();
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (!VideoPlayer.Control.IsPlaying())
        {
            VideoPlayer.Control.Play();
            IsPlaying = true;
            IsPaused = false;
        }
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        if (!VideoPlayer.Control.IsPaused())
        {
            VideoPlayer.Control.Pause();
            IsPlaying = false;
            IsPaused = true;
        }
    }

    [ContextMenu("Load test URL")]
    public void LoadTestURL()
    {
        LoadMediaAtUrl(TestMediaURL);
    }

    public void LoadMediaAtUrl(string url)
    {
        IsLoaded = false;
        IsOpening = VideoPlayer.OpenMedia(new MediaPath(url, MediaPathType.AbsolutePathOrURL), autoPlay: false);
        onLoadedUrlMedia.Invoke(url);
    }

    void UpdateDebugView()
    {
        UpdateDebugText($"xpos {transform.position.x}\nypos {transform.position.y}\nzpos {transform.position.z}\nscale {transform.localScale.x}");
    }

    void UpdateDebugText(string text)
    {
        if (DebugText != null)
        {
            DebugText.text = text;
        }
    }

    public void SetTextValues(string userHandle = "", string videoDescription = "")
    {
        if (UserHandle3DText != null && VideoDescription3DText != null)
        {
            UserHandle3DText.text = userHandle;
            VideoDescription3DText.text = videoDescription;
        }
    }

    public void SetMute(bool mute)
    {
        VideoPlayer.Control.MuteAudio(mute);
    }
}
