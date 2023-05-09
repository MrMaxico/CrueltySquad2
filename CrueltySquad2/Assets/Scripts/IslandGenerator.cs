using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IslandGenerator : MonoBehaviour
{
    public bool regenTerrain;
    Mesh mesh;

    Vector3 offsetPosition;
    Vector3[] vertices;

    [Range(25, 250)] public int xSize;
    [Range(25, 250)] public int zSize;
    [Range(0, 99999)] public int seed;

    [Range(5, 15)] public int octaves;
    [Range(1, 10)] public float frequencyMultiplier;
    [Range(0.1f, 1)] public float amplitudeMultiplier;
    [Range(1, 50)] public float heightMultiplier;
    [Range(12.5f, 125)] public float center;
    [Range(0.001f, 0.01f)] public float fallOff;
    public float bottomVertHeight;
    public float extraNoice;
    float fallOffX;
    float fallOffZ;
    float fallOffXZ;
    int[] triangles;

    // Start is called before the first frame update
    public void Start()
    {
        if (seed != 0)
        {
            StartGenerating(seed);
        }
        else
        {
            StartGenerating(Random.Range(0, 99999));
        }
    }

    public void StartGenerating(int n_seed)
    {
        seed = n_seed;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (regenTerrain)
        {
            regenTerrain = false;
            StartGenerating(Random.Range(0, 99999));
        }
    }

    //generates a random terrain using a seed
    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        int i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float freq = 1;
                float amp = 1;

                float noieHeight = 0;
                for (int j = 0; j < octaves; j++)
                {
                    fallOffX = (center * center - 2 * center * x + 4 * heightMultiplier * fallOff + x * x) / 4 * fallOff;
                    fallOffZ = (center * center - 2 * center * z + 4 * heightMultiplier * fallOff + z * z) / 4 * fallOff;
                    fallOffXZ = (fallOffX + fallOffZ) / 2;
                    noieHeight += (Mathf.PerlinNoise((x * .03f * freq) + seed, (z * .03f * freq) + seed) * heightMultiplier * amp) - fallOffXZ;
                    amp *= amplitudeMultiplier;
                    freq *= frequencyMultiplier;
                }
                float y = noieHeight;
                if (y < bottomVertHeight)
                {
                    y = bottomVertHeight;
                }
                y += bottomVertHeight + Random.Range(-extraNoice, extraNoice);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
