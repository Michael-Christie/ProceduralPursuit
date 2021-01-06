using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

public class TerrainGenerator : MonoBehaviour
{
    //The frequency of the noise
    static float frequency = 1f;
    //The lacunarity of the noise
    static float lacu = 2f;
    //The number of octaves
    static int octaves = 2;
    //The persistance of the noise
    static float persist = 1f;

    List<Coord> returnMap;
    public Terrain t;

    TerrainData terrainData;

    public static TerrainGenerator instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        terrainData = t.terrainData;
    }

    public List<Coord> GetMap()
    {
        return returnMap;
    }

    public IEnumerator Generate(bool[,] mapData, bool[,] pathMap, float radius)
    {
        Vector2 resolution = new Vector2(512 / mapData.GetLength(0), 512 / mapData.GetLength(1));
        float[,] data = new float[mapData.GetLength(0) * (int)resolution.x, mapData.GetLength(1) * (int)resolution.y];/* = noise.GetNormalizedData();*/

        returnMap = new List<Coord>();

        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
            {
                float height = .0001f;
                if (mapData[i, j])
                    height = 0;
                if (pathMap[i, j])
                    height = 0;

                if (i > 2 && j > 2 && i < mapData.GetLength(0) - 2 && j < mapData.GetLength(1) - 2)
                {
                    if (height > 0)
                    {
                        float distance = (new Vector2(i, j) - new Vector2(mapData.GetLength(0) * .5f, mapData.GetLength(1) * .5f)).magnitude;
                        if (distance < radius - 5f)
                        {


                            if ((mapData[i + 1, j] || pathMap[i + 1, j]) && (mapData[i - 1, j] || pathMap[i - 1, j]))
                            {
                                height = 0;
                                returnMap.Add(new Coord(i, j));
                                yield return new WaitForEndOfFrame();
                            }
                            else if ((mapData[i, j + 1] || pathMap[i, j + 1]) && (mapData[i, j + 1] || pathMap[i, j + 1]))
                            {
                                height = 0;
                                returnMap.Add(new Coord(i, j));
                            }

                        }
                    }
                }

                for (int x = 0; x < resolution.x; x++)
                {
                    for (int y = 0; y < resolution.y; y++)
                    {
                        data[j * (int)resolution.x + x, i * (int)resolution.y + y] = height;
                    }
                }

            }
        }

        yield return new WaitForEndOfFrame();

        Perlin p = new Perlin
        {
            Frequency = frequency,
            Persistence = persist,
            OctaveCount = octaves,
            Lacunarity = lacu
        };

        float radiusDepth = 0;

        //add noise to anything that isnt 0
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                float distance = (new Vector2(i / resolution.x, j / resolution.y) - new Vector2(mapData.GetLength(0) * .5f, mapData.GetLength(1) * .5f)).magnitude;
                if (distance > radius)
                {
                    radiusDepth = distance - radius + 1f;
                }
                else
                    radiusDepth = 1f;

                if (data[i, j] > 0)
                {
                    data[i, j] += (((float)p.GetValue(i / 54f, 0, j / 99f) + 1f) * .5f * radiusDepth) * .075f;
                }

            }
        }

        yield return new WaitForEndOfFrame();

        //.. and actually set the heights
        terrainData.SetHeights(0, 0, data);
    }

    public void ResetTerrain()
    {
        float[,] reset = new float[512, 512];
        terrainData.SetHeights(0, 0, reset);
    }
}
