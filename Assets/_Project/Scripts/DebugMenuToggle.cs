using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DebugMenuToggle : MonoBehaviour
{
    public GameObject debugMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleDebugMenu()
    {
        Debug.Log("DebugMenuToggle:ToggleDebugMenu()");
        debugMenu.SetActive(!debugMenu.activeSelf);
    }
}
