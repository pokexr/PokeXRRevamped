using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;
using Firebase;
using System.Linq;

public class RealtimeDBManager : MonoBehaviour
{
    public bool IsChatSceneOpened = true;
    public GameObject ChatIndicator;

    DatabaseReference reference;

    private int ChatUpdateCounter = 0;
    private List<int> numbers = new List<int>();
    private const string PlayerPrefsKey = "SavedNumbers";

    public static int ChatUnreadedMsgsCheck
    {
        set
        {
            PlayerPrefs.SetInt("ChatUnreadedMsgsCheck", value);
            Debug.Log(ChatUnreadedMsgsCheck);
        }
        get
        {
            return PlayerPrefs.GetInt("ChatUnreadedMsgsCheck");
        }
    }

    private void Start()
    {
        //DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        //reference.ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("chat").Child(ProfileManager.UserID.ToString()).ValueChanged += HandleUpdateScore;
        ChatUpdateCounter = 0;
    }
    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        // Access the updated data
        if (args.Snapshot != null && args.Snapshot.Value != null)
        {
            // Parse and use the updated value
            string updatedValue = args.Snapshot.Value.ToString();
            Debug.Log("Value changed: " + updatedValue);
        }
    }
    public void HandleUpdateScore(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot snapshot = args.Snapshot;
        Debug.Log("Snapshot value " + snapshot.Value);
        Debug.Log("Snapshot Key " + snapshot.Key);
        Debug.Log("Snapshot sender_id Key " + snapshot.Child("sender_id").Key);
        Debug.Log("Snapshot sender_id Value " + snapshot.Child("sender_id").Value);
        Debug.Log("Snapshot sender_id " + snapshot.Child("sender_id"));
        //foreach(var i in snapshot.Key)
        //{
        //    Debug.Log(""+i);
        //}
        //ScoreText.text = snapshot.Value.ToString();
        Debug.Log("value chnageddd");
        if (IsChatSceneOpened)
        {
            if (ChatManager.Instance.IsPanelOpened == true)
            {
                ChatManager.Instance.RefreshChat();
            }
        }
        else
        {
            if (ChatUpdateCounter > 0)
            {
                if (ChatIndicator != null)
                {
                    Debug.Log("Chat value changed");
                    ChatIndicator.SetActive(true);
                    ChatUnreadedMsgsCheck = 1;
                }
            }
            else
            {
                ChatUpdateCounter++;
            }
            if (ChatUnreadedMsgsCheck>0)
            {
                ChatIndicator.SetActive(true);
            }
            Debug.Log("ChatUpdateCounter " + ChatUpdateCounter);
        }
    }
    public void SetValueOfUnreadMsgs()
    {
        ChatUnreadedMsgsCheck = 0;
    }
    void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        Debug.Log("HandleChildAdded " + args.Snapshot);/////
    }

    void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        Debug.Log("HandleChildChanged " + args.Snapshot);
    }

    void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        Debug.Log("HandleChildRemoved " + args.Snapshot);
    }

    void HandleChildMoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        Debug.Log("HandleChildMoved " + args.Snapshot);
    }
    public void UpdateScore()
    {
        FirebaseDatabase.DefaultInstance.GetReference("counter").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("task :: " + task);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int value = int.Parse(Convert.ToString(snapshot.Value));
                value++;
                reference.Child("counter").SetValueAsync(value);
            }
        });
    }
}
