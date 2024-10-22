using System;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;

namespace UnityEngine.XR.ARFoundation.Samples
{
  public class TMARPlaceMediaAnchor : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The enabled Anchor Manager in the scene.")]
    ARAnchorManager m_AnchorManager;

    [SerializeField]
    [Tooltip("The Scriptable Object Asset that contains the ARRaycastHit event.")]
    ARRaycastHitEventAsset m_RaycastHitEvent;

    public GameObject PrefabToInstantiate;

    public GameObject CurrentAnchor;

    List<ARAnchor> m_Anchors = new();

    public ARAnchorMediaManager anchorMediaManager;

    public ARAnchorManager anchorManager
    {
      get => m_AnchorManager;
      set => m_AnchorManager = value;
    }

    [ContextMenu("Remove All Anchors")]
    public void RemoveAllAnchors()
    {
      foreach (var anchor in m_Anchors)
      {
        Destroy(anchor.gameObject);
      }
      m_Anchors.Clear();
    }

    [ContextMenu("Reset Anchors")]
    // Runs when the reset option is called in the context menu in-editor, or when first created.
    void Reset()
    {
      if (m_AnchorManager == null)
        m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();
    }

    void OnEnable()
    {
      if (anchorMediaManager == null)
        anchorMediaManager = FindAnyObjectByType<ARAnchorMediaManager>();

      if (m_AnchorManager == null)
        m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();

      if ((m_AnchorManager ? m_AnchorManager.subsystem : null) == null)
      {
        enabled = false;
        Debug.LogWarning($"No XRAnchorSubsystem was found in {nameof(ARPlaceAnchor)}'s {nameof(m_AnchorManager)}, so this script will be disabled.", this);
        return;
      }

      if (m_RaycastHitEvent == null)
      {
        enabled = false;
        Debug.LogWarning($"{nameof(m_RaycastHitEvent)} field on {nameof(ARPlaceAnchor)} component of {name} is not assigned.", this);
        return;
      }

      m_RaycastHitEvent.eventRaised += CreateAnchor;
      m_AnchorManager.anchorsChanged += OnAnchorsChanged;
    }

    void OnAnchorsChanged(ARAnchorsChangedEventArgs eventArgs)
    {
      //remove any anchors that have been removed outside our control, such as during a session reset
      foreach (var removedAnchor in eventArgs.removed)
      {
        Destroy(removedAnchor.gameObject);
        m_Anchors.Remove(removedAnchor);
      }

      // Instantiate the prefab at the position of the new anchor if
      // necessary, otherwise reposition the existing asset.
      foreach (var addedAnchor in eventArgs.added)
      {
        if (CurrentAnchor == null)
        {
          anchorMediaManager.Log("TMARPlaceMediaAnchor: OnAnchorsChanged: Instantiated AR Media Anchor: " + addedAnchor.trackableId + " at " + addedAnchor.transform.position + " with rotation " + addedAnchor.transform.rotation);

          CurrentAnchor = Instantiate(PrefabToInstantiate, addedAnchor.transform.position, addedAnchor.transform.rotation);
          FindAnyObjectByType<ARAnchorMediaManager>().ARAnchor3DTarget = CurrentAnchor;
          CurrentAnchor.GetComponentInChildren<DisplayUGUI>().Player = FindAnyObjectByType<ARAnchorMediaManager>().mediaPlayer;

          FindAnyObjectByType<ARAnchorMediaManager>().mediaPlayer.OpenMedia(autoPlay: true);
        }
        else
        {
          anchorMediaManager.Log("TMARPlaceMediaAnchor: OnAnchorsChanged: Moved AR Media Anchor: " + CurrentAnchor.name + " to " + addedAnchor.transform.position + " with rotation " + addedAnchor.transform.rotation);

          CurrentAnchor.transform.position = addedAnchor.transform.position;
          CurrentAnchor.transform.rotation = addedAnchor.transform.rotation;
        }
      }
    }

    void OnDisable()
    {
      if (m_RaycastHitEvent != null)
        m_RaycastHitEvent.eventRaised -= CreateAnchor;
      if (m_AnchorManager != null)
        m_AnchorManager.anchorsChanged += OnAnchorsChanged;
    }

    /// <summary>
    /// Attempts to attach a new anchor to a hit `ARPlane` if supported.
    /// Otherwise, asynchronously creates a new anchor.
    /// </summary>
    async void CreateAnchor(object sender, ARRaycastHit hit)
    {
      if (m_AnchorManager.descriptor.supportsTrackableAttachments && hit.trackable is ARPlane plane)
      {
        var attachedAnchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
        FinalizePlacedAnchor(attachedAnchor, $"Attached to plane {plane.trackableId}");
        return;
      }

      var result = await m_AnchorManager.TryAddAnchorAsync(hit.pose);
      if (result.status.IsSuccess())
      {
        FinalizePlacedAnchor(result.value, $"Anchor (from {hit.hitType})");
      }
    }

    void FinalizePlacedAnchor(ARAnchor anchor, string text)
    {
      var canvasTextManager = anchor.GetComponent<CanvasTextManager>();
      if (canvasTextManager != null)
      {
        canvasTextManager.text = text;
      }
      anchorMediaManager.Log(text);

      m_Anchors.Add(anchor);
    }
  }
}
