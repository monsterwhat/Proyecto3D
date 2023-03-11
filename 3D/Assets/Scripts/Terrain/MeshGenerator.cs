using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    [Header("Terrain Settings")]
    public int xSize = 20;
    public int zSize = 20;

    public LayerMask groundMask;

    [Header("Generation Settings")]
    public float perlinScale = 0.05f;
    public float simplexScale = 0.5f;

    [Header("SpawnRates")]
    public float hollowObjectSpawnRate = 0.1f; // The rate of spawning hollow objects
    public float solidObjectSpawnRate = 0.01f; // The rate of spawning solid objects

    [Header("Probabilities")]
    public float[] solidObjectProbabilities;
    public float[] hollowObjectProbabilities;

    [Header("Prefabs")]
    public GameObject[] hollowObjectPrefabs;
    public GameObject[] solidObjectPrefabs;



    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshCollider>().convex = true;
        groundMask = LayerMask.NameToLayer("GroundLayer");

        UpdateTerrain();
    }

    public void UpdateTerrain()
    {
        CreateShape();
        UpdateMesh();
        SpawnSolidObjects();
        SpawnHollowObjects();
        GetComponent<MeshCollider>().convex = false;
    }

    void CreateShape()
    {
        float perlinScale = 0.05f;
        float simplexScale = 0.5f;
        float maxHeight = 2.0f;
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float perlin = Mathf.PerlinNoise(x * perlinScale, z * perlinScale);
                float simplex = Mathf.PerlinNoise(x * simplexScale, z * simplexScale);
                float y = perlin + (simplex * 0.5f);
                y *= maxHeight;
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

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, groundMask))
        {
            transform.position = hit.point;
        }
    }

    void SpawnSolidObjects()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // Randomly decide whether to spawn a solid object
            if (Random.value < solidObjectSpawnRate)
            {
                // Randomly choose a solid object prefab based on its probability
                float totalProbability = 0f;
                for (int j = 0; j < solidObjectPrefabs.Length; j++)
                {
                    totalProbability += solidObjectProbabilities[j];
                }

                float randomValue = Random.value * totalProbability;
                int chosenIndex = 0;
                float cumulativeProbability = solidObjectProbabilities[chosenIndex];

                while (randomValue > cumulativeProbability && chosenIndex < solidObjectPrefabs.Length - 1)
                {
                    chosenIndex++;
                    cumulativeProbability += solidObjectProbabilities[chosenIndex];
                }

                GameObject solidObjectToSpawn = solidObjectPrefabs[chosenIndex];

                GameObject obj = Instantiate(solidObjectToSpawn);

                // Get the height of the terrain at the object's position
                RaycastHit hit;
                if (Physics.Raycast(vertices[i] + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, groundMask))
                {
                    // Set the object's position to the terrain height plus a small offset
                    obj.transform.position = new Vector3(vertices[i].x, hit.point.y + 0.05f, vertices[i].z);
                }
                else
                {
                    // If the raycast didn't hit the terrain, just position the object on the vertex with a small offset
                    obj.transform.position = vertices[i] + Vector3.up * 0.05f;
                }

                obj.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
                obj.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                obj.transform.parent = transform;
            }
        }
    }

    void SpawnHollowObjects()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // Randomly decide whether to spawn a hollow object
            if (Random.value < hollowObjectSpawnRate)
            {
                float randomProb = Random.value;
                int j = 0;
                while (j < hollowObjectPrefabs.Length && randomProb > hollowObjectProbabilities[j])
                {
                    randomProb -= hollowObjectProbabilities[j];
                    j++;
                }

                // Spawn the selected hollow object prefab from the hollowObjectPrefabs array
                GameObject obj = Instantiate(hollowObjectPrefabs[j]);

                // Get the height of the terrain at the object's position
                RaycastHit hit;
                if (Physics.Raycast(vertices[i] + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, groundMask))
                {
                    // Set the object's position to the terrain height plus a small offset
                    obj.transform.position = new Vector3(vertices[i].x, hit.point.y + 0.05f, vertices[i].z);
                }
                else
                {
                    // If the raycast didn't hit the terrain, just position the object on the vertex with a small offset
                    obj.transform.position = vertices[i] + Vector3.up * 0.05f;
                }

                obj.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
                obj.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                obj.transform.parent = transform;
                Destroy(obj.GetComponent<Collider>()); // Remove the collider component
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
    }
}