using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager instance;

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

    private Text messageText;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        messageText = GetComponentInChildren<Text>();
        animator = GetComponent<Animator>();
    }

    public void ShowMessage(string message)
    {
        StopAllCoroutines();
        messageText.text = message;
        animator.SetTrigger("FadeIn");
        StartCoroutine(HideMessage());
    }

    IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(2);
        animator.SetTrigger("FadeOut");
    }

}
