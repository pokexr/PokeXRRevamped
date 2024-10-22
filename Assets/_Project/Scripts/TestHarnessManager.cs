using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHarnessManager : MonoBehaviour
{
    public GameObject XrRig;

    [Range(1f, 10f)]
    public float SpinSpeedModifier = 1f;

    public bool AutoSpinOnY = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (AutoSpinOnY)
        {
            XrRig.transform.Rotate(Vector3.up, Time.deltaTime * SpinSpeedModifier);
        }
    }
}
