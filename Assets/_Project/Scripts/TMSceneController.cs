using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TMSceneController : MonoBehaviour
{
    public static TMSceneController Instance;

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

    public void LoadScene(string SceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
    }
    
    public void LoadSceneAdditive(int SceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneIndex, 
            UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void LoadSceneAdditive(string SceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName, 
            UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void UnloadScene(string SceneName)
    {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName,
            UnityEngine.SceneManagement.UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
    }

    public IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoadScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        // Defer scene activation until loaded
        asyncLoadScene.allowSceneActivation = false;

        // Register completed callback method
        asyncLoadScene.completed += OnLoadOpComplete;

        // Monitor load progress
        while (!asyncLoadScene.isDone)
        {
            //Debug.Log($"Loading progress: {asyncLoadScene.progress}");

            // Check if load is waiting for activation (0.9 loaded)
            if (asyncLoadScene.progress >= 0.9f)
            {
                // Debug.Log($"Scene loaded, activating.");
                asyncLoadScene.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private static void OnLoadOpComplete(AsyncOperation asyncOp)
    {
        Debug.Log($"Scene loaded.");
    }
}
