using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    World world;
    public Transform eye;
    [Range(1.2f,12f)]
    public float speed = 3f;

    private void Start()
    {
        world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
    }

    private void Update()
    {
        if (world.State != WorldState.Working)
            return;

        float move = speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            transform.position += Vector3.Cross(eye.forward, Vector3.up).normalized * move;
        if (Input.GetKey(KeyCode.D))
            transform.position -= Vector3.Cross(eye.forward, Vector3.up).normalized * move;
        if (Input.GetKey(KeyCode.W))
            transform.position += Vector3.Cross(eye.right,Vector3.up).normalized * move;
        if (Input.GetKey(KeyCode.S))
            transform.position -= Vector3.Cross(eye.right, Vector3.up).normalized * move;
        if (Input.GetKey(KeyCode.Space))
            transform.position += Vector3.up * move;
        if (Input.GetKey(KeyCode.LeftShift))
            transform.position += Vector3.down * move;
    }
}
