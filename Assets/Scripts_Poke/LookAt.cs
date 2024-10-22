using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LookAt : MonoBehaviour
{
    //public  Camera myCamera;
    //public Transform Target;
    //public float speed = 1f;

    private Camera ARCamera;

    private void Start()
    {
        ARCamera = Camera.main;
    }
    private void Update()
    {
        var lookPos = ARCamera.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);
        //Vector3 targetPosition = new Vector3(ARCamera.transform.position.x, 0, ARCamera.transform.position.y);
        //transform.LookAt(targetPosition);




        //transform.LookAt(new Vector3(transform.position.x, transform.position.y, myCamera.transform.position.y));
        //transform.LookAt(myCamera.transform);
    }
}

    //public void StartRotating()
    //{
    //    if (lookCoroutine != null)
    //    {
    //        StopCoroutine(lookCoroutine);
    //    }
    //    lookCoroutine = StartCoroutine(LookatCamera());
    //}

    //IEnumerator LookatCamera()
    //{
    //    Quaternion lookRoattion = Quaternion.LookRotation(Target.position - transform.position);

    //    float time = 0;

    //    while (time < 1)
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, lookRoattion, time);

    //        time += Time.deltaTime + speed;
    //        yield return null;

    //    }

    //}



    //void Update()
    //{

    //    //var clone = Instantiate(newMissile,transform.position, Quaternion.identity);
    //    //clone.parent = transform;
    //}
    //    //var lookPos = target.position - transform.position;
        //lookPos.y = 0;
        //var rotation = Quaternion.LookRotation(lookPos);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);


        //// Rotate the camera every frame so it keeps looking at the target
        //transform.LookAt(target);

        //// Same as above, but setting the worldUp parameter to Vector3.left in this example turns the camera on its side
        //transform.LookAt(target, Vector3.up);
   // }

