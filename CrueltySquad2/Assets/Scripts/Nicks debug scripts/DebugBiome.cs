using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBiome : MonoBehaviour
{
    public Transform objectToCheckBiomeOf;
    public string output;

    private void Update()
    {
        output = GetComponent<IslandGenerator>().verticeBiome[Mathf.RoundToInt(objectToCheckBiomeOf.position.x), Mathf.RoundToInt(objectToCheckBiomeOf.position.z)].biomeName;
    }
}
