using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyARPokogramsGenerator : MonoBehaviour
{
    public static MyARPokogramsGenerator instance;
    
    public GameObject myARPokeItem;

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

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid(CurrentUserVideosManager.FetchedVideos);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateGrid(VideosList videosList)
    {
        foreach (Video video in videosList.videos)
        {
            GameObject poke = Instantiate(myARPokeItem, transform);
            poke.GetComponent<UserARVideoItem>().Init(video.result_video_url,video.id,video.thumbnail_url);
        }
    }
 
  
}
