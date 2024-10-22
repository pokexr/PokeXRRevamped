using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateScripts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   public void rateUs()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.DefaultCompany.MeshFlix");
#elif UNITY_IPHONE
 Application.OpenURL("itms-apps://itunes.apple.com/app/com.DefaultCompany.MeshFlix");
#endif
    }
}
