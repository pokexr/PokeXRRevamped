using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using NatSuite.Recorders.Inputs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using Unity.Collections;
using Logger = UnityEngine.XR.ARFoundation.Samples.Logger;

public class TMRawRecorder : MonoBehaviour
{
    public bool EnableRenderTexture = true;
    public RenderTexture renderTexture;
    public RealtimeClock clock;
    public MP4Recorder recorderRawCamera;
    public MP4Recorder recorderHumanStencil;
    TextureInput textureInputRawCamera;
    TextureInput textureInputHumanStencil;

    public int frameCount;

    public bool IsRecording { get; private set; }

    Texture2D m_CameraTexture;
    XRCpuImage.Transformation m_Transformation = XRCpuImage.Transformation.MirrorY;

    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events.")]
    ARCameraManager m_CameraManager;

    [SerializeField]
    [Tooltip("The AROcclusionManager which will produce human depth and stencil textures.")]
    AROcclusionManager m_OcclusionManager;

    [SerializeField]
    RawImage m_RawCameraImage;

    /// <summary>
    /// Get or set the UI RawImage used to display the image on screen.
    /// </summary>
    public RawImage rawCameraImage
    {
        get => m_RawCameraImage;
        set => m_RawCameraImage = value;
    }

    [SerializeField]
    RawImage m_RawHumanDepthImage;

    /// <summary>
    /// The UI RawImage used to display the image on screen.
    /// </summary>
    public RawImage rawHumanDepthImage
    {
        get => m_RawHumanDepthImage;
        set => m_RawHumanDepthImage = value;
    }

    [SerializeField]
    RawImage m_RawHumanStencilImage;

    /// <summary>
    /// The UI RawImage used to display the image on screen.
    /// </summary>
    public RawImage rawHumanStencilImage
    {
        get => m_RawHumanStencilImage;
        set => m_RawHumanStencilImage = value;
    }

    [SerializeField]
    RawImage m_RawEnvironmentDepthImage;

    /// <summary>
    /// The UI RawImage used to display the image on screen.
    /// </summary>
    public RawImage rawEnvironmentDepthImage
    {
        get => m_RawEnvironmentDepthImage;
        set => m_RawEnvironmentDepthImage = value;
    }

    [SerializeField]
    RawImage m_RawEnvironmentDepthConfidenceImage;

    /// <summary>
    /// The UI RawImage used to display the image on screen.
    /// </summary>
    public RawImage rawEnvironmentDepthConfidenceImage
    {
        get => m_RawEnvironmentDepthConfidenceImage;
        set => m_RawEnvironmentDepthConfidenceImage = value;
    }

    [SerializeField]
    TextMeshProUGUI m_ImageInfo;

    /// <summary>
    /// The UI Text used to display information about the image on screen.
    /// </summary>
    public TextMeshProUGUI imageInfo
    {
        get => m_ImageInfo;
        set => m_ImageInfo = value;
    }

    public Button m_TransformationButton;

    delegate bool TryAcquireDepthImageDelegate(out XRCpuImage image);

