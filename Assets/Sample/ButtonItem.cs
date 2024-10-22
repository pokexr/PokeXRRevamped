using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonItem : MonoBehaviour
{
    public static ButtonItem buttonItem;
    //  public int scene;

    //public static ButtonItem instance;
   public  static string buttonvideoURl;

    public Video Button_user;
   // public Image serverImage;

    public Text billborad_title;
    //public string RedeemUrl;

    public void Init()
    {
        StartCoroutine(GetThumbnail(Button_user.id, Button_user.name));
    }

    private void Start()
    {
        billborad_title.text = Button_user.id.ToString();
        Debug.Log("Name is here" + Button_user.id.ToString());
    }

    private IEnumerator GetThumbnail(int id,string name)
    {
        Debug.Log(name);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(name);
        StartCoroutine(WaitForResponse(www));
        www.SetRequestHeader("Content-type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.responseCode);
        }
        else
        {
           
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            Debug.Log("Image Downloaded!");
            Sprite thumbnail = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
           // serverImage.sprite = thumbnail;
        }
    }

    public void loadScene()
    {
        SceneManager.LoadScene("VideoAR");

    }

    public void LoadVideo()
    {
        Debug.Log(Button_user.result_video_url);
        buttonvideoURl = Button_user.result_video_url;

    }

    public void DisplayInfo()
    {
        //GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().ImagePopUp.sprite = serverImage.sprite;
        //GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().ImagePopUpPanel.SetActive(true);
        //GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().description_title.text = Button_user.title;
        //GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().descrition_txt.text = Button_user.description;
        //GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().consumption_date.text = Button_user.consumption_date;
        Debug.Log(Button_user.result_video_url);
    }
    private void CheckModel()
    {
    }
    private IEnumerator GetModel(string uri)
    {
        UnityWebRequest www = UnityWebRequest.Get(uri);
        StartCoroutine(WaitForResponse(www));
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string ModelDestination = Application.persistentDataPath + "/" + Button_user.id + ".glb";
            File.WriteAllBytes(ModelDestination, www.downloadHandler.data);
            Debug.Log("Model Downloaded! and save at: " + ModelDestination);
        }
    }
    private IEnumerator WaitForResponse(UnityWebRequest request)
    {
        while (!request.isDone)
        {
            ButtonsUIManager.instance.ModelProgressText.text = "Downloading " + (request.downloadProgress * 100).ToString("F0") + "%";
            Debug.Log("Loading " + (request.downloadProgress * 100).ToString("F0") + "%");
            //LoadingManager.instance.progress.text = "" + (request.downloadProgress * 100).ToString("F0") + "%";
            yield return null;
        }
    }
    //public void GetUrlForRedeem(R)
    //{
    //    Application.OpenURL(URL);
    //    Debug.Log(URL);
    //}

}
