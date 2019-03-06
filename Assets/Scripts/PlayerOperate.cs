﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOperate : MonoBehaviour
{
    public GameObject selectionFaceX;
    public GameObject selectionFaceY;
    public GameObject selectionFaceZ;
    Vector3 selectionFacePos = new Vector3(0, -1000, 0);
    float offset = 0.001f;

    void Update()
    {
        selectionFaceX.transform.position = selectionFacePos;
        selectionFaceY.transform.position = selectionFacePos;
        selectionFaceZ.transform.position = selectionFacePos;

        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 10f))
        {
            Vector3 point = raycastHit.point;
            Vector3Int block = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), Mathf.FloorToInt(point.z));
            Vector3Int blockAdd = block;
            if (point.y - block.y < 0.01f)
            {
                if (point.x - block.x < 0.01f || point.z - block.z < 0.01f)
                    return;
                else if (transform.forward.y <= 0)
                {
                    block.y--; // 命中顶面
                    selectionFaceY.transform.position = new Vector3(block.x + 0.5f, block.y + 1 + offset, block.z + 0.5f);
                }
                else
                {
                    blockAdd.y--; // 命中底面
                    selectionFaceY.transform.position = new Vector3(block.x + 0.5f, block.y - offset, block.z + 0.5f);
                }
            }
            else if (point.z - block.z < 0.01f)
            {
                if (point.x - block.x < 0.01f)
                    return;
                else if (transform.forward.z >= 0)
                {
                    blockAdd.z--; // 命中前面
                    selectionFaceZ.transform.position = new Vector3(block.x + 0.5f, block.y + 0.5f, block.z - offset);
                }
                else
                {
                    block.z--; // 命中后面
                    selectionFaceZ.transform.position = new Vector3(block.x + 0.5f, block.y + 0.5f, block.z + 1 + offset);
                }
            }
            else
            {
                if (transform.forward.x <= 0)
                {
                    block.x--; // 命中右边
                    selectionFaceX.transform.position = new Vector3(block.x + 1 + offset, block.y + 0.5f, block.z + 0.5f);
                }
                else
                {
                    blockAdd.x--; // 命中左边
                    selectionFaceX.transform.position = new Vector3(block.x - offset, block.y + 0.5f, block.z + 0.5f);
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                Chunk chunk = raycastHit.collider.gameObject.GetComponent<Chunk>();
                Debug.Log("chunk:" + chunk.position + " block:" + block + "to Air");
                chunk.SetBlock(block - chunk.position, BlockID.Air);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Chunk chunk = raycastHit.collider.gameObject.GetComponent<Chunk>();
                Debug.Log("chunk:" + chunk.position + " block:" + blockAdd + "to Stone");
                chunk.SetBlock(blockAdd - chunk.position, BlockID.Stone);
            }
        }
    }
}