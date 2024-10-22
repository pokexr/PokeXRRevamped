using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Logger = UnityEngine.XR.ARFoundation.Samples.Logger;

public class ARAnchorMediaManager : MonoBehaviour
{
    public enum PlayerMode
    {
        RenderTexture,
        ARAnchorAuto,
        ARAnchorPlacement,
        UGUI,
    }

    public bool PlayOnStart = true;

    public bool ShouldLoop = true;

    [SerializeField]
    public PlayerMode PlayerModeSelection;

    [Header("System State")]

    public bool HasMedia = false;
    public bool IsOpening = false;
    public bool IsPlaying = false;
    public bool IsPaused = false;
    public bool IsClosing = false;
    public bool IsVisible
    {
        get
        {
            return mediaPlayer.gameObject.activeSelf;
        }
    }

    public GameObject CurtainPanel;

    public MediaPlayer mediaPlayer;

    public MediaPlayer UGUIMediaPlayer;
    public MediaPlayer ARAnchorMediaPlayer;
    public MediaPlayer RenderTextureMediaPlayer;
    public DisplayUGUI DisplayUGUITarget;
    public DisplayUGUI DisplayARAnchorTarget;
    public RawImage RenderTextureTarget;

    [Tooltip("The currently active media AR anchor, or null.")]
    public GameObject ARAnchor3DTarget;

    public List<ARVideoPrefabController> CurrentARTargetsList = new();

    public TextMeshProUGUI DebugText;

    string sourcePath;

    void Start()
    {
        Log("ARAnchorMediaManager: Initializing...");

        // Enable proper configuration for PlayerMode selected
        InitializePlayerView();

        GetComponent<ResolveToRenderTexture>().MediaPlayer = mediaPlayer;

        UpdateMediaPath(mediaPlayer.MediaPath.PathType, mediaPlayer.MediaPath.Path);
        
        Log("ARAnchorMediaManager: Player Type: " + Enum.GetName(typeof(PlayerMode), PlayerModeSelection) + ".");
        Log("ARAnchorMediaManager: Initialized.");

        // If we don't have a media player, something went wrong. Stop
        if (mediaPlayer == null)
        {
            Debug.LogError("ARAnchorMediaManager: No MediaPlayer determined for ARAnchorMediaManager, check prefab settings!");
            return;
        }

        // Play on start
        if (PlayOnStart)
        {
            IsOpening = Play();
        }
    }

    void Update()
    {

    }
    
    void InitializePlayerView()
    {
        // Put curtain up
        ShowCurtain();

        // Disable all player and texture GOs
        RenderTextureTarget.gameObject.SetActive(false);
        DisplayUGUITarget.gameObject.SetActive(false);
        if (ARAnchor3DTarget != null)
            ARAnchor3DTarget.gameObject.SetActive(false);
        
        RenderTextureMediaPlayer.gameObject.SetActive(false);
        UGUIMediaPlayer.gameObject.SetActive(false);
        ARAnchorMediaPlayer.gameObject.SetActive(false);

        Log("ARAnchorMediaManager: Initializing media view for PlayerMode: " + Enum.GetName(typeof(PlayerMode), PlayerModeSelection) + ".");

        // Conditionally re-enable what we need
        switch (PlayerModeSelection)
        {
            case PlayerMode.UGUI:
                DisplayUGUITarget.gameObject.SetActive(true);
                DisplayUGUITarget.GetComponent<DisplayUGUI>().Player = UGUIMediaPlayer;
                SetMediaPlayer(UGUIMediaPlayer);
                break;
            case PlayerMode.RenderTexture:
                RenderTextureTarget.gameObject.SetActive(true);
                SetMediaPlayer(RenderTextureMediaPlayer);
                break;
            case PlayerMode.ARAnchorAuto:
            case PlayerMode.ARAnchorPlacement:
                SetMediaPlayer(ARAnchorMediaPlayer);
                break;
            default:
                Debug.LogError("ARAnchorMediaManager: PlayerMode not recognized: " + Enum.GetName(typeof(PlayerMode), PlayerModeSelection) + ".");
                break;
        }

        mediaPlayer.gameObject.SetActive(true);
        GetComponent<ResolveToRenderTexture>().MediaPlayer = mediaPlayer;

        Log("ARAnchorMediaManager: Initialized media view for PlayerMode: " + Enum.GetName(typeof(PlayerMode), PlayerModeSelection) + ".");
        HideCurtain();
    }

    void Show()
    {
        mediaPlayer.gameObject.SetActive(true);
    }

    void Hide()
    {
        mediaPlayer.gameObject.SetActive(false);
    }

    void ShowCurtain()
    {
        CurtainPanel.gameObject.SetActive(true);
    }

    void HideCurtain()
    {
        CurtainPanel.gameObject.SetActive(false);
    }

    [ContextMenu("ToggleVisible")]
    void ToggleVisible()
    {
        mediaPlayer.gameObject.SetActive(!mediaPlayer.gameObject.activeSelf);
    }

    [ContextMenu("Play")]
    public bool Play()
    {
        Log("ARAnchorMediaManager: Play");

        if (string.IsNullOrEmpty(mediaPlayer.MediaPath.Path))
        {
            Log("ARAnchorMediaManager: No media path set for MediaPlayer, manager: " + gameObject.name);
        }

        if (mediaPlayer != null)
        {
            mediaPlayer.Loop = ShouldLoop;
            return mediaPlayer.OpenMedia(mediaPlayer.MediaPath, autoPlay: true);
        }

        return false;
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        Log("ARAnchorMediaManager: Stop");

        if (mediaPlayer != null)
        {
            mediaPlayer.Stop();
        }
    }

    public VideoPlaylistManager videoPlaylistManager;

    void SetMediaPlayer(MediaPlayer newPlayer)
    {
        Log("ARAnchorMediaManager: SetMediaPlayer: " + newPlayer.name);
        mediaPlayer = newPlayer;
        GetComponent<ResolveToRenderTexture>().MediaPlayer = mediaPlayer;

        videoPlaylistManager.mediaPlayers = new GameObject[1];
        videoPlaylistManager.mediaPlayers[0] = mediaPlayer.gameObject;
    }

    void UpdateMediaPath(MediaPathType pathType, string path)
    {
        sourcePath = path;
        Log("ARAnchorMediaManager: UpdateMediaPath: " + path + " (type " + Enum.GetName(typeof(MediaPathType), pathType) + ")");
        HasMedia = true;
    }

    public void Log(string txt)
    {
        Logger.Log(txt);
    }
}
