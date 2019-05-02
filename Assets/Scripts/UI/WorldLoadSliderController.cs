using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldLoadSliderController : MonoBehaviour
{
    World world;
    public Text text;
    public Slider slider;
    float startTime;

    private void Start()
    {
        world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
        startTime = Time.time;
    }

    private void Update()
    {
        if (world.State == WorldState.Loading)
        {
            float v = 1f - (float)world.chunkManager.ChunkWaitUpdateCount / world.chunkManager.ChunkCount;
            if (slider.value < v)
                slider.value += 0.01f;
            text.text = ((int)(slider.value * 100)).ToString() + "%";
        }
        else
        {
            Debug.Log("World Load Time : " + (Time.time - startTime) + "s");
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

    }
}
