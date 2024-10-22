using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class FriendsListManager : MonoBehaviour
{
    public static FriendsListManager Instance;

    public GameObject FriendItemPrefab;
    public GameObject FriendRequestItemPrefab;
    public GameObject FriendListParent;
    public GameObject FriendRequestListParent;
    public GameObject FriendListPanel;
    public GameObject FriendRquestListPanel;
    public GameObject FriendRemovalPopup;

    public GameObject[] FriendsButtonBG;
    public GameObject[] FriendRequestsButtonBG;

    public GameObject FreindRequestsCountBG;
    public TextMeshProUGUI FreindRequestsCount;

    public FriendList.FriendsList FriendsList = new FriendList.FriendsList();
    public FriendList.FriendsList friendRquestMainList = new FriendList.FriendsList();

    private GameObject FriendItem;
    private bool GetFriendList = true;
    private bool GetFriendRequestList = true;

    private int PageNumber = 1;
    private int FriendRequestPageNumber = 1;
    public int FriendsPerPage = 20;

    private void Awake()
    {
        if (Instance!=null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        PageNumber = 1;
        FriendRequestPageNumber = 1;
        GetFriendList = true;
        GetFriendRequestList = true;
        //FriendListPanel.SetActive(false);
        //if (PlayerPrefs.GetInt("BottomBarButtonsIndex") == 3)
        //{
        //    OpenFriendsPanel();
        //}
        //FriendsList.users.Clear();
        FriendsList.users = new List<FriendList.User>();
        StartCoroutine(PostGetFriendsList(false,false));

        FriendRequestPageNumber = 1;
        friendRquestMainList.users.Clear();
        StartCoroutine(PostGetFriendRequestList(false));
    }

    public void OpenFriendsPanel()
    {
        FriendsButtonBG[1].SetActive(false);
        FriendRequestsButtonBG[1].SetActive(false);

        FriendsButtonBG[0].SetActive(true);
        FriendRequestsButtonBG[0].SetActive(true);

        FriendRquestListPanel.SetActive(false);
        //if (GetFriendList)
        //{
        //    ClearFriendsList(FriendListParent);
        //    LoadingManager.Instance.Loading.SetActive(true);
        //    FriendListPanel.SetActive(true);
        //    //StartCoroutine(PostGetFriendsList());
        //    CreateList(FriendsList, FriendListParent, FriendItemPrefab);
        //    LoadingManager.Instance.Loading.SetActive(false);
        //    GetFriendList = false;
        //}
        //else
        //{
        //    LoadingManager.Instance.Loading.SetActive(true);
        //    FriendListPanel.SetActive(true);
        //    LoadingManager.Instance.Loading.SetActive(false);
        //}
        CreateFriendList();
    }
    private void CreateFriendList()
    {
        ClearFriendsList(FriendListParent);
        LoadingManager.Instance.Loading.SetActive(true);
        FriendListPanel.SetActive(true);
        //StartCoroutine(PostGetFriendsList());
        CreateList(FriendsList, FriendListParent, FriendItemPrefab);
        LoadingManager.Instance.Loading.SetActive(false);
    }
    public void OpenFriendsRequestPanel()
    {
        FriendsButtonBG[0].SetActive(false);
        FriendRequestsButtonBG[0].SetActive(false);

        FriendsButtonBG[1].SetActive(true);
        FriendRequestsButtonBG[1].SetActive(true);
        if (GetFriendRequestList)
        {
            ClearFriendsList(FriendRequestListParent);
            LoadingManager.Instance.Loading.SetActive(true);
            FriendRquestListPanel.SetActive(true);
            FriendRequestPageNumber = 1;
            friendRquestMainList.users.Clear();
            StartCoroutine(PostGetFriendRequestList(true));
        }
        else
        {
            LoadingManager.Instance.Loading.SetActive(true);
            FriendRquestListPanel.SetActive(true);
            LoadingManager.Instance.Loading.SetActive(false);
        }
    }
    public void ClearFriendsList(GameObject Content)
    {
        if (Content.transform.childCount>0)
        {
            foreach (Transform Child in Content.transform)
            {
                Destroy(Child.gameObject);
            }
        }
    }
    public void SetFriendForRemoval(GameObject Friend)
    {
        FriendItem = Friend;
        FriendRemovalPopup.SetActive(true);
    }
    public void CloseFriendRemovalPopup()
    {
        FriendRemovalPopup.SetActive(false);
    }
    public void RemoveFriendFunc()
    {
        FriendItem.GetComponent<FriendUser>().RemoveFriend();
    }
    public void RefereshFriendList()
    {
        LoadingManager.Instance.Loading.SetActive(true);
        PageNumber = 1;
        if (FriendsList.users.Count>0)
        {
            FriendsList.users.Clear();
        }
        StartCoroutine(PostGetFriendsList(true,true));
        //CreateFriendList();
    }
    public void RefereshFriendRequestList()
    {
        LoadingManager.Instance.Loading.SetActive(true);
        ClearFriendsList(FriendRequestListParent);
        FriendRequestPageNumber = 1;
        friendRquestMainList.users.Clear();
        StartCoroutine(PostGetFriendRequestList(true));
    }
    IEnumerator PostGetFriendsList(bool Createlist,bool RefreshList)
    {

        string requestName = "api/v1/users/get_related_user?page="+ PageNumber;
        string request = AuthManager.BASE_URL + requestName;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(request))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = request.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    LoadingManager.Instance.Loading.SetActive(false);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    LoadingManager.Instance.Loading.SetActive(false);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("friends request " + request);
                    Debug.Log("friends List" + webRequest.downloadHandler.text);
                    FriendList.FriendsList friendsListResponce = JsonUtility.FromJson<FriendList.FriendsList>(webRequest.downloadHandler.text);
                    for (int i = 0; i < friendsListResponce.users.Count; i++)
                    {
                        FriendsList.users.Add(friendsListResponce.users[i]);
                    }
                    if (friendsListResponce.users.Count == FriendsPerPage)
                    {
                        Debug.Log("PageNumber++");
                        PageNumber++;
                        if (!RefreshList)
                        {
                            StartCoroutine(PostGetFriendsList(false,false));
                        }
                        else
                        {
                            StartCoroutine(PostGetFriendsList(true, true));
                        }
                    }
                    if (Createlist)
                    {
                        CreateFriendList();
                    }
                    //CreateList(FriendsList, FriendListParent, FriendItemPrefab);
                    //GetFriendList = false;
                    LoadingManager.Instance.Loading.SetActive(false);
                    //count = friendsList.users.Count;
                    //Debug.Log("Friends Count " + count);
                    //Debug.Log("PageNumber " + PageNumber);
                    //if (count == FriendsPerPage)
                    //{
                    //    FriendListCreator.instance.CreateList(friendsList);
                    //    PageNumber++;
                    //    StartCoroutine(PostGetFriendsList());
                    //}
                    //else
                    //{
                    //    FriendListCreator.instance.CreateList(friendsList);
                    //}

                    //StartCoroutine(WaitFor());
                    break;
            }
        }
    }
    IEnumerator PostGetFriendRequestList(bool Createlist)
    {

        string requestName = "api/v1/users/requests?page=" + FriendRequestPageNumber;
        string request = AuthManager.BASE_URL + requestName;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(request))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = request.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    LoadingManager.Instance.Loading.SetActive(false);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    LoadingManager.Instance.Loading.SetActive(false);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    FriendList.FriendsList friendRquestListResponce = JsonUtility.FromJson<FriendList.FriendsList>(webRequest.downloadHandler.text);
                    if (Createlist)
                    {
                        CreateList(friendRquestListResponce, FriendRequestListParent, FriendRequestItemPrefab);
                    }
                    for (int i = 0; i < friendRquestListResponce.users.Count; i++)
                    {
                        friendRquestMainList.users.Add(friendRquestListResponce.users[i]);
                    }
                    if (friendRquestListResponce.users.Count == FriendsPerPage)
                    {
                        FriendRequestPageNumber++;
                        StartCoroutine(PostGetFriendRequestList(Createlist));
                    }
                    if (friendRquestMainList.users.Count>0)
                    {
                        FreindRequestsCountBG.SetActive(true);
                        FreindRequestsCount.text = "" + friendRquestMainList.users.Count;
                    }
                    GetFriendRequestList = true;
                    LoadingManager.Instance.Loading.SetActive(false);
                    //count = friendsList.users.Count;
                    //Debug.Log("Friends Count " + count);
                    //Debug.Log("PageNumber " + PageNumber);
                    //if (count == FriendsPerPage)
                    //{
                    //    FriendListCreator.instance.CreateList(friendsList);
                    //    PageNumber++;
                    //    StartCoroutine(PostGetFriendsList());
                    //}
                    //else
                    //{
                    //    FriendListCreator.instance.CreateList(friendsList);
                    //}

                    //StartCoroutine(WaitFor());
                    break;
            }
        }
    }
    public void CreateList(FriendList.FriendsList friendsList,GameObject Content,GameObject Prefab)
    {
        if (friendsList.users.Count>0)
        {
            foreach (FriendList.User user in friendsList.users)
            {
                GameObject refUser = Instantiate(Prefab, Content.transform);
                refUser.GetComponent<FriendUser>().Init(user);
            }
        }
        else
        {
            ConsoleManager.instance.ShowMessage("No Friends Found");
        }
    }
}
