using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 只能被实例化一次
/// 生成区块，管理已有区块，管理脏区块
/// </summary>
public class ChunkManager : MonoBehaviour
{
    public static readonly int width = WorldTerrain.chunkWidth;
    public static readonly int height = WorldTerrain.heightMax;

    static ChunkManager chunkManager = null;

    GameObject player;
    public GameObject chunkPrefab;

    Queue<Chunk> chunksGen = new Queue<Chunk>();
    Dictionary<Vector3Int, GameObject> chunks = new Dictionary<Vector3Int, GameObject>();
    Dictionary<Vector3Int, ChunkData> dirtyChunks = new Dictionary<Vector3Int, ChunkData>();

    [Range(16, 64)]
    public int workSpeed = 32;

    private void Start()
    {
        if (chunkManager == null)
            chunkManager = this;
        else
            Debug.LogError("ChunkManager同一时间被加载了两次");
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Work());
    }

    private void OnDestroy()
    {
        chunkManager = null;
    }


    public static int AlignChunkXZ(int xz) { return xz / width * width; }
    public static int AlignChunkXZ(float xz) { return Mathf.FloorToInt(xz) / width * width; }
    public static int AlignChunkY(int y) { return 0; }
    public static int AlignChunkY(float y) { return 0; }
    public static Vector3Int AlignChunk(Vector3Int p) { return new Vector3Int(AlignChunkXZ(p.x), AlignChunkY(p.y), AlignChunkXZ(p.z)); }
    public static Vector3Int AlignChunk(Vector3 p) { return new Vector3Int(AlignChunkXZ(p.x), AlignChunkY(p.y), AlignChunkXZ(p.z)); }


    private void Update()
    {
        // 对足够近的未生成区块进行生成
        Vector3Int chunkPos = new Vector3Int();
        chunkPos.y = 0;
        float dis = World.maxDistanceGen;
        for (int x = AlignChunkXZ(player.transform.position.x - dis) - width; x < player.transform.position.x + dis; x += width)
        {
            chunkPos.x = x;
            for (int z = AlignChunkXZ(player.transform.position.z - dis) - width; z < player.transform.position.z + dis; z += width)
            {
                chunkPos.z = z;
                if (!chunks.ContainsKey(chunkPos))
                {
                    float disX = x + width / 2 - player.transform.position.x;
                    float disZ = z + width / 2 - player.transform.position.z;
                    if (disX * disX + disZ * disZ < dis * dis)
                        GenerateChunk(x, 0, z);
                }
            }
        }
    }

    void GenerateChunk(int x, int y, int z)
    {
        Debug.Log("开始生成区块:(" + x + "," + 0 + "," + z + ")");
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
        chunks.Add(new Vector3Int(x, y, z), chunk);
    }

    public static void StartGenerate(Chunk chunk)
    {
        chunk.player = chunkManager.player;
        chunkManager.chunksGen.Enqueue(chunk);
    }

    public static void DestroyChunk(Vector3Int position)
    {
        Debug.Log("销毁区块:" + position);
        chunkManager.chunks.Remove(position);
    }

    public static int CountGenQueue()
    {
        return chunkManager.chunksGen.Count;
    }

    public static int Count()
    {
        return chunkManager.chunks.Count;
    }

    IEnumerator Work()
    {
        while (true)
        {
            if (chunksGen.Count == 0)
                yield return null;
            else
            {
                Chunk chunk = chunksGen.Dequeue();
                if (chunk.state == ChunkState.Destroy)
                    continue;
                chunk.state = ChunkState.GenerateIsWorking;

                // Generate
                if (dirtyChunks.ContainsKey(chunk.position))
                {
                    chunk.blocks = dirtyChunks[chunk.position].blocks;
                }
                else
                {
                    BlockID[,,] blocks = new BlockID[width, height, width];
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                            for (int z = 0; z < width; z++)
                                blocks[x, y, z] = WorldTerrain.GetBlock(x + chunk.position.x, y + chunk.position.y, z + chunk.position.z);
                        if (y % workSpeed == 0)
                            yield return null;
                    }
                    chunk.blocks = blocks;
                }
                // Submit to Updater
                ChunkUpdater.UpdateChunk(chunk);
                // Finish
                Debug.Log("生成:" + chunk.name);
                yield return null;
            }
        }
    }
}

