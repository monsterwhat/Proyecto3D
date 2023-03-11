using UnityEngine;

public static class TerrainGeneratorUtility
{
    public static void GenerateTerrainMesh(TerrainData terrainData, float[,] heightMap, int levelOfDetail, int textureDetail, out Mesh mesh, out Texture2D texture)
    {
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float heightScale = terrainData.size.y;

        // Generate vertices and UVs
        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uvs = new Vector2[width * height];
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++, i++)
            {
                float heightValue = heightMap[x, z];
                vertices[i] = new Vector3(x, heightValue * heightScale, z);
                uvs[i] = new Vector2((float)x / width, (float)z / height);
            }
        }

        // Generate triangles
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        for (int z = 0, ti = 0, vi = 0; z < height - 1; z++, vi++)
        {
            for (int x = 0; x < width - 1; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + width;
                triangles[ti + 5] = vi + width + 1;
            }
        }

        // Generate mesh
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        // Generate texture
        texture = new Texture2D(width / textureDetail, height / textureDetail);
        for (int z = 0, i = 0; z < height; z += textureDetail)
        {
            for (int x = 0; x < width; x += textureDetail, i++)
            {
                float heightValue = heightMap[x, z];
                Color color = terrainData.terrainLayers[0].diffuseTexture.GetPixelBilinear(x / (float)width, z / (float)height);
                float maxSteepness = 1f;
                float steepness = terrainData.GetSteepness(x / terrainData.size.x, z / terrainData.size.z);
                for (int j = 1; j < terrainData.terrainLayers.Length; j++)
                {
                    if (steepness < maxSteepness)
                    {
                        color = terrainData.terrainLayers[j].diffuseTexture.GetPixelBilinear(x/(float)width,z/(float)height);
                        break;
                    }
                }
                texture.SetPixel(i % texture.width, i / texture.width, color);
            }
        }
        texture.Apply();
    }

    public static void ApplyTerrainMeshAndTexture(GameObject terrainObject, Mesh mesh, Texture2D texture)
    {
        // Apply mesh
        MeshFilter meshFilter = terrainObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = terrainObject.AddComponent<MeshFilter>();
        }
        meshFilter.sharedMesh = mesh;

        // Apply texture
        MeshRenderer meshRenderer = terrainObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = terrainObject.AddComponent<MeshRenderer>();
        }
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        // Set the texture to the material
        meshRenderer.sharedMaterial.mainTexture = texture;

        // Calculate the normals
        mesh.RecalculateNormals();

        // Set the mesh to the mesh filter
        meshFilter.mesh = mesh;
    }
    
    public static void ApplyTextureToMesh(Texture2D texture, MeshFilter meshFilter, MeshRenderer meshRenderer)
        {
            // Get the mesh from the mesh filter
            Mesh mesh = meshFilter.sharedMesh;

            // Set the texture to the material
            meshRenderer.sharedMaterial.mainTexture = texture;

            // Calculate the normals
            mesh.RecalculateNormals();

            // Set the mesh to the mesh filter
            meshFilter.mesh = mesh;
        }

    public static TerrainData CreateTerrainData(float[,] heightMap, float heightScale, float heightOffset)
    {
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = heightMap.GetLength(0);
        terrainData.size = new Vector3(terrainData.heightmapResolution - 1, heightScale, terrainData.heightmapResolution - 1);
        terrainData.SetHeights(0, 0, NormalizeHeightMap(heightMap, heightOffset));

        return terrainData;
    }

    private static float[,] NormalizeHeightMap(float[,] heightMap, float heightOffset)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float[,] normalizedHeightMap = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                normalizedHeightMap[x, y] = (heightMap[x, y] + heightOffset) / heightOffset;
            }
        }

        return normalizedHeightMap;

    }
}

