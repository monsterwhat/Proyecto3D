using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    public LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshCollider>().convex = true;

        UpdateTerrain();
    }

    public void UpdateTerrain()
    {
        CreateShape();
        UpdateMesh();
        SpawnGrassAndFlowers();
        SpawnTrees();
        SpawnBuildings();
        GetComponent<MeshCollider>().convex = false;
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
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

    public GameObject[] grassAndFlowerPrefabs;

    public float spawnRate = 0.1f; // The rate of spawning grass and flowers

    void SpawnGrassAndFlowers()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // Randomly decide whether to spawn grass or flowers
            if (Random.value < spawnRate)
            {
                // Spawn a random prefab from the prefabs array
                GameObject prefabToSpawn = grassAndFlowerPrefabs[Random.Range(0, grassAndFlowerPrefabs.Length)];
                GameObject obj = Instantiate(prefabToSpawn);

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

    public GameObject[] treePrefabs;

    public float treeSpawnRate = 0.01f; // The rate of spawning trees

    void SpawnTrees()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // Randomly decide whether to spawn a tree
            if (Random.value < treeSpawnRate)
            {
                // Spawn a random tree prefab from the treePrefabs array
                GameObject treeToSpawn = treePrefabs[Random.Range(0, treePrefabs.Length)];
                GameObject obj = Instantiate(treeToSpawn);

                // Get the height of the terrain at the object's position
                RaycastHit hit;
                if (Physics.Raycast(vertices[i] + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, groundMask))
                {
                    // Set the object's position to the terrain height plus a small offset
                    obj.transform.position = new Vector3(vertices[i].x, hit.point.y, vertices[i].z);
                }
                else
                {
                    // If the raycast didn't hit the terrain, just position the object on the vertex with a small offset
                    obj.transform.position = vertices[i];
                }

                obj.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
                obj.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                obj.transform.parent = transform;
            }
        }
    }

    public GameObject[] buildingPrefabs;
    public float buildingSpawnRate = 0.005f; // The rate of spawning buildings

    void SpawnBuildings()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // Randomly decide whether to spawn a building
            if (Random.value < buildingSpawnRate)
            {
                // Spawn a random building prefab from the buildingPrefabs array
                GameObject buildingToSpawn = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                GameObject obj = Instantiate(buildingToSpawn);

                // Get the height of the terrain at the object's position
                RaycastHit hit;
                if (Physics.Raycast(vertices[i] + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, groundMask))
                {
                    // Set the object's position to the terrain height plus a small offset
                    obj.transform.position = new Vector3(vertices[i].x, hit.point.y, vertices[i].z);
                }
                else
                {
                    // If the raycast didn't hit the terrain, just position the object on the vertex with a small offset
                    obj.transform.position = vertices[i];
                }

                obj.transform.localScale = Vector3.one * Random.Range(1.0f, 3.0f);
                obj.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                obj.transform.parent = transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}