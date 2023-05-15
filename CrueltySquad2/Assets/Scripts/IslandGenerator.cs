using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IslandGenerator : MonoBehaviour
{
    public bool regenTerrain;
    Mesh mesh;

    Vector3[] vertices;

    [Range(25, 250)] public int xSize = 250;
    [Range(25, 250)] public int zSize = 250;
    [Range(50, 125)] public int islandRadius = 75;
    [Range(0, 99999)] public int seed;

    [Range(5, 15)] public int octaves = 15;
    [Range(1, 10)] public float frequencyMultiplier = 1;
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
    public float sandThreshold = 0.735f;
    public float greenThreshold = 0.785f;
    public float whiteThreshold = 0.95f;

    int colorSeed;

    public Biome lowerArea;
    public Biome higherArea;
    public Biome topArea;
    public Biome bottomArea;

    public List<Structure> structures;
    public List<GameObject> spawnedStructures;
    Vector3 invalidPosition;

    // Start is called before the first frame update
    public void Start()
    {
        //if a seed is already chosen it gets generated, otherwise it just picks a random seed
        if (seed != 0)
        {
            StartGenerating(seed);
        }
        else
        {
            StartGenerating(Random.Range(0, 99999));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //when you trigger the bool in the inspector, a new terrain generates
        if (regenTerrain)
        {
            regenTerrain = false;
            for (int i = spawnedStructures.Count - 1; i >= 0; i--)
            {
                Destroy(spawnedStructures[i]);
                spawnedStructures.Remove(spawnedStructures[i]);
            }
            StartGenerating(Random.Range(0, 99999));
        }
    }

    public void StartGenerating(int n_seed)
    {
        seed = n_seed;
        colorSeed = Random.Range(0, 99999);
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
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
                if (y < bottomVertHeight && !IsBeach(x, z))
                {
                    y = bottomVertHeight - (Mathf.PerlinNoise((x * .03f * lowGroundFrequencyMultiplier) + seed, (z * .03f * lowGroundFrequencyMultiplier) + seed) * lowGroundHeightMultiplier);
                }
                else if (y < bottomVertHeight)
                {
                    y = bottomVertHeight - (DistanceFromCenter(x, z) - islandRadius) - (Mathf.PerlinNoise((x * .03f * lowGroundFrequencyMultiplier) + seed, (z * .03f * lowGroundFrequencyMultiplier) + seed) * lowGroundHeightMultiplier);
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
        SpawnStructures();
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
            if (normalizedHeight < greenThreshold && normalizedHeight >= sandThreshold)
            {
                pixelColor = TerrainColor(new Vector2(vertices[i].x, vertices[i].z), lowerArea);
            }
            else if (normalizedHeight < sandThreshold)
            {
                pixelColor = TerrainColor(new Vector2(vertices[i].x, vertices[i].z), bottomArea);
            }
            else if (normalizedHeight >= whiteThreshold)
            {
                pixelColor = TerrainColor(new Vector2(vertices[i].x, vertices[i].z), topArea);
            }
            else
            {
                pixelColor = TerrainColor(new Vector2(vertices[i].x, vertices[i].z), higherArea);
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

    void SpawnStructures()
    {
        foreach (Structure n_structure in structures)
        {
            int n_spawnAmount = Random.Range(n_structure.minimalInstances, n_structure.maximalInstances);
            for (int i = 0; i < n_spawnAmount; i++)
            {
                StructureSpawnInfo n_spawnInfo = StructureSpawnData(n_structure.allowedBiomes);
                spawnedStructures.Add(Instantiate(n_structure.structurePrefab, n_spawnInfo.position, n_spawnInfo.rotation));
            }
        }
    }

    bool IsBeach(float n_x, float n_z)
    {
        if (DistanceFromCenter(n_x, n_z) > islandRadius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    float DistanceFromCenter(float n_x, float n_z)
    {
        return Vector3.Distance(new Vector3(n_x, 0, n_z), new Vector3(center, 0, center));
    }

    StructureSpawnInfo StructureSpawnData(List<Biome> n_allowedBiomes)
    {
        Vector3 n_postion = new Vector3();
        Quaternion n_rotation = new Quaternion();
        Vector3 n_inAirPos = new Vector3(Random.Range(center - islandRadius, center + islandRadius), 500, Random.Range(center - islandRadius, center + islandRadius));
        if (Physics.Raycast(n_inAirPos, -Vector3.up, out RaycastHit hitpoint, 1000))
        {
            if (hitpoint.transform.gameObject.TryGetComponent<IslandGenerator>(out IslandGenerator n_island) && CheckValidBiome(n_inAirPos, n_allowedBiomes))
            {
                Debug.Log("Valid position found");
                n_postion = hitpoint.point;
                n_rotation = Quaternion.FromToRotation(Vector3.up, hitpoint.normal);
                n_rotation *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                return new StructureSpawnInfo(n_postion, n_rotation);
            }
        }
        Debug.Log("Position resulted invalid, trying to find a new one");
        n_postion = invalidPosition;
        n_rotation = Quaternion.FromToRotation(Vector3.up, invalidPosition);
        n_rotation *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        return new StructureSpawnInfo(n_postion, n_rotation);
    }

    bool CheckValidBiome(Vector3 n_position, List<Biome> n_allowedBiomes)
    {
        for (int i = 0; i < n_allowedBiomes.Count; i++)
        {
            bool rValid = false;
            bool gValid = false;
            bool bValid = false;
            Color pixelColor = texture.GetPixel(Mathf.RoundToInt(n_position.x), Mathf.RoundToInt(n_position.z));
            Color allowedBiomeColorA = n_allowedBiomes[i].colorA;
            Color allowedBiomeColorB = n_allowedBiomes[i].colorB;
            if (pixelColor.r >= allowedBiomeColorA.r && pixelColor.r <= allowedBiomeColorB.r)
            {
                rValid = true;
            }

            if (pixelColor.g >= allowedBiomeColorA.g && pixelColor.g <= allowedBiomeColorB.g)
            {
                gValid = true;
            }

            if (pixelColor.b >= allowedBiomeColorA.b && pixelColor.b <= allowedBiomeColorB.b)
            {
                bValid = true;
            }

            if (rValid && gValid && bValid)
            {
                return true;
            }
        }
        return false;
    }

    Color TerrainColor(Vector2 n_texturePosition, Biome n_biome)
    {
        return Color.Lerp(n_biome.colorA, n_biome.colorB, Mathf.PerlinNoise((n_texturePosition.x * .01f * lowGroundFrequencyMultiplier) + colorSeed, (n_texturePosition.y * .01f * lowGroundFrequencyMultiplier) + colorSeed));
    }
}

public class StructureSpawnInfo
{
    public Vector3 position;
    public Quaternion rotation;

    public StructureSpawnInfo(Vector3 n_position, Quaternion n_rotation)
    {
        position = n_position;
        rotation = n_rotation;
    }
}
