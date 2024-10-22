using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonItem : MonoBehaviour
{
    //public static ButtonItem instance;

    public Datum Button_user;
    public Image serverImage;

    public Text billborad_title;
    public string RedeemUrl;

    public void Init()
    {
        StartCoroutine(GetThumbnail(Button_user.image_url, Button_user.title, Button_user.description, Button_user.consumption_date, Button_user.url));
    }

    private void Start()
    {
        billborad_title.text = Button_user.title;
    }

    private IEnumerator GetThumbnail(string uri,string title,string des, string date, string URl)
    {
        Debug.Log(uri);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
        StartCoroutine(WaitForResponse(www));
        www.SetRequestHeader("Content-type", "application/json");
        //www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
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
            serverImage.sprite = thumbnail;
        }
    }


    public void DisplayInfo()
    {
        GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().ImagePopUp.sprite = serverImage.sprite;
        GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().ImagePopUpPanel.SetActive(true);
        GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().description_title.text = Button_user.title;
        GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().descrition_txt.text = Button_user.description;
        GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().consumption_date.text = Button_user.consumption_date;
        Debug.Log(Button_user.url);
        GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().ConsumedURl = Button_user.url;
        // descrition_txt.text = BillBoardsAPICount.instance.descrition_txt.ToString();
        // description_title.text = BillBoardsAPICount.instance.description_title.ToString();
        //        Debug.Log("maryum " + description_title.ToString());
        // GameObject.Find("ConsumeLocationPopUp").GetComponent<ConsumeLocationPopUp>().ImagePopUp.sprite = serverImage.sprite;
        // Debug.Log("Button Id = " + Button_user.id);
        // PlayerPrefs.SetInt("indexToPass", Button_user.id);
        // LoadingManager.instance.loading.SetActive(true);
        // ButtonsUIManager.instance.ModelProgressText.gameObject.SetActive(true);
        //CheckModel();
    }
    private void CheckModel()
    {
        string path = Application.persistentDataPath + "/" + Button_user.id + ".png";
        string ModelDestination = Application.persistentDataPath + "/" + Button_user.id + ".glb";
        if (File.Exists(ModelDestination))
        {
            //SceneManager.LoadScene(5);
        }
        else if (File.Exists(path))
        {
           //SceneManager.LoadScene(5);
        }
        else if (Button_user.modal_url == "" || Button_user.modal_url == null)
        {
            if (Button_user.image_url != "" || Button_user.image_url != null)
            {
                Debug.Log("Image Link");
                StartCoroutine(GetThumbnail(Button_user.image_url, Button_user.title, Button_user.description,Button_user.consumption_date, Button_user.url));
            }
        }
        else
        {
            Debug.Log("modell Link  Q" + Button_user.modal_url + "Q");
            StartCoroutine(GetModel(Button_user.modal_url));
        }
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
            //ARLocationsManager.instance.SavedPath.text = "Saved P " + ModelDestination;
            //ARLocationsManager.instance.Progress.text = "Processing";
            LoadingManager.instance.loading.SetActive(false);
           // SceneManager.LoadScene(5);
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
