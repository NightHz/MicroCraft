using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;

    public float maxElevationAngle = 80f;
    public float minElevationAngle = -80f;

    float angleX = 0f;
    float angleY = 30f;

    public float eyeOffset = 0.6f;
    public float thirdPersonOffset = 8f;

    public bool isThirdPerson = false;

    private void Update()
    {
        float rotationAngleX = Input.GetAxis("Mouse X");
        float rotationAngleY = Input.GetAxis("Mouse Y");

        angleX += rotationAngleX;
        angleY -= rotationAngleY;
        angleY = Mathf.Clamp(angleY, minElevationAngle, maxElevationAngle);

        transform.rotation = Quaternion.Euler(angleY, angleX, 0);

        if (!isThirdPerson)
            // 第一人称
            transform.position = player.position + Vector3.up * eyeOffset;
        else
            // 第三人称
            transform.position = player.position + Vector3.up * eyeOffset - transform.forward * thirdPersonOffset;
    }

}
