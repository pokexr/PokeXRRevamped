using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class TMGalleryView : MonoBehaviour
{
    public bool DebugMode;

    /// <summary>
    /// Run Update to arc for debug purposes, don't leave this active.
    /// </summary>
    public bool EditLive;

    [Range(1f, 360f)]
    public float angleBetweenObjects = 10f; // In degrees

    /* --- Properties in use --- */

    [Range(1f, 10f)]
    public float ArcRadius = 1f;

    public int AmountToSpawn;

    public List<GameObject> GalleryObjects;

    [SerializeField] private Transform ArcStartPoint;

    [SerializeField] private Transform ArcEndPoint;
    [SerializeField] private Transform ArcCenter;

    [SerializeField] private float ArcHeight = 5f;

    [SerializeField] private int ArcResolution = 10;

    [SerializeField] private int ArcModifier = 4;

    [SerializeField] private List<MediaReference> TestMediaReferences;

    public GameObject GalleryObjectPrefab;

    void Start()
    {
        if (GalleryObjects == null)
        {
            GalleryObjects = new List<GameObject>();
        }
        InitializeGallery();
    }

    void Update()
    {
        if (EditLive)
        {
            OrganizeInArc();
        }
    }

    [ContextMenu("Initialize Gallery")]
    public void InitializeGallery()
    {
        GalleryObjects.Clear();
        var cam = Camera.main;
        var spawnPos = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);

        transform.Translate(spawnPos);
        SpawnObjects();
        OrganizeInArc();
    }

    [ContextMenu("Test Play all Players")]
    public void TestPlayAllPlayers()
    {
        StartAllPlayers();
    }

    [ContextMenu("Test Stop all Players")]
    public void TestStopAllPlayers()
    {
        StopAllPlayers();
    }

    private void OnDrawGizmos()
    {
        if (!DebugMode)
        {
            return;
        }

        // DrawArc();
        DrawCircle();
    }

    private void DrawCircle()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ArcCenter.position, ArcRadius);

        for (int i = 0; i < GalleryObjects.Count; i++)
        {
            Gizmos.color = Color.red;
        }
    }

    private void DrawArc()
    {
        Vector3 startPos = ArcStartPoint.position;
        Vector3 endPos = ArcEndPoint.position;
        Vector3 arcDirection = endPos - startPos;
        float arcLength = arcDirection.magnitude;
        arcDirection.Normalize();
        Gizmos.color = Color.green;
        Vector3 lastPos = startPos;
        for (int i = 1; i <= ArcResolution; i++)
        {
            float t = i / (float)ArcResolution;
            float parabola = (-ArcModifier * ArcRadius * t) + (ArcModifier * ArcRadius * t * t);
            Vector3 nextPos = Vector3.Lerp(startPos, endPos, t) + new Vector3(0, 0, parabola);
            Gizmos.DrawLine(lastPos, nextPos);
            Debug.Log("Drawing arc from " + lastPos + " to " + nextPos);
            lastPos = nextPos;
        }
    }

    void SpawnObjects()
    {
        for (int i = 0; i < AmountToSpawn; i++)
        {
            GameObject go = Instantiate(GalleryObjectPrefab, transform);
            go.transform.position = Vector3.zero; // Position out front to extent
            GalleryObjects.Add(go); // Add to gallery objects

            // Debug.Log("Instantiated object " + go.name + " at position " + go.transform.position.ToString());
        }
    }

    void OrganizeInArc()
    {
        float totalArcAngle = angleBetweenObjects * (GalleryObjects.Count - 1);
        float startAngle = -totalArcAngle / 2;

        for (int i = 0; i < GalleryObjects.Count; i++)
        {
            float angle = startAngle + (angleBetweenObjects * i);
            float radian = angle * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Sin(radian) * ArcRadius, 0, Mathf.Cos(radian) * ArcRadius);
            GalleryObjects[i].transform.position = ArcCenter.transform.position + position;

            // Debug.Log("Arc angle: " + angle + " radian: " + radian + " position: " + position.ToString());
        }
    }

    public void StartAllPlayers()
    {
        Debug.Log($"TMGalleryView: Starting all players for {gameObject.name}");
        foreach (var go in GalleryObjects)
        {
            MediaPlayer player = go.GetComponent<ARVideoPrefabController>().VideoPlayer;
            player.Control.Play();
        }
        Debug.Log($"TMGalleryView: Done.");
    }

    public void StopAllPlayers()
    {
        Debug.Log($"TMGalleryView: Stopping all players for {gameObject.name}");
        foreach (var go in GalleryObjects)
        {
            MediaPlayer player = go.GetComponent<ARVideoPrefabController>().VideoPlayer;
            player.Control.Pause();
        }
        Debug.Log($"TMGalleryView: Done.");
    }
}