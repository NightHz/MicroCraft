using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform eye;
    [Range(0.02f,0.2f)]
    public float speed = 0.05f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
            transform.position += Vector3.Cross(eye.forward, Vector3.up).normalized * speed;
        if (Input.GetKey(KeyCode.D))
            transform.position -= Vector3.Cross(eye.forward, Vector3.up).normalized * speed;
        if (Input.GetKey(KeyCode.W))
            transform.position += Vector3.Cross(eye.right,Vector3.up).normalized * speed;
        if (Input.GetKey(KeyCode.S))
            transform.position -= Vector3.Cross(eye.right, Vector3.up).normalized * speed;
        if (Input.GetKey(KeyCode.Space))
            transform.position += Vector3.up * speed;
        if (Input.GetKey(KeyCode.LeftShift))
            transform.position += Vector3.down * speed;
    }
}
