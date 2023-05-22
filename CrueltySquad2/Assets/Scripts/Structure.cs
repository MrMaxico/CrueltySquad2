using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "ScriptableObjects/Structure", order = 1)]
[System.Serializable]
public class Structure : ScriptableObject
{
    [Tooltip("The prefab that generates")]
    public GameObject structurePrefab;
    [Space(20)]
    [Tooltip("The minimal amount of prefabs that needs to be spawned")]
    public int minimalInstances;
    [Tooltip("The maximal amount of prefabs that needs to be spawned")]
    public int maximalInstances;
    [Space(20)]
    public List<Biome> allowedBiomes;
    [Space(20)]
    [Tooltip("Check this to give the instances a random scale")]
    public bool variableSize;
    public float minSize;
    public float maxSize;
    [Space(20)]
    [Tooltip("Check this to ignore rotating to the slopes of the terrain at spawning")]
    public bool ignoreSlopes;
    [Tooltip("Use this to spawn this structure higher above the ground")]
    public float spawnAltitude;
}
