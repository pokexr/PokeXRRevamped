using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TMPointerOverUI : MonoBehaviour
{
    private ARRaycastManager m_RaycastManager;
    private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        m_RaycastManager = FindFirstObjectByType<ARRaycastManager>();
    }

    public static bool IsPointerOverUIObject(Vector2 touchPosition)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = touchPosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        return raycastResults.Count > 0;
    }

    private bool PhysicRayCastBlockedByUi(Vector2 touchPosition)
    {
        if (IsPointerOverUIObject(touchPosition))
        {
            return false;
        }

        return m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon);
    }
}