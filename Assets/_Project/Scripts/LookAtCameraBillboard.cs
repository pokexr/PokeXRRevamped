using UnityEngine;

public class LookAtCameraBillboard : MonoBehaviour
{
    void LateUpdate()
    {
        // Ensure the billboard faces the camera
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.SetLocalPositionAndRotation(transform.localPosition, Quaternion.Euler(0, transform.localEulerAngles.y, 0));
    }
}
