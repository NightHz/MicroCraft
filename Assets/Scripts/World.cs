using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界
/// </summary>
public class World : MonoBehaviour
{
    public string seed;
    public static readonly int maxDistanceGen = 100;
    public static readonly int minDistanceDes = 200*100;

    private void Start()
    {
        WorldTerrain.Awake(seed.GetHashCode());
    }

}
