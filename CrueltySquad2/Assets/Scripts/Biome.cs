using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "ScriptableObjects/Biome", order = 2)]
[System.Serializable]
public class Biome : ScriptableObject
{
    public string biomeName;
    public Color color;

    public Biome(string n_name, Color n_color)
    {
        name = n_name;
        color = n_color;
    }
}
