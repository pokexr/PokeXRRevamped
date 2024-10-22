 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ShareScript : MonoBehaviour
{
	private string shareMsg;
    
    public void clickShare()
    {
		StartCoroutine(TakeScreenshotAndShare());   
    }
	private IEnumerator TakeScreenshotAndShare()
	{
		yield return new WaitForEndOfFrame();
        new NativeShare().AddFile(shareMsg)
			.SetSubject("meshFlix").SetText(shareMsg).SetUrl("https://github.com/yasirkula/UnityNativeShare")
			.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
			.Share();
	}
}
