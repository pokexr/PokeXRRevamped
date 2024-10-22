using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressSceneLoad : MonoBehaviour
{
    public string SceneToLoad;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProgressSceneLoadCheck(Single percent)
    {
        Debug.Log(percent);
        if (percent == 100.0)
        {
            LoadSceneByName(SceneToLoad);
        }
    }

    public void LoadSceneByName(string name)
    {
        Scenes_Manager.Instance.LoadSceneByName(name);
    }
}
