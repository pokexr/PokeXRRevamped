using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;
    public static Root root;
    public static bool DebugMode = false;

    public string LoginSuccessSceneName = "MainScene";

    public static string Token
    {
        set
        {
            PlayerPrefs.SetString("Token", value);
            Debug.Log(Token);
        }
        get
        {
            return PlayerPrefs.GetString("Token");
        }
    }

    public static string DebugToken
    {
        set
        {
            PlayerPrefs.SetString("DebugToken", value);
            Debug.Log(Token);
        }
        get
        {
            return PlayerPrefs.GetString("DebugToken");
        }
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("IsUserLogged")==1)
        {
            InputUIManager.instance.LoadingPanel.SetActive(true);
            InputUIManager.instance.s_emailInput.text = ProfileManager.UserEmail;
            InputUIManager.instance.s_passwordInput.text = EncryptPassword();
            LoginUser(ProfileManager.UserEmail, ProfileManager.UserPassword);
        }
    }
    
    private string EncryptPassword()
    {
        string NewPass = "";
        for (int i = 0; i < ProfileManager.UserPassword.Length; i++)
        {
            NewPass.Append('*');
        }
        return NewPass;
    }
    public static string BASE_URL = "https://api.pokexr.com/";

    public void CreateUser(string name, string email, string password, string username, string gender, string birthday, string Color)
    {
        Debug.Log("Creating User");
        StartCoroutine(CreateUserStCoroutine(name,email,password, username, gender, birthday,Color));
    }
    public void SendPassResetEmail(string email)
    {
        StartCoroutine(PasswordReset(email));
    }
    public void LoginUser(string username, string password)
    {
        Debug.Log("AuthManager:LoginUser: " + username);
        StartCoroutine(LoginUserCoroutine(username, password));
    }

    IEnumerator PasswordReset(string email)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);

        string requestName = "api/v1/auth/send_forgot_password";

        using (UnityWebRequest www = UnityWebRequest.Post(BASE_URL + requestName, form))
        {

            yield return www.SendWebRequest();


            if (www.isHttpError)
            {
                if (www.responseCode == 404)
                {
                    ConsoleManager.instance.ShowMessage("Invalid Email or Account not found");
                    InputUIManager.instance.LoadingPanel.SetActive(false);
                }
                else
                {
                    ConsoleManager.instance.ShowMessage(www.error);
                    InputUIManager.instance.LoadingPanel.SetActive(false);
                    Debug.Log("isHttpError " + www.error);
                }
            }
            else if (www.isNetworkError)
            {
                ConsoleManager.instance.ShowMessage(www.error);
                InputUIManager.instance.LoadingPanel.SetActive(false);
                Debug.Log("isNetworkError" + www.error);
            }
            else
            {
                ConsoleManager.instance.ShowMessage("Kindly check your email.");
                InputUIManager.instance.PasswordResetPanel.SetActive(false);
                InputUIManager.instance.LoadingPanel.SetActive(false);
            }
        }
    }
    IEnumerator CreateUserStCoroutine(string name, string email, string password, string username, string gender, string birthday, string Color)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("color", Color);
        form.AddField("gender", gender);
        form.AddField("birthday", birthday);

        string requestName = "api/v1/auth/sign_up";
        using (UnityWebRequest www = UnityWebRequest.Post(BASE_URL + requestName, form))
        {
            yield return www.SendWebRequest();

            if (www.isHttpError)
            {
                if (www.responseCode == 403)
                {
                    ConsoleManager.instance.ShowMessage("Email or username already taken");
                    InputUIManager.instance.LoadingPanel.SetActive(false);
                    Debug.Log(www.error);
                }
                else
                {
                    ConsoleManager.instance.ShowMessage(www.error);
                    InputUIManager.instance.LoadingPanel.SetActive(false);
                    Debug.Log("isHttpError "+www.error);
                }
            }
            else if (www.isNetworkError)
            {
                ConsoleManager.instance.ShowMessage(www.error);
                InputUIManager.instance.LoadingPanel.SetActive(false);
                Debug.Log("isNetworkError "+www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                
                // Trigger login success transition
                StartCoroutine(TMSceneController.Instance.LoadSceneAsync(LoginSuccessSceneName));
            }
        }
    }
    IEnumerator LoginUserCoroutine(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        string requestName = "api/v1/auth/sign_in";

        using (UnityWebRequest www = UnityWebRequest.Post(BASE_URL + requestName, form))
        {
            
            yield return www.SendWebRequest();

            
            if (www.isHttpError)
            {
                if (www.responseCode == 403)
                {
                    ConsoleManager.instance.ShowMessage("Invalid Email or Password");
                    InputUIManager.instance.LoadingPanel.SetActive(false);
                }
                else
                {
                    ConsoleManager.instance.ShowMessage(www.error);
                    InputUIManager.instance.LoadingPanel.SetActive(false);
                    Debug.Log("isHttpError " + www.error);
                }
            }
            else if (www.isNetworkError)
            {
                ConsoleManager.instance.ShowMessage(www.error);
                InputUIManager.instance.LoadingPanel.SetActive(false);
                Debug.Log("isNetworkError" + www.error);
            }
            else
            {
                OnSuccess(www.downloadHandler.text);
                SceneManager.LoadScene(LoginSuccessSceneName);
                Debug.Log("Authorized: Token is " + root.meta.token);
            }
        }
    }

    private void OnSuccess(string json)
    {
        Debug.Log("Json "+json);
        root = JsonUtility.FromJson<Root>(json);
        Debug.Log("Login Success Function");
        Debug.Log(root.meta.token);
        Token = root.meta.token;
        
        ProfileManager.FullName = root.user.name;
        ProfileManager.UserName = root.user.username;
        ProfileManager.UserID = root.user.id;
        ProfileManager.UserEmail = root.user.email;
        ProfileManager.UserImageUrl = root.user.image_url;
        //DateTime = DateTimeOffset.Parse(root.user.birthday).DateTime;
        ProfileManager.UserAge = root.user.birthday.ToString();
        PlayerPrefs.SetInt("IsUserLogged",1);
        //DateTime Birthday = DateTimeOffset.Parse(root.user.birthday.ToString()).DateTime;
        //ProfileManager.UserAge = GetAge(Birthday).ToString();
        Debug.Log("ProfileManager.UserAge " + ProfileManager.UserAge);
        Debug.Log("UserEmail " + ProfileManager.UserEmail);
        PlayerPrefs.SetInt("BottomBarButtonsIndex",0);
    }
    public static int GetAge(DateTime birthDate)
    {
        DateTime n = DateTime.Now; // To avoid a race condition around midnight
        int age = n.Year - birthDate.Year;

        if (n.Month < birthDate.Month || (n.Month == birthDate.Month && n.Day < birthDate.Day))
            age--;

        return age;
    }
}