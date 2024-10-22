using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{
	public Image image;
	public Image UserImage;
	private string localURL;

	private void Start()
	{
		image = GetComponent<Image>();
	}

	public void ResetImage()
	{
		image.sprite = transform.parent.GetComponent<Image>().sprite;
	}

	public void Init(int id, string uri)
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
		image.sprite = thumbnail;
		UserImage.sprite = thumbnail;
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
			//image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			File.WriteAllBytes(localURL, texture.EncodeToPNG());
			Debug.Log("Image Downloaded and saved!");
			LoadLocalFile();

		}
	}
}
