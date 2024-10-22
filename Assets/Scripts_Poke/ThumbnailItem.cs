using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ThumbnailItem : MonoBehaviour
{
    public UserVideo User_Video;
    public Button Thumbnail_Item;
    public int ThumbnailItemIndex = 0;
    public bool FriendThumbnailItem = false;
    private string localURL;
    
    public void Init(UserVideo User)
    {
        User_Video = User;
        if (User.thumbnail_url!="")
        {
            GetUserImage(User.id, User.thumbnail_url);
        }
    }
    public void PlayUserVideo()
    {
        if (User_Video.result_video_url=="" || User_Video.result_video_url==null)
        {
            ConsoleManager.instance.ShowMessage("Video not found");
        }
        else
        {
            if (VideoPlaylistManager.instance.IsFriendProfileOpened)
            {
                VideoPlaylistManager.instance.FriendListPanel.SetActive(false);
                VideoPlaylistManager.instance.FriendProfilePanel.SetActive(false);
            }
            else
            {
                VideoPlaylistManager.instance.MyProfilePanel.SetActive(false);
            }
            VideoPlaylistManager.instance.LoadUserVideo(User_Video.result_video_url, User_Video.id);
        }
    }
    public void GetUserImage(int id, string uri)
    {
        Debug.Log("id and URL " + id + ", " + uri);
        localURL = string.Format("{0}/{1}.jpg", Application.persistentDataPath, "" + id);
        if (File.Exists(localURL))
        {
            LoadLocalFile();
        }
        else
        {
            StartCoroutine(GetThumbnail(uri));
        }
    }
    public void LoadLocalFile()
    {
        byte[] bytes;
        bytes = File.ReadAllBytes(localURL);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        Sprite thumbnail = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        Thumbnail_Item.image.sprite = thumbnail;
    }
    IEnumerator GetThumbnail(string uri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
        www.SetRequestHeader("Content-type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.responseCode);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            //UserImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            File.WriteAllBytes(localURL, texture.EncodeToPNG());
            Debug.Log("Image Downloaded and saved!");
            //LoadLocalFile();
            Thumbnail_Item.image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
