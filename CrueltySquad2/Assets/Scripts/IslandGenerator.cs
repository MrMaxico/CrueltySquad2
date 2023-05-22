using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IslandGenerator : MonoBehaviour
{
    [Tooltip("Trigger this bool to regenerate the terrain")]
    [SerializeField] bool regenTerrain;

    Mesh mesh;
    Vector3[] vertices;

    [Space(20)]
    [Header("Generation values")]
    [Tooltip("Set the length of the island")]
    [Range(25, 250)] public int xSize = 250;
    [Tooltip("Set the width of the island")]
    [Range(25, 250)] public int zSize = 250;
    [Tooltip("Set the radius of the island. The rest of the island will be a beach. This value should always be lower than half of the xSize and zSize")]
    [Range(50, 125)] public int islandRadius = 75;
    [Tooltip("Set the seed for the generation")]
    [Range(0, 99999)] public int seed;
    [Tooltip("Set the amount of layers of noise that will generate on top of each other")]
    [Range(5, 15)] public int octaves = 15;
    [Tooltip("Set the frequency of noise")]
    [Range(1, 4)] public float frequencyMultiplier = 1;
    [Tooltip("Set the frequency of noise in the lower area")]
    [Range(1, 4)] public float lowGroundFrequencyMultiplier = 1;
    [Tooltip("Set the intensity of the noise")]
    [Range(0.1f, 1)] public float amplitudeMultiplier = 0.3f;
    [Tooltip("Set the multiplier on the height of the noise")]
    [Range(1, 250)] public float heightMultiplier = 120;
    [Tooltip("Set the multiplier on the height of the noise of the lower area")]
    [Range(1, 10)] public float lowGroundHeightMultiplier = 10;
    [Tooltip("Set the center of the island. Most times, this should be half of xSize and zSize")]
    [Range(12.5f, 125)] public float center = 125;
    [Tooltip("Set the falloff of the height on the edges of the island")]
    [Range(0.001f, 0.01f)] public float fallOff = 0.004f;
    [Tooltip("Set height level of the lower area")]
    [Range(0, 250)] public float bottomVertHeight = 80;
    [Tooltip("Set the extra noise for a triangle shaped terrain. 0 for smooth terrain")]
    [Range(0, 1)] public float extraNoice = 0.1f;
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
    public float maxGreenAreaThreshold;

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
        // Find the maximum height among the vertices
        float maxVertexHeight = float.MinValue;
        for (int i = 0; i < vertices.Length; i++)
        {
            maxVertexHeight = Mathf.Max(maxVertexHeight, vertices[i].y);
        }

        // Assign colors based on vertex and triangle height
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int vertexIndex1 = triangles[i];
            int vertexIndex2 = triangles[i + 1];
            int vertexIndex3 = triangles[i + 2];

            float height1 = vertices[vertexIndex1].y;
            float height2 = vertices[vertexIndex2].y;
            float height3 = vertices[vertexIndex3].y;

            float averageHeight = (height1 + height2 + height3) / 3f;
            float normalizedVertexHeight = Mathf.InverseLerp(minHeight, maxHeight, averageHeight);
            float normalizedTriangleHeight = Mathf.InverseLerp(minHeight, maxHeight, Mathf.Max(height1, height2, height3));

            // Set the pixel color based on the vertex and triangle heights
            Color pixelColor;
            if (normalizedVertexHeight < sandThreshold)
            {
                pixelColor = TerrainColor(vertices[vertexIndex1], bottomArea);
            }
            else if (normalizedVertexHeight < greenThreshold)
            {
                // Calculate the interpolation factor between sand and green
                pixelColor = TerrainColor(vertices[vertexIndex1], lowerArea);
            }
            else if (normalizedVertexHeight >= whiteThreshold)
            {
                pixelColor = TerrainColor(vertices[vertexIndex1], topArea);
            }
            else
            {
                // Calculate the interpolation factor between green and gray
                float t = Mathf.InverseLerp(greenThreshold - maxGreenAreaThreshold, whiteThreshold, normalizedVertexHeight);
                pixelColor = Color.Lerp(TerrainColor(vertices[vertexIndex1], lowerArea), TerrainColor(vertices[vertexIndex1], higherArea), t);
            }

            // Assign the pixel color to the vertices of the triangle
            texture.SetPixel(vertexIndex1 % textureWidth, vertexIndex1 / textureWidth, pixelColor);
            texture.SetPixel(vertexIndex2 % textureWidth, vertexIndex2 / textureWidth, pixelColor);
            texture.SetPixel(vertexIndex3 % textureWidth, vertexIndex3 / textureWidth, pixelColor);

            // Assign UV coordinates to the vertices of the triangle
            uvs[vertexIndex1] = new Vector2((float)(vertexIndex1 % textureWidth) / textureWidth, (float)(vertexIndex1 / textureWidth) / textureHeight);
            uvs[vertexIndex2] = new Vector2((float)(vertexIndex2 % textureWidth) / textureWidth, (float)(vertexIndex2 / textureWidth) / textureHeight);
            uvs[vertexIndex3] = new Vector2((float)(vertexIndex3 % textureWidth) / textureWidth, (float)(vertexIndex3 / textureWidth) / textureHeight);
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
                StructureSpawnInfo n_spawnInfo = StructureSpawnData(n_structure);
                GameObject spawnedStructure = Instantiate(n_structure.structurePrefab, n_spawnInfo.position + new Vector3(0, n_structure.spawnAltitude, 0), n_spawnInfo.rotation);
                while (spawnedStructure.transform.position == invalidPosition)
                {
                    Debug.Log("Position resulted invalid, trying to find a new one");
                    n_spawnInfo = StructureSpawnData(n_structure);
                    spawnedStructure.transform.position = n_spawnInfo.position;
                    spawnedStructure.transform.rotation = n_spawnInfo.rotation;
                }
                if (n_structure.variableSize)
                {
                    float n_scale = Random.Range(n_structure.minSize, n_structure.maxSize);
                    spawnedStructure.transform.localScale = new Vector3(n_scale, n_scale, n_scale);
                }
                spawnedStructures.Add(spawnedStructure);
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

    StructureSpawnInfo StructureSpawnData(Structure n_structure)
    {
        Vector3 n_postion = new Vector3();
        Quaternion n_rotation = new Quaternion();
        Vector3 n_inAirPos = new Vector3(Random.Range(center - islandRadius, center + islandRadius), 500, Random.Range(center - islandRadius, center + islandRadius));
        if (Physics.Raycast(n_inAirPos, -Vector3.up, out RaycastHit hitpoint, 1000))
        {
            if (hitpoint.transform.gameObject.TryGetComponent<IslandGenerator>(out IslandGenerator n_island) && CheckValidBiome(n_inAirPos, n_structure.allowedBiomes))
            {
                Debug.Log("Valid position found");
                n_postion = hitpoint.point;
                if (!n_structure.ignoreSlopes)
                {
                    n_rotation = Quaternion.FromToRotation(Vector3.up, hitpoint.normal);
                    n_rotation *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                }
                else
                {
                    n_rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                }
                return new StructureSpawnInfo(n_postion, n_rotation);
            }
        }
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
