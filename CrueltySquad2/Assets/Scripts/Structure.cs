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

    public Structure(GameObject n_structurePrefab, int n_minimalInstances, int n_maximalInstances, List<Biome> n_allowedBiomes, bool n_variableSize, float n_minSize, float n_maxSize, bool n_ignoreSlopes)
    {
        structurePrefab = n_structurePrefab;
        minimalInstances = n_minimalInstances;
        maximalInstances = n_maximalInstances;
        allowedBiomes = n_allowedBiomes;
        variableSize = n_variableSize;
        minSize = n_minSize;
        maxSize = n_maxSize;
        ignoreSlopes = n_ignoreSlopes;
    }
}
