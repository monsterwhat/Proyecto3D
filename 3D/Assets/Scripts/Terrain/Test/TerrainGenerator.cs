using UnityEngine;
using System.Threading.Tasks;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 512;
    public int depth = 512;
    public float scale = 20f;

    public int octaves = 5;
    public float persistence = 0.5f;
    public float lacunarity = 2f;

    public int seed = 0;

    public float minHeight = 0f;
    public float maxHeight = 100f;

    public float treeThreshold = 0.6f;
    public float grassThreshold = 0.4f;

    public GameObject[] treePrefabs;
    public GameObject[] grassPrefabs;

    private float[,] heightMap;

    private void Start()
    {
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        // Generate height map
        heightMap = HeightMapGenerator.GenerateHeightMap(width, depth, scale, octaves, persistence, lacunarity, seed);

        // Create terrain
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = TerrainGeneratorUtility.CreateTerrainData(heightMap, minHeight, maxHeight);

        // Spawn trees and grass
        SpawnTrees();
        SpawnGrass();
    }

    private void SpawnTrees()
    {
        foreach (GameObject treePrefab in treePrefabs)
        {
            int treeCount = 0;

            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    float height = heightMap[x, z];

                    if (height > treeThreshold)
                    {
                        Vector3 treePos = new Vector3(x, height, z);
                        Instantiate(treePrefab, treePos, Quaternion.identity);
                        treeCount++;
                    }
                }
            }

            Debug.Log("Spawned " + treeCount + " " + treePrefab.name + " trees.");
        }
    }

    private void SpawnGrass()
    {
        foreach (GameObject grassPrefab in grassPrefabs)
        {
            int grassCount = 0;

            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    float height = heightMap[x, z];

                    if (height > grassThreshold)
                    {
                        Vector3 grassPos = new Vector3(x, height, z);
                        Instantiate(grassPrefab, grassPos, Quaternion.identity);
                        grassCount++;
                    }
                }
            }

            Debug.Log("Spawned " + grassCount + " " + grassPrefab.name + " grass.");
        }
    }

}