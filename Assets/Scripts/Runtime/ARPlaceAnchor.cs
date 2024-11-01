using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ARPlaceAnchor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The enabled Anchor Manager in the scene.")]
        ARAnchorManager m_AnchorManager;

        [SerializeField]
        [Tooltip("The Scriptable Object Asset that contains the ARRaycastHit event.")]
        ARRaycastHitEventAsset m_RaycastHitEvent;

        List<ARAnchor> m_Anchors = new();

        public ARAnchorManager anchorManager
        {
            get => m_AnchorManager;
            set => m_AnchorManager = value;
        }

        public void RemoveAllAnchors()
        {
            foreach (var anchor in m_Anchors)
            {
                Destroy(anchor.gameObject);
            }
            m_Anchors.Clear();
        }

        void Reset()
        {
            if (m_AnchorManager == null)
                m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();
        }

        void OnEnable()
        {
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
            foreach (var removedAnchor in eventArgs.removed)
            {
                Destroy(removedAnchor.gameObject);
                m_Anchors.Remove(removedAnchor);
            }
        }

        void OnDisable()
        {
            if (m_RaycastHitEvent != null)
                m_RaycastHitEvent.eventRaised -= CreateAnchor;
            if (m_AnchorManager != null)
                m_AnchorManager.anchorsChanged -= OnAnchorsChanged;
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

            var result = await m_AnchorManager.TryAddAnchorAsync(hit.pose); // Updated API call
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

            m_Anchors.Add(anchor);
        }
    }
}