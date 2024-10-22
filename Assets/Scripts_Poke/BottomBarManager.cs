using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomBarManager : MonoBehaviour
{
    [Header("Bottum Bar Data")]
    public Button[] BottomBarButtons;
    public Sprite[] BlueSprites;
    public Sprite[] GreySprites;

    public GameObject[] Panels;
    public GameObject VideoActions;
    void Start()
    {
        DisablePanels();
        SetBarButtonSprite(0);
    }
    public void SetBarButtonSprite(int Index)
    {
        for (int i = 0; i < BottomBarButtons.Length; i++)
        {
            BottomBarButtons[i].image.sprite = GreySprites[i];
        }
        BottomBarButtons[Index].image.sprite = BlueSprites[Index];
        Debug.Log("BottomBarButtons Index "+ BottomBarButtons[Index].gameObject.name);
    }
    public void ShowPanel(int Index)
    {
        foreach (GameObject panel in Panels)
        {
            panel.SetActive(false);
        }
        Panels[Index].SetActive(true);
    }
    public void DisablePanels()
    {
        foreach (GameObject panel in Panels)
        {
            panel.SetActive(false);
        }
    }
    public void DisbaleVideoActions(bool Value)
    {
        VideoActions.SetActive(Value);
    }
}
