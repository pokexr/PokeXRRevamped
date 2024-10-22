using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TMARTestbed : MonoBehaviour
{
    [SerializeField] private TMP_InputField tokenField;

    private string token;

    public ARCameraManager arCam;
    public ARPlaneManager arPlane;
    public ARAnchorManager arAnchor;

    // Start is called before the first frame update
    void Start()
    {
        // Go get PlayerPref values
        token = PlayerPrefs.GetString("Token");

        arCam = FindFirstObjectByType<ARCameraManager>();
        arPlane = FindFirstObjectByType<ARPlaneManager>();
        arAnchor = FindFirstObjectByType<ARAnchorManager>();

        if (arCam == null || arPlane == null || arAnchor == null)
        {
            Debug.LogError("TMARTestbed: ARCameraManager, ARPlaneManager, or ARAnchorManager not found.");
        }
        else
        {
            Debug.Log("TMARTestbed: Initialized (cam, planes, anchors)");
        }

        UpdateInputValue(tokenField, token);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInputValue(TMP_InputField inputField, string value)
    {
        inputField.text = value;
    }

    public void Log(string str)
    {
        Debug.Log("TMARTestbed: " + str);
    }
}
