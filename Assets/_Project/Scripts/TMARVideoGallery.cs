using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TMARVideoGallery : MonoBehaviour
{
    public bool PlayOnStart;

    public bool MuteOnStart;

    public bool BillboardTowardCamera;

    
    [SerializeField]
    public GameObject VideoPlayerPrefab;

    [Range(1, 4)]
    public int VideoPlayerCount = 3;

    [Range(0.1f, 10.0f)]
    public float GalleryRadiusScale = 1.0f;

    [SerializeField]
    public List<ARVideoPrefabController> VideoPlayersList = new();

    public Vector3[] playerPositions;

    // Start is called before the first frame update
    void Start()
    {
        // Kill billboarding if not enabled
        if (!BillboardTowardCamera)
        {
            GetComponent<LookAtCameraBillboard>().enabled = false;
        }

        // Set some default zero positions
        if (playerPositions.Count() < VideoPlayerCount)
        {
            playerPositions = new Vector3[VideoPlayerCount];
            for (int i = 0; i < VideoPlayerCount; i++)
            {
                playerPositions[i] = new Vector3(0, 0, 0);
            }
        }
        SpawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGalleryView();
    }

    public void UpdateGalleryView()
    {
        // TODO: Update view in realtime based on arc/radius values
        // Only attempt for instantiated players
        for (int i = 0; i < VideoPlayersList.Count; i++)
        {
            VideoPlayersList[i].transform.localPosition = playerPositions[i] * GalleryRadiusScale;
        }
    }

    public void SpawnPlayers()
    {
        for (int i = 0; i < VideoPlayerCount; i++)
        {
            var player = Instantiate(VideoPlayerPrefab, transform);
            player.transform.localPosition = playerPositions[i] * GalleryRadiusScale;
            VideoPlayersList.Add(player.GetComponent<ARVideoPrefabController>());

            UpdateGalleryView();
        }
    }
}
