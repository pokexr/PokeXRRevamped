using UnityEngine;
using UnityEngine.UI;

public class ButtonsUIManager : MonoBehaviour
{
    public static ButtonsUIManager instance;

    public GameObject ItemPrefab;
    public GameObject ParentPanel;

    public GameObject ParentPanel2;
    public GameObject itemPrefab2;

    //public Text TotalButtonsText;
    public Text ModelProgressText;
   // public Text itemCollectedForMapScene;
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

    void Start()
    {
        //ModelProgressText.gameObject.SetActive(false);
      //  ItemPrefab.SetActive(true);
        //itemPrefab2.SetActive(true);
    }


    public void GenerateButtons(VideosList videosList)
    {
        ItemPrefab.SetActive(true);
       // itemPrefab2.SetActive(true);
        // TotalButtonsText.text = "Total " + FetchedLocations.videos.Count;
        // itemCollectedForMapScene.text = "Total " + FetchedLocations.locations.Count;
        Debug.Log("Count " + videosList.videos.Count);
        Debug.Log("check1");
        GameObject temp;


        foreach (Video video in videosList.videos)
        {

            Debug.Log("Done videos are here");
            Debug.Log("check2");
            temp = Instantiate(itemPrefab2, ParentPanel2.transform);
            temp.GetComponent<ButtonItem>().Button_user = video;
            temp.GetComponent<ButtonItem>().Init();
            //if (video.status == "done")
            //{
            //    Debug.Log("Done videos are here");
            //    Debug.Log("check2");
            //    temp = Instantiate(itemPrefab2, ParentPanel2.transform);
            //    temp.GetComponent<ButtonItem>().Button_user = video;
            //    temp.GetComponent<ButtonItem>().Init();
            //   // temp.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Collection " + video.id;
            //}
            //else
            //{
            //    Debug.Log("Processing videos are here");
            //    temp = Instantiate(ItemPrefab, ParentPanel.transform);
            //    temp.GetComponent<ButtonItem>().Button_user = video;
            //    temp.GetComponent<ButtonItem>().Init();
            //    temp.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Collection " + video.id;
            //}

           
        }
        Destroy(ItemPrefab);
        Destroy(itemPrefab2);
    }

}
