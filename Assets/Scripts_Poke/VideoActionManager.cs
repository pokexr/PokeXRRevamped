using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VideoActionManager : MonoBehaviour
{
    public GameObject VideoActionBG;
    public GameObject ResponceBG;
    public GameObject ReportVideoBG;

    private bool IsVideoPlaying = false;


    private void Start()
    {
        //CheckVideoPlaying();
    }
    public void CheckVideoPlaying()
    {
        IsVideoPlaying = true;
    }
    public void ShowVideoActionBG()
    {
        if (IsVideoPlaying)
        {
            VideoActionBG.SetActive(true);
        }
        else
        {
            ConsoleManager.instance.ShowMessage("Play a video to report.");
        }
    }
    public void ReportVideo(InputField inputField)
    {
        if (inputField.text!="")
        {
            ResponceBG.SetActive(true);
            ReportVideoBG.SetActive(false);
            ConsoleManager.instance.ShowMessage("Responce submitted successfully");
            Debug.Log("Responce " + inputField.text);
            inputField.text = "";
        }
        else
        {
            ResponceBG.SetActive(false);
            ConsoleManager.instance.ShowMessage("Kindly write something!");
            Debug.Log("Responce " + inputField.text);
        }
        
    }
    public void FlagVideo()
    {
        //ConsoleManager.instance.ShowMessage("Video flagged successfully");
        //Debug.Log("Video flagged");
        //VideoActionBG.SetActive(false);
        LoadingManager.Instance.Loading.SetActive(true);
        Debug.Log("FlagVideo ID "+ VideoPlaylistManager.instance.CurrentVideoID);
        StartCoroutine(FlagUserVideo(VideoPlaylistManager.instance.CurrentVideoID));
        //ResponceBG.SetActive(true);
    }
    public void BlockUser()
    {
        //ConsoleManager.instance.ShowMessage("User blocked successfully");
        //Debug.Log("User blocked");
        //VideoActionBG.SetActive(false);
        LoadingManager.Instance.Loading.SetActive(true);
        Debug.Log("BlockUser ID " + VideoPlaylistManager.instance.CurrentUserID);
        StartCoroutine(Block_User(VideoPlaylistManager.instance.CurrentUserID));
    }
    IEnumerator Block_User(int ID)
    {
        WWWForm form = new WWWForm();
        form.AddField("blocked_user_id", ID);
        string requestName = "api/v1/users/block_content";

        using (UnityWebRequest www = UnityWebRequest.Post(AuthManager.BASE_URL + requestName, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                ConsoleManager.instance.ShowMessage("Network error!");
                LoadingManager.Instance.Loading.SetActive(false);
                Debug.Log(www.error);
            }
            else if (www.isHttpError)
            {
                if (www.responseCode == 422)
                {
                    ConsoleManager.instance.ShowMessage("User already blocked");
                }
                else
                {
                    ConsoleManager.instance.ShowMessage("Block error!");
                }
                LoadingManager.Instance.Loading.SetActive(false);
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("User Videos " + www.downloadHandler.text);
                GeneralResponce AccountDeletionResponce = JsonUtility.FromJson<GeneralResponce>(www.downloadHandler.text);
                if (AccountDeletionResponce.success)
                {
                    ConsoleManager.instance.ShowMessage("User Blocked successfully.");
                    VideoActionBG.SetActive(false);
                    //SceneManager.LoadScene(1);;
                    StartCoroutine(Wait());
                }
                else
                {
                    ConsoleManager.instance.ShowMessage("Block error!");
                    LoadingManager.Instance.Loading.SetActive(false);
                }
                LoadingManager.Instance.Loading.SetActive(false);
            }
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(1);;
    }
    IEnumerator FlagUserVideo(int ID)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", ID);
        form.AddField("description", "None");
        string requestName = "api/v1/videos/video_flagged";

        using (UnityWebRequest www = UnityWebRequest.Post(AuthManager.BASE_URL + requestName, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                ConsoleManager.instance.ShowMessage("Network error!");
                LoadingManager.Instance.Loading.SetActive(false);
                Debug.Log(www.error);
            }
            else if (www.isHttpError)
            {
                if (www.responseCode == 422)
                {
                    ConsoleManager.instance.ShowMessage("Video already flagged");
                }
                else
                {
                    ConsoleManager.instance.ShowMessage("Video flag error!");
                }
                LoadingManager.Instance.Loading.SetActive(false);
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("User Videos " + www.downloadHandler.text);
                GeneralResponce AccountDeletionResponce = JsonUtility.FromJson<GeneralResponce>(www.downloadHandler.text);
                if (AccountDeletionResponce.success)
                {
                    ConsoleManager.instance.ShowMessage("Video flagged successfully.");
                    VideoPlaylistManager.instance.PlayVideo();
                    VideoActionBG.SetActive(false);
                    ResponceBG.SetActive(true);
                    //StartCoroutine(WaitToClearData());
                }
                else
                {
                    ConsoleManager.instance.ShowMessage("Video flag error!");
                }
                LoadingManager.Instance.Loading.SetActive(false);
            }
        }
    }
}
