using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomVariable", menuName = "ScriptableObjects/RandomVariable", order = 1)]
[System.Serializable]
public class RandomVariable : ScriptableObject
{
    public int spawnChance;
    public variableType type;
    [Space(20)]
    [Header("You only need to assign the choosen variable type")]
    public GameObject GameObject;
    public string String;

    public enum variableType
    {
        GameObject,
        String
    }
}
