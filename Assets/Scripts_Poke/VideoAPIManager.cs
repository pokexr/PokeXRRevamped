using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class VideoAPIManager : MonoBehaviour
{
    public static VideoAPIManager instance;
 
    private VideosList videosList;

    public GameObject loading;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(GetVideoRequest());

    }
    public static string BASE_URL = "https://pokexr.com";

    public void reloadScene()
    {
        SceneManager.LoadScene("VideoAR");
    }


    //GetVideo from server
    public void GetVideo()
    {
        StartCoroutine(GetVideoRequest());
    }
    IEnumerator GetVideoRequest()
    {
        //WWWForm form = new WWWForm();
        string requestName = "/api/v1/videos/get_video_list/done";
        //AuthManager.Token = "5324904a468b6671d09be41e8d837534";

        using (UnityWebRequest www = UnityWebRequest.Get(BASE_URL+requestName))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                VideosList FetchedVideos = JsonUtility.FromJson<VideosList>(www.downloadHandler.text);
                VideoPlaylistManager.instance.StartList(FetchedVideos);

                if (loading != null)
                    loading.SetActive(false);

                Debug.Log("VideoAPIManager:GetVideoRequest(): FetchedVideos.videos.Count: " + FetchedVideos.videos.Count);

                // foreach (Video video in FetchedVideos.videos)
                // {
                //     Debug.Log("id " + video.id);
                //     Debug.Log("name " + video.name);
                // }
               
            }

        }
    }
}
