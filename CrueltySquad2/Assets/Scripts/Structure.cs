using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "ScriptableObjects/Structure", order = 1)]
[System.Serializable]
public class Structure : ScriptableObject
{
    public GameObject structurePrefab;
    public int minimalInstances;
    public int maximalInstances;
    public List<Biome> allowedBiomes;

    public Structure(GameObject n_structurePrefab, int n_minimalInstances, int n_maximalInstances, List<Biome> n_allowedBiomes)
    {
        structurePrefab = n_structurePrefab;
        minimalInstances = n_minimalInstances;
        maximalInstances = n_maximalInstances;
        allowedBiomes = n_allowedBiomes;
    }
}
