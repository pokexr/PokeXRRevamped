using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CurrentUserVideosManager : MonoBehaviour
{
    public static VideosList FetchedVideos;

    public GameObject userProfilePanel;



    private void Start()
    {
        GetVideos();
    }

    public void GetVideos()
    {
        StartCoroutine(GetVideosRequest());
    }
    IEnumerator GetVideosRequest()
    {
        
        string requestName = "api/v1/videos/get_video_list/status?current_user=true";
        Debug.LogFormat("CurrentUserVideosManager:GetVideosRequest: requestName: {0} token: {1}", requestName, AuthManager.Token);

        using (UnityWebRequest www = UnityWebRequest.Get(AuthManager.BASE_URL + requestName))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError  || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                FetchedVideos = JsonUtility.FromJson<VideosList>(www.downloadHandler.text);
             
                foreach (Video video in FetchedVideos.videos)
                {
                    Debug.Log("name " + video.user_id);
                }

            }

        }
    }

    public void OpenProfile()
    {
        userProfilePanel.SetActive(true);
        MyPokogramsGenerator.instance.GenerateGrid(FetchedVideos);
    }
}
