using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界，只能被实例化一次
/// 控制区块生成
/// </summary>
public class World : MonoBehaviour
{
    static World world = null;

    public GameObject chunkPrefab;
    Dictionary<Vector3Int, GameObject> chunks = new Dictionary<Vector3Int, GameObject>();
    
    GameObject player;

    public string seed;
    public static readonly int maxDistanceGen = 100;
    public static readonly int minDistanceDes = 140;
    
    private void Start()
    {
        if (world == null)
            world = this;
        else
            Debug.LogError("World同一时间被加载了两次");
        WorldTerrain.Awake(seed.GetHashCode());
        player = GameObject.FindGameObjectWithTag("Player");
        // GenerateWorld();
    }

    private void OnDestroy()
    {
        world = null;
    }

    private void Update()
    {
        // 对足够近的未生成区块进行生成
        Vector3Int chunkPos = new Vector3Int();
        chunkPos.y = 0;
        for (int x = ChunkGen.AlignChunkXZ(player.transform.position.x - maxDistanceGen) - ChunkGen.width; x < player.transform.position.x + maxDistanceGen; x += ChunkGen.width)
        {
            chunkPos.x = x;
            for (int z = ChunkGen.AlignChunkXZ(player.transform.position.z - maxDistanceGen) - ChunkGen.width; z < player.transform.position.z + maxDistanceGen; z += ChunkGen.width)
            {
                chunkPos.z = z;
                if (!chunks.ContainsKey(chunkPos))
                {
                    float disX = x + ChunkGen.width / 2 - player.transform.position.x;
                    float disZ = z + ChunkGen.width / 2 - player.transform.position.z;
                    if (disX * disX + disZ * disZ < maxDistanceGen * maxDistanceGen)
                    {
                        Debug.Log("开始生成区块:(" + x + "," + 0 + "," + z + ")");
                        GenerateChunk(x, 0, z);
                    }
                }
            }
        }
    }

    void GenerateWorld()
    {
        int distanceGen = (maxDistanceGen + minDistanceDes) / 2;
        Vector3Int chunkPos = new Vector3Int();
        chunkPos.y = 0;
        for (int x = ChunkGen.AlignChunkXZ(-distanceGen) - ChunkGen.width; x < distanceGen; x += ChunkGen.width)
        {
            chunkPos.x = x;
            for (int z = ChunkGen.AlignChunkXZ(-distanceGen) - ChunkGen.width; z < distanceGen; z += ChunkGen.width)
            {
                chunkPos.z = z;
                float disX = x + ChunkGen.width / 2;
                float disZ = z + ChunkGen.width / 2;
                if (disX * disX + disZ * disZ < distanceGen * distanceGen)
                {
                    Debug.Log("开始生成区块:(" + x + "," + 0 + "," + z + ")");
                    GenerateChunk(x, 0, z);
                }

            }
        }
    }

    void GenerateChunk(int x, int y, int z)
    {
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
        chunks.Add(new Vector3Int(x, y, z), chunk);
    }

    public static void DestroyChunk(Vector3Int position)
    {
        world.chunks.Remove(position);
    }
}
