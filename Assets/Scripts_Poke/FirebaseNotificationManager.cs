using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Messaging;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseNotificationManager : MonoBehaviour
{

    private bool firebaseInitialized = false;

    void Start()
    {
        // Check if Firebase is initialized
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase is ready to use
                firebaseInitialized = true;
                Debug.Log("Firebase Initialized!");

                // Request FCM token
                RequestFCMToken();
            }
            else
            {
                Debug.LogError("Firebase could not be initialized: " + dependencyStatus.ToString());
            }
        });
    }

    private void RequestFCMToken()
    {
        if (firebaseInitialized)
        {
            FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        // Called when a new FCM token is received or refreshed
        string fcmToken = token.Token;
        Debug.Log("FCM Token: " + fcmToken);

        StartCoroutine(SendRegistrationToken(fcmToken));
        // You can send this token to your server for handling push notifications
        // Remember to remove listeners if they are no longer needed
        FirebaseMessaging.TokenReceived -= OnTokenReceived;
        FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        // Called when a new message is received while the app is in the foreground
        Debug.Log("FCM Message Received: " + e.Message.Notification.Body);
    }

    //FirebaseApp app;
    //// Start is called before the first frame update
    //void Start()
    //{

    //    if (Application.platform == RuntimePlatform.IPhonePlayer)
    //    {
    //        // Request push notification permission from the user
    //        UnityEngine.iOS.NotificationServices.RegisterForNotifications(
    //            UnityEngine.iOS.NotificationType.Alert |
    //            UnityEngine.iOS.NotificationType.Badge |
    //            UnityEngine.iOS.NotificationType.Sound
    //        );
    //    }

    //    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
    //        var dependencyStatus = task.Result;
    //        if (dependencyStatus == DependencyStatus.Available)
    //        {
    //            // Create and hold a reference to your FirebaseApp,
    //            // where app is a Firebase.FirebaseApp property of your application class.
    //            app = FirebaseApp.DefaultInstance;
    //            Debug.Log("Calling FCM");
    //            // Set a flag here to indicate whether Firebase is ready to use by your app.
    //            InitializeFCM();
    //        }
    //        else
    //        {
    //            Debug.LogError(string.Format(
    //              "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
    //            // Firebase Unity SDK is not safe to use here.
    //        }
    //    });

    //    Debug.Log("start FCM");
    //    Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
    //    Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    //    InitializeFCM();
    //}
    //private void OnApplicationFocus(bool hasFocus)
    //{
    //    if (hasFocus)
    //    {
    //        // Check if user granted permission
    //        if (Application.platform == RuntimePlatform.IPhonePlayer &&
    //            UnityEngine.iOS.NotificationServices.deviceToken != null)
    //        {
    //            string deviceToken = System.BitConverter.ToString(UnityEngine.iOS.NotificationServices.deviceToken);
    //            Debug.Log("Device Token: " + deviceToken);


    //        }
    //        else
    //        {
    //            Debug.Log("Push notification permission denied or not available on this platform.");
    //        }
    //    }
    //}
    //public void InitializeFCM()
    //{
    //    StartCoroutine(GetTokenAsync());
    //}

    //private IEnumerator GetTokenAsync()
    //{
    //    var task = Firebase.Messaging.FirebaseMessaging.GetTokenAsync();

    //    while (!task.IsCompleted)
    //        yield return new WaitForEndOfFrame();
    //    Debug.Log("GET TOKEN ASYNC " + task.Result);
    //    if (task.Result != null || task.Result != "")
    //    {
    //        StartCoroutine(SendRegistrationToken(task.Result));
    //    }
    //}


    //public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    //{
    //    Debug.Log("Received Registration Token: " + token.Token);
    //    if (token.Token != null || token.Token != "")
    //    {
    //        StartCoroutine(SendRegistrationToken(token.Token));
    //    }
    //}

    //public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    //{
    //    Debug.Log("Received a new message from: " + e.Message.From);
    //}

    IEnumerator SendRegistrationToken(string RegistrationToken)
    {
        WWWForm form = new WWWForm();
        form.AddField("device_registration_id", RegistrationToken);

        string requestName = "api/v1/users";
        Debug.Log("Send Device URL : "+ AuthManager.BASE_URL + requestName);
        using (UnityWebRequest www = UnityWebRequest.Post(AuthManager.BASE_URL + requestName, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                ConsoleManager.instance.ShowMessage("Device ID Updating Error!");
                Debug.Log(www.error);
                LoadingManager.Instance.Loading.SetActive(false);
            }
            else
            {
                Debug.Log("chat " + www.downloadHandler.text);
                SendDeviceID.Root DeviceIDResponce = JsonUtility.FromJson<SendDeviceID.Root>(www.downloadHandler.text);
                if (DeviceIDResponce.user.device_registration_id.ToString()=="")
                {
                    ConsoleManager.instance.ShowMessage("Notification Error!");
                }
                Debug.Log("Uploaded token " + RegistrationToken);
                LoadingManager.Instance.Loading.SetActive(false);
            }
        }
    }
}
