using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OPenPanel : MonoBehaviour
{
    public GameObject infopnel;
    public GameObject infopnel1;
    // Start is called before the first frame update
    public void openPanel()
    {
        infopnel.SetActive(true);
        //infopnel1.SetActive(false);
        Debug.Log("Info panel is opened");
    }
    public void closeinfo()
    {
       infopnel.SetActive(false);
        infopnel1.SetActive(true);
        Debug.Log("Info panel is closed");

    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

}
