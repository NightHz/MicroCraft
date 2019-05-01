using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class F1Message : MonoBehaviour
{
    World world;
    Text text;
    float timer;
    int fps;
    string strFps;
    string strUpdateChunkCount;
    string strChunkCount;

    // Start is called before the first frame update
    void Start()
    {
        world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
        text = GetComponent<Text>();
        timer = 0;
        fps = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        fps++;
        if (timer > 0.5)
        {
            strFps = (fps + fps) + " fps";
            timer -= 0.5f;
            fps = 0;
        }
        strChunkCount = world.chunkManager.ChunkCount + " chunks load";
        strUpdateChunkCount = world.chunkManager.ChunkWaitUpdateCount + " chunks in update list";
        text.text = strFps + "\n" + strChunkCount + "\n" + strUpdateChunkCount;
    }
}
