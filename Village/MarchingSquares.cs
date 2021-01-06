using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MarchingSquares 
{
    const float scale = 3.33333f;

    public static Mesh Generate(bool[,] map, Coord centerPoint, float radius)
    {
        Mesh mesh = new Mesh();
        List<Vector3> points = new List<Vector3>();
        Vector2[] uvs;
        List<int> triangles = new List<int>();

        for (int i = centerPoint.x * 3 - (int)radius; i < centerPoint.x * 3 + radius - 1; i++)
        {
            for (int j = centerPoint.y * 3 - (int)radius; j < centerPoint.x * 3 + radius - 1; j++)
            {

                switch (CalculateValue(i, j, map))
                {
                    case 0:

                        break;
                    case 1:
                        points.Add(new Vector3(i, 0, j) * scale);
                        points.Add(new Vector3(i, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + .5f, 0, j) * scale);

                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);
                        break;
                    case 2:
                        points.Add(new Vector3(i + 1, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + 1, 0, j) * scale);
                        points.Add(new Vector3(i + .5f, 0, j) * scale);

                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);

                        break;
                    case 3:
                        points.Add(new Vector3(i, 0, j) * scale);
                        points.Add(new Vector3(i, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + 1, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + 1, 0, j) * scale);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);
                        break;
                    case 4:
                        points.Add(new Vector3(i + 1, 0, j + 1) * scale);
                        points.Add(new Vector3(i + 1, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + .5f, 0, j + 1) * scale);

                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);

                        break;
                    case 5:
                        //bottom left
                        points.Add(new Vector3(i, 0, j) * scale);
                        points.Add(new Vector3(i, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + .5f, 0, j) * scale);
                        //top right
                        points.Add(new Vector3(i + 1, 0, j + 1) * scale);
                        points.Add(new Vector3(i + 1, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + .5f, 0, j + 1) * scale);

                        triangles.Add(points.Count - 6);
                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 4);

                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);


                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 1);
                        triangles.Add(points.Count - 2);

                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 4);


                        break;
                    case 6:
                        points.Add(new Vector3(i + .5f, 0, j + 1) * scale);
                        points.Add(new Vector3(i + 1, 0, j + 1) * scale);
                        points.Add(new Vector3(i + 1, 0, j) * scale);
                        points.Add(new Vector3(i + .5f, 0, j) * scale);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);
                        break;
                    case 7:
                        points.Add(new Vector3(i, 0, j) * scale);
                        points.Add(new Vector3(i + 1, 0, j) * scale);
                        points.Add(new Vector3(i + 1, 0, j + 1) * scale);
                        points.Add(new Vector3(i, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + .5f, 0, j + 1) * scale);

                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 4);

                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 3);

                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);


                        break;
                    case 8:
                        points.Add(new Vector3(i, 0, j + 1) * scale);
                        points.Add(new Vector3(i + .5f, 0, j + 1) * scale);
                        points.Add(new Vector3(i, 0, j + .5f) * scale);

                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);
                        break;
                    case 9:
                        points.Add(new Vector3(i, 0, j) * scale);
                        points.Add(new Vector3(i + .5f, 0, j + 1) * scale);
                        points.Add(new Vector3(i + .5f, 0, j) * scale);
                        points.Add(new Vector3(i, 0, j + 1) * scale);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 1);
                        triangles.Add(points.Count - 3);
                        break;
                    case 10:
                        //top left
                        points.Add(new Vector3(i, 0, j + 1) * scale);
                        points.Add(new Vector3(i + .5f, 0, j + 1) * scale);
                        points.Add(new Vector3(i, 0, j + .5f) * scale);
                        //bottom right                        
                        points.Add(new Vector3(i + 1, 0, j) * scale);
                        points.Add(new Vector3(i + .5f, 0, j) * scale);
                        points.Add(new Vector3(i + 1, 0, j + .5f) * scale);

                        triangles.Add(points.Count - 6);
                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 4);

                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);

                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 1);
                        triangles.Add(points.Count - 2);

                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 4);


                        break;
                    case 11:
                        points.Add(new Vector3(i, 0, j) * scale);
                        points.Add(new Vector3(i + 1, 0, j) * scale);
                        points.Add(new Vector3(i, 0, j + 1) * scale);
                        points.Add(new Vector3(i + 1, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + .5f, 0, j + 1) * scale);

                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 4);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 1);

                        triangles.Add(points.Count - 1);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 4);

                        break;
                    case 12:
                        points.Add(new Vector3(i, 0, j + 1) * scale);
                        points.Add(new Vector3(i, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + 1, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + 1, 0, j + 1) * scale);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 3);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 1);
                        triangles.Add(points.Count - 2);


                        break;
                    case 13:
                        points.Add(new Vector3(i, 0, j) * scale);
                        points.Add(new Vector3(i, 0, j + 1) * scale);//4
                        points.Add(new Vector3(i + 1, 0, j + 1) * scale);
                        points.Add(new Vector3(i + 1, 0, j + .5f) * scale);
                        points.Add(new Vector3(i + .5f, 0, j) * scale);

                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 3);

                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);

                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 1);



                        break;
                    case 14:
                        points.Add(new Vector3(i + 1, 0, j + 1) * scale);
                        points.Add(new Vector3(i + 1, 0, j) * scale);
                        points.Add(new Vector3(i, 0, j + 1) * scale);
                        points.Add(new Vector3(i + .5f, 0, j) * scale);
                        points.Add(new Vector3(i, 0, j + .5f) * scale);

                        triangles.Add(points.Count - 5);
                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 3);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 1);
                        triangles.Add(points.Count - 3);

                        triangles.Add(points.Count - 1);
                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 2);
                        break;
                    case 15:
                        points.Add(new Vector3(i, 0, j) * scale);
                        points.Add(new Vector3(i, 0, j + 1) * scale);
                        points.Add(new Vector3(i + 1, 0, j + 1) * scale);
                        points.Add(new Vector3(i + 1, 0, j) * scale);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 3);
                        triangles.Add(points.Count - 2);

                        triangles.Add(points.Count - 4);
                        triangles.Add(points.Count - 2);
                        triangles.Add(points.Count - 1);
                        break;
                }
            }
        }

        uvs = new Vector2[points.Count];
        List<Vector3> normal = new List<Vector3>();

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(points[i].x * .5f , points[i].z * .3f);
            normal.Add(Vector3.up);
        }

        mesh.SetVertices(points);
        mesh.SetTriangles(triangles, 0);
        mesh.SetNormals(normal);
        mesh.SetUVs(0, uvs);

        return mesh;
    }

    static int CalculateValue(int x, int z, bool[,] map)
    {
        int value = 0;

        //where a = x y
        //D - C
        //|   |
        //A - B

        //corner A
        if (map[x,z])
            value += 1;
        //corner B
        if (x + 1 < map.GetLength(0))
        {
            if (map[(x + 1), z])
                value += 2;
        }
        else
            value += 2;
        //corner D
        if (z + 1 < map.GetLength(1))
        {
            if (map[x, (z + 1)])
                value += 8;
        }
        else
            value += 8;
        //corner C
        if (x + 1 < map.GetLength(0) || z + 1 < map.GetLength(1))
        {
            if (map[(x + 1), (z + 1)])
                value += 4;
        }
        else if (x + 1 >= map.GetLength(0) && z + 1 >= map.GetLength(1))
            value += 4;

        return value;
    }
}
