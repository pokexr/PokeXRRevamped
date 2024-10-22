using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Michsky.MUIP;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class TMARSceneController : MonoBehaviour
{
    public static TMARSceneController Instance;

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

    public UnityEvent onScanComplete;

    [Range(10f, 100f)]
    public float MinimumTotalArea;
    public bool ScanComplete = false;
    public float ScanPercentage = 0f;
    public ProgressBar ScanProgressBar;

    public GameObject DebugPanelRoot;
    public TextMeshProUGUI DebugText;

    public List<TMPlaneArea> PlaneAreas;

    // List of ARVideoPrefabControllers
    [SerializeField]
    private List<ARVideoPrefabController> VideoControllers;

    [SerializeField]
    private List<string> TestMediaUrls;

    // Get accessor for the count of video controllers
    [SerializeField]
    private int VideoControllerCount
    {
        get
        {
            return VideoControllers.Count;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        onScanComplete.AddListener(HandleScanComplete);

        PlaneAreas = new List<TMPlaneArea>();

        // Clear the list of video controllers
        ClearVideos();

        // Clear debug log
        ClearLog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterARVideoPrefabController(ARVideoPrefabController videoController)
    {
        VideoControllers.Add(videoController);
        Log($"TMARSceneController: RegisterARVideoPrefabController: Registered {videoController.name} (Count is {VideoControllerCount})");
    }

    public void UnregisterARVideoPrefabController(ARVideoPrefabController videoController)
    {
        VideoControllers.Remove(videoController);
        Log($"TMARSceneController: UnregisterARVideoPrefabController: Unregistered {videoController.name} (Count is {VideoControllerCount})");
    }



    public void PauseAllVideos()
    {
        for (int i = 0; i < VideoControllerCount; i++)
        {
            VideoControllers[i].Pause();
        }
    }

    public void PlayAllVideos()
    {
        for (int i = 0; i < VideoControllerCount; i++)
        {
            VideoControllers[i].Play();
        }
    }

    public void Shuffle()
    {
        for (int i = 0; i < VideoControllerCount; i++)
        {
            LoadVideoController(VideoControllers[i], 
                TestMediaUrls[UnityEngine.Random.Range(0, TestMediaUrls.Count)],
                playOnLoad: false,
                muteOnPlay: true
            );
        }
    }

    void LoadVideoController(ARVideoPrefabController videoController, string url, bool playOnLoad = false, bool muteOnPlay = true)
    {
        string randomUrl = TestMediaUrls[UnityEngine.Random.Range(0, TestMediaUrls.Count)];
        Log($"ARController: Shuffle: Loading video controller {videoController.name} with {randomUrl}");
        videoController.LoadMediaAtUrl(randomUrl);

        if (playOnLoad)
        {
            videoController.VideoPlayer.Control.MuteAudio(muteOnPlay);
            videoController.Play();
        }
    }

    public void ToggleMuteAll()
    {
        for (int i = 0; i < VideoControllerCount; i++)
        {
            VideoControllers[i].ToggleMute();
        }
    }

    public void SetMuteAll(bool mute)
    {
        for (int i = 0; i < VideoControllerCount; i++)
        {
            VideoControllers[i].SetMute(mute);
        }
        Debug.Log($"TMARSceneController: SetMuteAll: {mute} complete.");
    }

    public void UnmuteLast()
    {
        if (VideoControllerCount > 0)
        {
            VideoControllers[VideoControllerCount - 1].SetMute(false);
        }
        Debug.Log("TMARSceneController: UnmuteLast: complete.");
    }

    public void ClearVideos()
    {
        VideoControllers.Clear();
        FindObjectsByType<ARVideoPrefabController>(FindObjectsSortMode.None)
            .ToList()
            .ForEach(videoController =>
            {
                Destroy(videoController.gameObject);
            });
    }

    public void ClearLastVideo()
    {
        if (VideoControllerCount > 0)
        {
            VideoControllers.RemoveAt(VideoControllerCount - 1);
            Destroy(VideoControllers[VideoControllerCount - 1].gameObject);
        }
    }

    public void ClearLog()
    {
        DebugText.text = "";
    }

    public void Log(string message)
    {
        Debug.Log(message);
        if (DebugText != null)
        {
            DebugText.text += $"{message}\n";
        }
    }

    public void UpdatePlaneAreas(ARPlaneBoundaryChangedEventArgs obj)
    {
        var newObj = obj.plane.GetComponent<TMPlaneArea>();
        PlaneAreas.Add(newObj);
        Log($"TMARSceneController: UpdatePlaneAreas: {newObj.CurrentAreaMeters}");
    }

    [SerializeField]
    public ARPlane LargestPlane;

    [SerializeField]
    public ARPlane FarthestPlane;

    [SerializeField]
    public float LargestPlaneArea;

    [SerializeField]
    public float FarthestPlaneDistance;

    public float TotalCoveredSqMeters;

    [ContextMenu("Calculate Plane Area Values")]
    public void CalculatePlaneAreaValues()
    {
        PlaneAreas.Clear();
        PlaneAreas = FindObjectsByType<TMPlaneArea>(FindObjectsSortMode.None).ToList();

        // Find the largest plane area in PlaneAreas based on CurrentAreaMeters value,
        // and set LargestPlane and LargestPlaneArea to the largest plane and its area.
        PlaneAreas.Select(planeArea =>
        {
            float distance = Vector3.Distance(Camera.main.transform.position, planeArea.ArPlane.transform.position);
            if (distance > FarthestPlaneDistance)
            {
                FarthestPlaneDistance = distance;
                FarthestPlane = planeArea.ArPlane;
            }

            if (planeArea.CurrentAreaMeters > LargestPlaneArea)
            {
                LargestPlaneArea = planeArea.CurrentAreaMeters;
                LargestPlane = planeArea.ArPlane;
            }

            TotalCoveredSqMeters += planeArea.CurrentAreaMeters;


            return planeArea;
        }).ToList();

        CheckMinimumTotalArea();

        // Log($"TMARSceneController: CalculatePlaneAreaValues: Largest Plane Area is {LargestPlaneArea} on {LargestPlane.trackableId}");
        // Log($"TMARSceneController: CalculatePlaneAreaValues: Farthest Plane Distance is {FarthestPlaneDistance} on {FarthestPlane.trackableId}");
        // Log($"TMARSceneController: CalculatePlaneAreaValues: Total Covered Area is {TotalCoveredSqMeters} m^2");
    }

    public bool CheckMinimumTotalArea()
    {
        ScanPercentage = TotalCoveredSqMeters / MinimumTotalArea * 100;
        ScanProgressBar.currentPercent = ScanPercentage <= 100f ? ScanPercentage : 100f;

        if (TotalCoveredSqMeters >= MinimumTotalArea)
        {
            onScanComplete.Invoke();
            Log($"TMARSceneController: Reached minimum total area: {TotalCoveredSqMeters} m^2");
            return true;
        }
        else
        {
            //Log($"TMARSceneController: Percentage minimum area is {ScanPercentage}%");
            return false;
        }
    }

    [ContextMenu("Simulate scan complete")]
    public void HandleScanComplete()
    {
        ScanComplete = true;
        ScanProgressBar.GetComponentInChildren<TextMeshProUGUI>().text = "Scan Complete.";
        Log("TMARSceneController: Scan Complete!");

        // Place a mystery box at the center of the largest plane
        if (LargestPlane != null)
        {
            Vector3 center = LargestPlane.center;
            Vector3 position = LargestPlane.transform.position + center;
            GameObject mysteryBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mysteryBox.transform.position = position;
            mysteryBox.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            mysteryBox.AddComponent<BoxCollider>();
        }
    }
}
