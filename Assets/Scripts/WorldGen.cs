using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{
    public GameObject chunkPrefab;
    Dictionary<Vector3Int, GameObject> chunks = new Dictionary<Vector3Int, GameObject>();

    public uint seed;

    GameObject player;

    bool finished = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (!finished && Input.GetMouseButtonDown(0))
        {
            GenerateWorld();
            finished = true;
        }
        if (finished)
        {
            // 对足够近的未生成区块进行生成
            Vector3Int chunkPos = new Vector3Int();
            for (int x = Mathf.FloorToInt(player.transform.position.x - 48) / 16 * 16; x < player.transform.position.x + 48; x += Chunk.width)
            {
                chunkPos.x = x;
                for (int z = Mathf.FloorToInt(player.transform.position.z - 48) / 16 * 16; z < player.transform.position.z + 48; z += Chunk.width)
                {
                    chunkPos.z = z;
                    for (int y = 0; y < 1; y += Chunk.height)
                    {
                        chunkPos.y = y;
                        if (!chunks.ContainsKey(chunkPos))
                        {
                            Debug.Log("开始生成区块:(" + x + "," + y + "," + z + ")");
                            GenerateChunk(x, y, z);
                        }
                    }
                }
            }
        }
    }

    void GenerateWorld()
    {
        for (int x = -64; x < 64; x += Chunk.width)
            for (int z = -64; z < 64; z += Chunk.width)
                for (int y = 0; y < 1; y += Chunk.height)
                    GenerateChunk(x, y, z);
    }

    void GenerateChunk(int x, int y, int z)
    {
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
        chunks.Add(new Vector3Int(x, y, z), chunk);

    }
}
