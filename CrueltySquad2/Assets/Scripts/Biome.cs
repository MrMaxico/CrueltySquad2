using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "ScriptableObjects/Biome", order = 2)]
[System.Serializable]
public class Biome : ScriptableObject
{
    public string biomeName;
    [Header("Terrain color will be a color between these two colors. The RGBA values of ColorA should be lower than the values of ColorB")]
    public Color colorA;
    public Color colorB;

    public Biome(string n_name, Color n_colorA, Color n_colorB)
    {
        name = n_name;
        colorA = n_colorA;
        colorB = n_colorB;
    }
}
