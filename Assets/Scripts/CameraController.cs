using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;

    public float maxElevationAngle = 80f;
    public float minElevationAngle = -80f;

    public float eyeOffset = 0.6f;
    public float tpsOffset = 8f;

    private void Update()
    {
        float rotationAngleX = Input.GetAxis("Mouse X");
        float rotationAngleY = Input.GetAxis("Mouse Y");

        float nowElevationAngle = 90f - Vector3.SignedAngle(Vector3.up, transform.forward, transform.right);

        float MaxRotationAngleY = maxElevationAngle - nowElevationAngle;
        float MinRotationAngleY = minElevationAngle - nowElevationAngle;

        rotationAngleY = Mathf.Clamp(rotationAngleY, MinRotationAngleY, MaxRotationAngleY);

        transform.Rotate(Vector3.up * rotationAngleX - transform.right * rotationAngleY, Space.World);

        // 第一人称
        //transform.position = player.position + Vector3.up * eyeOffset;

        // 第三人称
        transform.position = player.position + Vector3.up * eyeOffset - transform.forward * tpsOffset;
    }

}
