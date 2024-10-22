using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Logger = UnityEngine.XR.ARFoundation.Samples.Logger;

public class TMShareManager : MonoBehaviour
{
    public string testSubject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShareVideoSync(string videoPath)
    {
        Debug.Log("Sharing video: " + videoPath);

        new NativeShare().AddFile(videoPath)
            .SetCallback((result, shareTarget) =>
            {
                Logger.Log("Share result: " + result + ", selected app: " + shareTarget);
            }).Share();

        Debug.Log("Share complete.");

        return;
    }
}
