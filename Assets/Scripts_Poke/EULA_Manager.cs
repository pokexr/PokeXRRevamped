using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EULA_Manager : MonoBehaviour
{
    public GameObject EulaPopup;
    public Toggle toggle;
    void Start()
    {
        if (PlayerPrefs.GetString("EULA")=="True")
        {
            EulaPopup.SetActive(false);
        }
        else
        {
            EulaPopup.SetActive(true);
        }
    }

    public void OpenLink(Text url)
    {
        Application.OpenURL(url.text);
    }
    public void AcceptEulaFunc()
    {
        if (toggle.isOn)
        {
            EulaPopup.SetActive(false);
        }
        else
        {
            ConsoleManager.instance.ShowMessage("Kindly check the checkbox to accept.");
        }
        PlayerPrefs.SetString("EULA", toggle.isOn.ToString());
    }
    public void QuitApp()
    {
        Application.Quit();
    }
}