    /// <summary>
    /// Cycles the image transformation to the next case.
    /// </summary>
    public void CycleTransformation()
    {
        m_Transformation = m_Transformation switch
        {
            XRCpuImage.Transformation.None => XRCpuImage.Transformation.MirrorX,
            XRCpuImage.Transformation.MirrorX => XRCpuImage.Transformation.MirrorY,
            XRCpuImage.Transformation.MirrorY => XRCpuImage.Transformation.MirrorX | XRCpuImage.Transformation.MirrorY,
            _ => XRCpuImage.Transformation.None
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        if (m_CameraManager == null)
        {
            Debug.LogException(new NullReferenceException(
                $"Serialized properties were not initialized on {name}'s {nameof(CpuImageSample)} component."), this);
            return;
        }

        m_CameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        if (m_CameraManager != null)
            m_CameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        UpdateCameraImage();
        UpdateDepthImage(m_OcclusionManager.TryAcquireHumanDepthCpuImage, m_RawHumanDepthImage);
        UpdateDepthImage(m_OcclusionManager.TryAcquireHumanStencilCpuImage, m_RawHumanStencilImage);
        UpdateDepthImage(m_OcclusionManager.TryAcquireEnvironmentDepthCpuImage, m_RawEnvironmentDepthImage);
        UpdateDepthImage(m_OcclusionManager.TryAcquireEnvironmentDepthConfidenceCpuImage, m_RawEnvironmentDepthConfidenceImage);
    }

    unsafe void UpdateCameraImage()
    {
        // Attempt to get the latest camera image. If this method succeeds,
        // it acquires a native resource that must be disposed (see below).
        if (!m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            return;
        }

        m_ImageInfo.text = string.Format(
            "Image info:\n\twidth: {0}\n\theight: {1}\n\tplaneCount: {2}\n\ttimestamp: {3}\n\tformat: {4}",
            image.width, image.height, image.planeCount, image.timestamp, image.format);

        // Once we have a valid XRCpuImage, we can access the individual image "planes"
        // (the separate channels in the image). XRCpuImage.GetPlane provides
        // low-overhead access to this data. This could then be passed to a
        // computer vision algorithm. Here, we will convert the camera image
        // to an RGBA texture and draw it on the screen.

        // Choose an RGBA format.
        // See XRCpuImage.FormatSupported for a complete list of supported formats.
        const TextureFormat format = TextureFormat.RGBA32;

        if (m_CameraTexture == null || m_CameraTexture.width != image.width || m_CameraTexture.height != image.height)
            m_CameraTexture = new Texture2D(image.width, image.height, format, false);

        // Convert the image to format, flipping the image across the Y axis.
        // We can also get a sub rectangle, but we'll get the full image here.
        var conversionParams = new XRCpuImage.ConversionParams(image, format, m_Transformation);

        // Texture2D allows us write directly to the raw texture data
        // This allows us to do the conversion in-place without making any copies.
        var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
        try
        {
            image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
        }
        finally
        {
            // We must dispose of the XRCpuImage after we're finished
            // with it to avoid leaking native resources.
            image.Dispose();
        }

        // Apply the updated texture data to our texture
        m_CameraTexture.Apply();

        // Set the RawImage's texture so we can visualize it.
        m_RawCameraImage.texture = m_CameraTexture;
        CaptureFrame(m_CameraTexture);
    }

    /// <summary>
    /// Calls <paramref name="tryAcquireDepthImageDelegate"/> and renders the resulting depth image contents to <paramref name="rawImage"/>.
    /// </summary>
    /// <param name="tryAcquireDepthImageDelegate">The method to call to acquire a depth image.</param>
    /// <param name="rawImage">The Raw Image to use to render the depth image to the screen.</param>
    void UpdateDepthImage(TryAcquireDepthImageDelegate tryAcquireDepthImageDelegate, RawImage rawImage, string recordName = null)
    {
        if (tryAcquireDepthImageDelegate(out XRCpuImage cpuImage))
        {
            // XRCpuImages, if successfully acquired, must be disposed.
            // You can do this with a using statement as shown below, or by calling its Dispose() method directly.
            using (cpuImage)
            {
                UpdateRawImage(rawImage, cpuImage, m_Transformation);
            }
        }
        else
        {
            rawImage.enabled = false;
        }
    }

    void UpdateRawImage(RawImage rawImage, XRCpuImage cpuImage, XRCpuImage.Transformation transformation)
    {
        // Get the texture associated with the UI.RawImage that we wish to display on screen.
        var texture = rawImage.texture as Texture2D;

        // If the texture hasn't yet been created, or if its dimensions have changed, (re)create the texture.
        // Note: Although texture dimensions do not normally change frame-to-frame, they can change in response to
        //    a change in the camera resolution (for camera images) or changes to the quality of the human depth
        //    and human stencil buffers.
        if (texture == null || texture.width != cpuImage.width || texture.height != cpuImage.height)
        {
            texture = new Texture2D(cpuImage.width, cpuImage.height, cpuImage.format.AsTextureFormat(), false);
            rawImage.texture = texture;
        }

        // For display, we need to mirror about the vertical access.
        var conversionParams = new XRCpuImage.ConversionParams(cpuImage, cpuImage.format.AsTextureFormat(), transformation);

        // Get the Texture2D's underlying pixel buffer.
        var rawTextureData = texture.GetRawTextureData<byte>();

        // Make sure the destination buffer is large enough to hold the converted data (they should be the same size)
        Debug.Assert(rawTextureData.Length == cpuImage.GetConvertedDataSize(conversionParams.outputDimensions, conversionParams.outputFormat),
            "The Texture2D is not the same size as the converted data.");

        // Perform the conversion.
        cpuImage.Convert(conversionParams, rawTextureData);

        // "Apply" the new pixel data to the Texture2D.
        texture.Apply();
        

        // Make sure it's enabled.
        rawImage.enabled = true;
    }

    [ContextMenu("Get Image Async")]
    public void GetImageAsync()
    {
        Logger.Log("TMRawRecorder:GetImageAsync: Getting image async...");
        // Acquire an XRCpuImage
        if (m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            Debug.Log("Image acquired");

            SetCurrentImageSize(image.width, image.height);

            // Perform async conversion
            image.ConvertAsync(new XRCpuImage.ConversionParams
            {
                // Get the full image
                inputRect = new RectInt(0, 0, image.width, image.height),

                // Downsample by 2
                outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

                // Color image format
                outputFormat = TextureFormat.RGB24,

                // Flip across the Y axis
                transformation = XRCpuImage.Transformation.MirrorY

                // Call ProcessImage when the async operation completes
            }, ProcessImage);

            // It is safe to dispose the image before the async operation completes
            image.Dispose();
        }
    }

    public void SetCurrentImageSize(int width, int height)
    {
        Logger.Log("TMRawRecorder:SetCurrentImageSize: Setting image size to " + width + "x" + height + "...");
        RecordWidth = width;
        RecordHeight = height;
    }

    void ProcessImage(
        XRCpuImage.AsyncConversionStatus status,
        XRCpuImage.ConversionParams conversionParams,
        NativeArray<byte> data)
    {
        if (status != XRCpuImage.AsyncConversionStatus.Ready)
        {
            Debug.LogErrorFormat("Async request failed with status {0}", status);
            return;
        }

        // Copy to a Texture2D, pass to a computer vision algorithm, etc
        Debug.Log("TMRawRecorder:ProcessImage: Image data received. Do something with it!");

        // Data is destroyed upon return. No need to dispose
    }

    public int RecordWidth = 1080;
    public int RecordHeight = 1920;
    public int RecordFps = 30;
    public int VideoSampleRate = 1000000;
    public int AudioChannelCount = 2;
    public int AudioSampleRate = 44100;

    [ContextMenu("StartRecording")]
    public void StartRecording()
    {
        // Set up proper recording resolutions based on CPU image
        GetImageAsync();

        IsRecording = true;
        frameCount = 0;
        clock = new RealtimeClock();
        recorderRawCamera = new MP4Recorder(RecordWidth, RecordHeight, RecordFps, VideoSampleRate);
        recorderHumanStencil = new MP4Recorder(RecordWidth, RecordHeight, RecordFps, VideoSampleRate);
        // textureInputRawCamera = new TextureInput(recorderRawCamera);
        // textureInputHumanStencil = new TextureInput(recorderHumanStencil);
        Debug.Log("Started recording");
    }

    public void CaptureFrame(Texture2D texture)
    {
        if (!IsRecording) return;

        recorderHumanStencil.CommitFrame(texture.GetPixels32(), clock.timestamp);
        frameCount++;
    }

    public void CaptureDepthFrame(Texture2D texture)
    {
        if (!IsRecording) return;

        recorderHumanStencil.CommitFrame(texture.GetPixels32(), clock.timestamp);
    }

    [ContextMenu("StopRecording")]
    public async void StopRecording()
    {
        IsRecording = false;
        var recordRawCameraPath = await recorderRawCamera.FinishWriting();
        var recordHumanStencilPath = await recorderHumanStencil.FinishWriting();

        CurrentRawSharePath = recordHumanStencilPath;
        CurrentRawStencilPath = recordHumanStencilPath;

        Debug.Log("Stopped recording, " + frameCount + " frames at cam path: " + recordRawCameraPath + " stencil path: " + recordHumanStencilPath);
    }

    public void OnShare()
    {
        StartCoroutine(ShareCurrentRawVideo());
    }

    public string CurrentRawSharePath;
    public string CurrentRawStencilPath;

    private IEnumerator ShareCurrentRawVideo()
    {
        yield return new WaitForEndOfFrame();
        new NativeShare().AddFile(CurrentRawSharePath)
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();
    }

    /*
    public void ShareVideoSync(string videoPath)
    {
        Debug.Log("Sharing video: " + videoPath);

        new NativeShare().AddFile(videoPath)
            .SetCallback((result, shareTarget) =>
            {
                Logger.Log("Share result: " + result + ", selected app: " + shareTarget);
            }).Share();

        Debug.Log("Share complete.");

        return;
    }
    */
}
