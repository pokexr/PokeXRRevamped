using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes_Manager : MonoBehaviour
{
    public static Scenes_Manager Instance;

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
    public void SetPreviouseSeenIndex(int SeenIndex)
    {
        PlayerPrefs.SetInt("PreviouseSeenIndex", SeenIndex);
    }
    public void Load_Scene(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
    }
    public void Load_PreviouseScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("PreviouseSeenIndex"));
    }

    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }
}
