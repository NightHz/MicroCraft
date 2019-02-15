using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界生成器，绑定在游戏物件上，只能被实例化一次
/// 生成世界的起点，控制区块生成的位置
/// </summary>
public class WorldGen : MonoBehaviour
{
    static WorldGen worldGen = null;

    public GameObject chunkPrefab;
    Dictionary<Vector3Int, GameObject> chunks = new Dictionary<Vector3Int, GameObject>();
    
    GameObject player;

    public string seed;
    
    private void Start()
    {
        if (worldGen == null)
            worldGen = this;
        else
            Debug.LogError("WorldGen同一时间被加载了两次");
        WorldTerrain.Awake(seed.GetHashCode());
        GenerateWorld();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnDestroy()
    {
        worldGen = null;
    }

    private void Update()
    {
        // 对足够近的未生成区块进行生成
        Vector3Int chunkPos = new Vector3Int();
        for (int x = Mathf.FloorToInt(player.transform.position.x - 80) / 16 * 16; x < player.transform.position.x + 80; x += ChunkGen.width)
        {
            chunkPos.x = x;
            for (int z = Mathf.FloorToInt(player.transform.position.z - 80) / 16 * 16; z < player.transform.position.z + 80; z += ChunkGen.width)
            {
                chunkPos.z = z;
                for (int y = 0; y < 1; y += ChunkGen.height)
                {
                    chunkPos.y = y;
                    if (!chunks.ContainsKey(chunkPos))
                    {
                        float disX = player.transform.position.x - x;
                        float disZ = player.transform.position.z - z;
                        if (disX * disX + disZ * disZ < 5000)
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
        for (int x = -64; x < 64; x += ChunkGen.width)
            for (int z = -64; z < 64; z += ChunkGen.width)
                for (int y = 0; y < 1; y += ChunkGen.height)
                    GenerateChunk(x, y, z);
    }

    void GenerateChunk(int x, int y, int z)
    {
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
        chunks.Add(new Vector3Int(x, y, z), chunk);
    }

    public static void DestroyChunk(Vector3Int position)
    {
        worldGen.chunks.Remove(position);
    }
}
