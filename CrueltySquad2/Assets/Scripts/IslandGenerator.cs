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

    [Range(25, 250)] public int xSize = 250;
    [Range(25, 250)] public int zSize = 250;
    [Range(0, 99999)] public int seed;

    [Range(5, 15)] public int octaves = 10;
    [Range(1, 10)] public float frequencyMultiplier = 2;
    [Range(1, 10)] public float lowGroundFrequencyMultiplier = 1;
    [Range(0.1f, 1)] public float amplitudeMultiplier = 0.3f;
    [Range(1, 250)] public float heightMultiplier = 120;
    [Range(1, 10)] public float lowGroundHeightMultiplier = 10;
    [Range(12.5f, 125)] public float center = 125;
    [Range(0.001f, 0.01f)] public float fallOff = 0.004f;
    [Range(0, 250)] public float bottomVertHeight = 80;
    [Range(0.1f, 1)] public float extraNoice = 0.3f;
    float fallOffX;
    float fallOffZ;
    float fallOffXZ;
    int[] triangles;

    public MeshFilter meshFilter;
    public Texture2D texture;
    public float minHeight = 0f;
    public float maxHeight = 1f;
    public float greenThreshold = 0.3f;
    public float whiteThreshold = 1f;

    public Color greenColor = Color.green;
    public Color grayColor = Color.gray;
    public Color whiteColor = Color.white;

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
                    y = bottomVertHeight - (Mathf.PerlinNoise((x * .03f * lowGroundFrequencyMultiplier) + seed, (z * .03f * lowGroundFrequencyMultiplier) + seed) * lowGroundHeightMultiplier);
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

        GenerateTexture();
    }

    void GenerateTexture()
    {

        mesh = meshFilter.mesh;
        vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector2[] uvs = new Vector2[vertices.Length];

        int textureWidth = Mathf.RoundToInt(Mathf.Sqrt(vertices.Length));
        int textureHeight = Mathf.RoundToInt(Mathf.Sqrt(vertices.Length));
        texture = new Texture2D(textureWidth, textureHeight);
        texture.filterMode = FilterMode.Point;

        for (int i = 0; i < vertices.Length; i++)
        {
            float height = vertices[i].y;
            float normalizedHeight = Mathf.InverseLerp(minHeight, maxHeight, height);

            // Set the pixel color based on the height
            Color pixelColor;
            if (normalizedHeight < greenThreshold)
            {
                pixelColor = greenColor;
            }
            else if (normalizedHeight >= whiteThreshold)
            {
                pixelColor = whiteColor;
            }
            else
            {
                pixelColor = grayColor;
            }

            int x = i % texture.width;
            int y = i / texture.width;
            texture.SetPixel(x, y, pixelColor);

            // Calculate UV coordinates
            uvs[i] = new Vector2((float)x / texture.width, (float)y / texture.height);
        }

        // Apply the changes to the texture
        texture.Apply();

        // Assign the UV coordinates to the mesh
        mesh.uv = uvs;

        // Create a new material with the generated texture
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.mainTexture = texture;
        material.SetFloat("_Smoothness", 0);

        // Assign the material to the mesh renderer
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;
    }
}
