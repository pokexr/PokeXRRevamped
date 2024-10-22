using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserVideoItem : MonoBehaviour
{
    [HideInInspector]
    public string videoURL;
    public static string selectedUrl;
    public int id;
    public string thumbnail_url;
    

    public void Init(string videoUrl, int id, string thumbnail_url)
    {
        videoURL = videoUrl;
        this.id = id;
        this.thumbnail_url = thumbnail_url;
        GetComponent<Button>().onClick.AddListener(OnClick);
        LoadThumbnail();
    }

    void OnClick()
    {
        selectedUrl = videoURL;
        SceneManager.LoadScene("AR");
    }

    private void LoadThumbnail()
    {
        string localUrl = string.Format("{0}/{1}.jpg", Application.persistentDataPath, "" + id);

        if (File.Exists(localUrl))
        {
            byte[] bytes;
            bytes = File.ReadAllBytes(localUrl);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite thumbnail = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            GetComponent<Image>().sprite = thumbnail;
        }
        else
        {
            StartCoroutine(GetThumbnail(thumbnail_url));
        }
    }
    IEnumerator GetThumbnail(string uri)
    {
        Debug.Log(uri);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);

        Debug.Log(AuthManager.Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.responseCode);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            Sprite thumbnail = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            GetComponent<Image>().sprite = thumbnail;
            string localUrl = string.Format("{0}/{1}.jpg", Application.persistentDataPath, "" + id);
            File.WriteAllBytes(localUrl, texture.EncodeToJPG());

        }
    }
}
