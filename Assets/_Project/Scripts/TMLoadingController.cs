using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TMLoadingController : MonoBehaviour
{
    public string sceneToLoad;

    public Animator loadingUiAnimCtrl;

    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private ProgressBar loadingBar;

    public float currentPercent = 0f;

    [Range(1, 50)]
    public int LoadSpeed = 15;

    public UnityEvent onLoadStart;

    public UnityEvent onLoadComplete;

    void Start()
    {
        InitializeLoader();
    }

    void InitializeLoader()
    {
        loadingBar.currentPercent = 1f; // set current percent
        loadingBar.speed = LoadSpeed; // set speed
        loadingBar.invert = false; // 100 to 0
        loadingBar.restart = false; // restart when it's 100
        loadingBar.isOn = false; // enable or disable counting

        onLoadStart.AddListener(HandleLoadStart);
        onLoadComplete.AddListener(HandleLoadComplete);

        Debug.Log("TMLoadingController: Initialized.");
        onLoadStart.Invoke();
    }

    [ContextMenu("Start Loading")]
    public void StartLoading()
    {
        onLoadStart.Invoke();
    }

    public void HandleValueChanged(float value)
    {
        //Debug.Log("TMLoadingController: " + value);

        currentPercent = value;
        loadingUiAnimCtrl.SetFloat("progress", value);
        

        if (loadingText != null)
            loadingText.text = "Loading... " + value.ToString() + "%";

        if (currentPercent >= 100f)
        {
            onLoadComplete.Invoke();
        }
    }

    public void HandleLoadStart()
    {   
        loadingBar.isOn = true;
        loadingUiAnimCtrl.SetBool("idle", false);
        loadingUiAnimCtrl.SetBool("fadeIn", true);
        Debug.Log("TMLoadingController: Loading Start");
    }

    public void HandleLoadComplete()
    {
        loadingBar.isOn = false;
        loadingUiAnimCtrl.SetBool("fadeIn", false);
        loadingUiAnimCtrl.SetBool("fadeOut", true);
        Debug.Log("TMLoadingController: Loading Complete");

        StartCoroutine(TMSceneController.Instance.LoadSceneAsync(sceneToLoad));
    }
}
