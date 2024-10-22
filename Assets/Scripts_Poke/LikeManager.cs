using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikeManager : MonoBehaviour
{
    public Animator LikeAnimator;

    public void DisplayLike()
    {
        LikeAnimator.ResetTrigger("Like");
        LikeAnimator.SetTrigger("Like");
        LoadingManager.Instance.Loading.SetActive(true);
        Debug.Log("Follow friend ID "+ (VideoPlaylistManager.instance.CurrentUserID));
        StartCoroutine(OtherUserProfileManager.Instance.PostSendFollowrequest(VideoPlaylistManager.instance.CurrentUserID,"Poked","Poked Error", "Already Poked"));
    }
}
