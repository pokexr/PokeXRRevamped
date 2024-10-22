using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class InputUIManager : MonoBehaviour
{
    public static InputUIManager instance;
    [Header("Panel")]
    public GameObject SignupPanel;
    public GameObject SigninPanel;
    public GameObject ColorSelectionPanel;
    public  GameObject LoadingPanel;

    [Header("Password Reset Area")]
    public GameObject PasswordResetPanel;
    public InputField PR_emailInput;

    [Header("Signin Input Area")]
    public InputField s_emailInput;
    public InputField s_passwordInput;

    [Header("SignUp Input Area")]
    //public Animator animator;
    public InputField UserNameInput;
    public InputField nameInput;
    public InputField emailInput;
    public InputField DOBInput;
    public InputField GenderInput;
    public InputField passwordInput;
    public InputField confirmInput;
    public Text ColorCode;
    private const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";


    private const string MatchDOBpattern = "^[0-9]{1,2}\\-[0-9]{1,2}\\-[0-9]{4}$";
  

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void Next()
    {
        if (nameInput.text == "")
        {
            ConsoleManager.instance.ShowMessage("Name is Empty!");
            return;
        }
        if (emailInput.text == "")
        {
            ConsoleManager.instance.ShowMessage("Email is Empty!");
            return;
        }
        if (!ValidateEmail(emailInput.text))
        {
            ConsoleManager.instance.ShowMessage("Email not Valid!");
            return;
        }
    }

    public void SignInUser()
    {

        if (s_emailInput.text == "")
        {
            ConsoleManager.instance.ShowMessage("Email is Empty!");
            return;
        }
        if (s_passwordInput.text == "")
        {
            ConsoleManager.instance.ShowMessage("Password is Empty!");
            return;
        }

        if (s_passwordInput.text.Length < 8)
        {
            ConsoleManager.instance.ShowMessage("Password is not 8 characters!");
            return;
        }

        //if (!ValidateEmail(s_emailInput.text))
        //{
        //    ConsoleManager.instance.ShowMessage("Email not Valid!");
        //    return;
        //}
        LoadingPanel.SetActive(true);
        ProfileManager.UserPassword = s_passwordInput.text;
        AuthManager.instance.LoginUser(s_emailInput.text.ToLower(), s_passwordInput.text);
    }
    public void SendPasswordResetEmail()
    {
        if (PR_emailInput.text == "")
        {
            ConsoleManager.instance.ShowMessage("Email is Empty!");
            return;
        }
        if (!ValidateEmail(PR_emailInput.text))
        {
            ConsoleManager.instance.ShowMessage("Email not Valid!");
            return;
        }
        LoadingPanel.SetActive(true);
        AuthManager.instance.SendPassResetEmail(PR_emailInput.text.ToLower());
    }
    public void CreateUser()
    {
        LoadingPanel.SetActive(true);
        AuthManager.instance.CreateUser(nameInput.text, emailInput.text.ToLower(), passwordInput.text, UserNameInput.text.ToLower(), "", "",ColorCode.text);
    }

    private bool ValidateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, MatchEmailPattern);
        else
            return false;
    }

    private bool ValidateDOB(string DOB)
    {
        if (DOBInput!= null)
            return Regex.IsMatch(DOB, MatchDOBpattern);
        else
            return false;
    }


    public void SignUpPanelValidation()
    {
        if (UserNameInput.text == "")
        {
            ConsoleManager.instance.ShowMessage("UserName is Empty!");
            return;
        }
        if (nameInput.text == "")
        {
            ConsoleManager.instance.ShowMessage("Name is Empty!");
            return;
        }
        if (emailInput.text == "")
        {
            ConsoleManager.instance.ShowMessage("Email is Empty!");
            return;
        }

        //if (DOBInput.text == "")
        //{
        //    ConsoleManager.instance.ShowMessage("date of birth is Empty!");
        //    return;
        //}
        //if (GenderInput.text == "")
        //{
        //    ConsoleManager.instance.ShowMessage("Gender is Empty!");
        //    return;
        //}

        if (passwordInput.text == "")
        {
            ConsoleManager.instance.ShowMessage("Password is Empty!");
            return;
        }
        if (confirmInput.text == "")
        {
            ConsoleManager.instance.ShowMessage("Confirm Password Password is Empty!");
            return;
        }
        if (confirmInput.text != passwordInput.text)
        {
            ConsoleManager.instance.ShowMessage("Password Doesn't match");
            return;
        }
        if (passwordInput.text.Length < 8)
        {
            ConsoleManager.instance.ShowMessage("Password is not 8 characters!");
            return;
        }

        if (!ValidateEmail(emailInput.text))
        {
            ConsoleManager.instance.ShowMessage("Email not Valid!");
            return;
        }

        //if (!ValidateDOB(DOBInput.text))
        //{
        //    ConsoleManager.instance.ShowMessage("Date of Birth not Valid! use(-) between Dates");
        //    return;
        //}
        SignupPanel.SetActive(false);
        SigninPanel.SetActive(false);
        ColorSelectionPanel.SetActive(true);
    }

}
