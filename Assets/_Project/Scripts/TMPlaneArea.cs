using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TMPlaneArea : MonoBehaviour
{
    public float CurrentAreaMeters = 0f;

    public bool DisplayDebugVisuals;
    public TextMeshPro AreaText;
    public GameObject[] DebugVisualObjects;

    public ARPlane ArPlane;

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the ARPlane boundary changed event
        ArPlane = GetComponent<ARPlane>();
        ArPlane.boundaryChanged += ArPlane_BoundaryChanged;

        // Hide any objects in debug visuals array as needed
        if (!DisplayDebugVisuals) {
            // Disable each of the GameObjects contained in DebugVisualObjects
            foreach (var obj in DebugVisualObjects)
            {
                obj.SetActive(false);
            }
        }
    }

    void Update()
    {
        // Billboard the area text to device
        AreaText.transform.rotation = Quaternion.LookRotation(AreaText.transform.position - Camera.main.transform.position);
    }

    void OnDestroy()
    {
        // Unsubscribe from the ARPlane boundary changed event
        ArPlane.boundaryChanged -= ArPlane_BoundaryChanged;
    }

    [ContextMenu("Toggle Area View")]
    public void ToggleAreaView()
    {
        // Disable each of the GameObjects contained in DebugVisualObjects
        foreach (var obj in DebugVisualObjects)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }

    /* --- AR Plane Change Subscriptions --- */
    private void ArPlane_BoundaryChanged(ARPlaneBoundaryChangedEventArgs obj)
    {
        CurrentAreaMeters = CalculatePlaneArea(obj.plane);
        AreaText.text = $"{CurrentAreaMeters} m²";
    }
    private float CalculatePlaneArea(ARPlane plane)
    {
        // Calculate the area of the plane
        return plane.size.x * plane.size.y;
    }
}
