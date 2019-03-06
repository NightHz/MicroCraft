using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class F1Message : MonoBehaviour
{
    Text text;
    float timer;
    int fps;
    string strFps;
    string strUpdateChunkCount;

    // Start is called before the first frame update
    void Start()
    {
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
        int chunkGenCount = ChunkManager.CountGenQueue();
        strUpdateChunkCount = (ChunkUpdater.Count() + chunkGenCount) + "(" + chunkGenCount + ")" + " chunks update";
        text.text = strFps + "\n" + strUpdateChunkCount;
    }
}
