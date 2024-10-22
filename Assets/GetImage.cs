using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetImage : MonoBehaviour
{
	public static GetImage instance;

	public GameObject ImagePanel;
	//private Sprite DownloadedImage;
	public Sprite Defualt_Image;
	public Image SelectedImage;
	public Image ProfileImage;

	public bool ImageSelected = false;
	//public Texture2D CroppedImage;
	public Texture2D FinalImage;

	[Header("CROP SCREEN")]
	public RawImage croppedImageHolder;
	public Text croppedImageSize;
	public Toggle ovalSelectionInput, autoZoomInput;
	public InputField minAspectRatioInput, maxAspectRatioInput;

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
		ImageSelected = false;
	}
    public void PickImage()
	{
		//byte[] imageBytes = testImage.EncodeToJPG();
		//StartCoroutine(Upload(imageBytes));
		PickImage(512);
	}
	[System.Obsolete]
	private void PickImage(int maxSize)
	{
		NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
		{
			Debug.Log("Image path: " + path);
			if (path != null)
			{
				// Create Texture from selected image
				Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}
				//texture
				ImagePanel.SetActive(true);
				SelectedImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
				SelectedImage.preserveAspect = true;
				//Invoke(nameof(Crop), 1);
				//Crop(texture);
				StartCoroutine(CropCoroutine(texture));
            }
		});

		Debug.Log("Permission result: " + permission);
	}
	IEnumerator CropCoroutine(Texture2D CropTexture)
	{
		yield return new WaitForSeconds(1);
		Crop(CropTexture);
	}
	public void Crop(Texture2D SelectedTexture)
	{
		// If image cropper is already open, do nothing
		if (ImageCropper.Instance.IsOpen)
			return;

		StartCoroutine(TakeScreenshotAndCrop(SelectedTexture));
	}
	private IEnumerator TakeScreenshotAndCrop(Texture2D SelectedTexture)
	{
		yield return new WaitForEndOfFrame();

		bool ovalSelection = ovalSelectionInput.isOn = false;
		bool autoZoom = autoZoomInput.isOn;


		float minAspectRatio, maxAspectRatio;
		if (!float.TryParse(minAspectRatioInput.text, out minAspectRatio))
			minAspectRatio = 0f;
		if (!float.TryParse(maxAspectRatioInput.text, out maxAspectRatio))
			maxAspectRatio = 0f;
		ImageCropper.Instance.Show(SelectedTexture, (bool result, Texture originalImage, Texture2D croppedImage) =>
		{
			if (result)
			{
				croppedImageHolder.enabled = true;
				croppedImageHolder.texture = croppedImage;

				Vector2 size = croppedImageHolder.rectTransform.sizeDelta;
				if (croppedImage.height <= croppedImage.width)
					size = new Vector2(400f, 400f * (croppedImage.height / (float)croppedImage.width));
				else
					size = new Vector2(400f * (croppedImage.width / (float)croppedImage.height), 400f);
				croppedImageHolder.rectTransform.sizeDelta = size;

				Texture2D dest = new Texture2D(croppedImageHolder.texture.width, croppedImageHolder.texture.height, TextureFormat.RGBA32, false);
				dest.Apply(false);
				Graphics.CopyTexture(croppedImageHolder.texture, dest);

				FinalImage = dest;
				croppedImageSize.enabled = true;
			}
			else
			{
				croppedImageHolder.enabled = false;
				croppedImageSize.enabled = false;
			}
		},


		settings: new ImageCropper.Settings()
		{
			ovalSelection = ovalSelection,
			autoZoomEnabled = autoZoom,
			imageBackground = Color.clear, // transparent background
			selectionMinAspectRatio = minAspectRatio,
			selectionMaxAspectRatio = maxAspectRatio

		},
		croppedImageResizePolicy: (ref int width, ref int height) =>
		{
			//uncomment lines below to save cropped image at half resolution
			width /= 2;
			height /= 2;
		}
		);


	}
    private void Update()
    {
		if (ImageCropper.Instance.counter == 1)
		{
			ImagePanel.SetActive(false);
			FinalImage = duplicateTexture(FinalImage);
			ProfileImage.sprite = Sprite.Create(FinalImage, new Rect(0.0f, 0.0f, FinalImage.width, FinalImage.height), new Vector2(0.5f, 0.5f), 100.0f);
			ImageSelected = true;
			//UpdateProfile();
        }
	}
	Texture2D duplicateTexture(Texture2D source)
	{
		RenderTexture renderTex = RenderTexture.GetTemporary(
					source.width,
					source.height,
					0,
					RenderTextureFormat.Default,
					RenderTextureReadWrite.Linear);

		Graphics.Blit(source, renderTex);
		RenderTexture previous = RenderTexture.active;
		RenderTexture.active = renderTex;
		Texture2D readableText = new Texture2D(source.width, source.height);
		readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
		readableText.Apply();
		RenderTexture.active = previous;
		RenderTexture.ReleaseTemporary(renderTex);
		return readableText;
	}
}
