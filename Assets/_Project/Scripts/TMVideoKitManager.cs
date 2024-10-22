using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VideoKit;

/**
Properties for media recorder instantiation:

// static Task<MediaRecorder> Create (
//   Format format,
//   int width = 0,
//   int height = 0,
//   float frameRate = 0f,
//   int sampleRate = 0,
//   int channelCount = 0,
//   int videoBitRate = 20_000_000,
//   int keyframeInterval = 2,
//   float compressionQuality = 0.8f,
//   int audioBitRate = 64_000,
//   string? prefix = null
// );
 */

public class TMVideoKitManager : MonoBehaviour
{
    public bool IsCameraReady = false;
    public bool IsRecording = false;
    public int WaitDuration = 5;

    public GameObject StartButton;
    public GameObject StopButton;

    public GameObject LoadingPanel;

    public VideoKitCameraManager vkCamera;

    public VideoKitAudioManager vkAudio;

    public VideoKitRecorder vkRecorder;

    // Start is called before the first frame update
    public void Start()
    {
        SetCameraState();
        StartCoroutine(WaitForCamera());
    }

    private IEnumerator WaitForCamera()
    {
        yield return new WaitForSeconds(WaitDuration);
        SetCameraReady(true);
    }

    // Update is called once per frame
    void Update()
    {
        SetCameraState();
    }

    public void SetCameraReady(bool ready)
    {
        if (IsCameraReady != ready)
        {
            IsCameraReady = ready;
            LoadingPanel.SetActive(false);
            Debug.Log("Camera Ready: " + IsCameraReady);
        }
    }

    void SetCameraState()
    {
        if (IsRecording)
        {
            StartButton.SetActive(false);
            StopButton.SetActive(true);
        }
        else
        {
            StartButton.SetActive(true);
            StopButton.SetActive(false);
        }
    }

    public void StartRecording()
    {
        StartRecordingHologram();
    }

    public void StopRecording()
    {
        StopRecordingHologram();
    }

    [ContextMenu("Start Recording")]
    public void StartRecordingHologram()
    {
        IsRecording = true;
        vkRecorder.StartRecording();
    }

    [ContextMenu("Stop Recording")]
    public void StopRecordingHologram()
    {
        IsRecording = false;
        vkRecorder.StopRecording();
    }

    /// <summary>
    /// Once recording is complete, this callback will fire and invoke any
    /// listeners subscribed to it.
    /// </summary>
    public void OnRecordingComplete(MediaAsset recordAsset)
    {
        Debug.Log("Recording Complete, saved to " + recordAsset.path);
    }
}
