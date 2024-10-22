using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimedSceneLoad : MonoBehaviour
{
    public static TimedSceneLoad Instance;

    public float TimeToPlay = 6f;
    public float timeElapsed = 0f;
    public bool isPlaying = false;
    public string SceneToLoad;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        PlayInNSeconds(TimeToPlay);
    }

    void Update()
    {
        if (isPlaying)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= TimeToPlay)
            {
                isPlaying = false;
                timeElapsed = 0;
                LoadSceneByName(SceneToLoad);
            }
        }
    }

    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void PlayInNSeconds(float seconds)
    {
        isPlaying = true;
    }
}
