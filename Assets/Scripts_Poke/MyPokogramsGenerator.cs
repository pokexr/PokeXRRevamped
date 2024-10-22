using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MyPokogramsGenerator : MonoBehaviour
{
    public static MyPokogramsGenerator instance;
    public GameObject refRect;

    public GameObject myPokeItem;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateGrid(VideosList videosList)
    {
        foreach (Video video in videosList.videos)
        {
            GameObject poke = Instantiate(myPokeItem,transform);
            poke.GetComponent<UserVideoItem>().Init( video.result_video_url,video.id,video.thumbnail_url);
        }
        AdjustSize();
    }

    void AdjustSize()
    {
        float width = refRect.GetComponent<RectTransform>().rect.width;
        width -= 35;
        width /= 2;

        Vector2 newSize = new(width, width);
        GetComponent<GridLayoutGroup>().cellSize = newSize;
    }
}
