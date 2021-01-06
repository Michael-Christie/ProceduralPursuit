using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNature : MonoBehaviour
{
    public GameObject[] ExteriorTrees;
    public GameObject[] InteriorTrees;
    public GameObject[] plant;

    bool[,] NatureMap;
    bool[,] treeMap;
    bool[,] plantMap;

    public LayerMask groundMask;
    [Range(0,1f)]
    public float falloff = .5f;

    public GameObject c;

    public void GenerateNature(bool[,] buildingMap, bool[,] PathMap)
    {
        NatureMap = new bool[buildingMap.GetLength(0), buildingMap.GetLength(1)];

        for (int x = 0; x < NatureMap.GetLength(0); x++)
        {
            for (int y = 0; y < NatureMap.GetLength(1); y++)
            {
                if (!buildingMap[x, y])
                    NatureMap[x, y] = true;
                if (NatureMap[x, y] && PathMap[x, y])
                    NatureMap[x, y] = false;
            }
        }


        StartCoroutine(SpawnTrees());
        //SpawnPlants();
    }

    Queue<Coord> shuffledCoords;

    IEnumerator SpawnTrees()
    {

        yield return new WaitForSeconds(1.5f);
        treeMap = new bool[NatureMap.GetLength(0) * 4, NatureMap.GetLength(1) * 4];
        plantMap = new bool[NatureMap.GetLength(0) * 4, NatureMap.GetLength(1) * 4];
        //sets up the tree map array

        for (int x = 0; x < NatureMap.GetLength(0); x++)
        {
            for (int y = 0; y < NatureMap.GetLength(1); y++)
            {
                if (NatureMap[x, y])
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (Mathf.PerlinNoise((x * 4 + i) / 13f, (y * 4 + j) / 24f) > falloff)
                                treeMap[x * 4 + i, y * 4 + j] = true; //right
                            else
                                plantMap[x * 4 + i, y * 4 + j] = true;
                        }
                    }
                }
            }
        }

        yield return new WaitForEndOfFrame();

        //pick a possition
        List<Coord> treeList = new List<Coord>();
        for (int x = 0; x < treeMap.GetLength(0); x++)
        {
            for (int y = 0; y < treeMap.GetLength(1); y++)
            {
                if (treeMap[x, y])
                    treeList.Add(new Coord(x, y));
            }
        }
        shuffledCoords = new Queue<Coord>(Shuffle.ShuffleArray(treeList.ToArray()));

        int percentage = (int)(treeList.Count * .8f);
        for(int i = 0; i < percentage; i++)
        {
            Coord rnd = GetRandomCoord();

            if(treeMap[rnd.x, rnd.y])
            {
                Vector3 location = new Vector3(rnd.x * 2.5f + RandomNumber.Range(-1f, .1f), 60f, rnd.y * 2.5f + RandomNumber.Range(-1f, 1f));
                //spawn the tree there
                RaycastHit hit;
                Ray ray = new Ray(location, Vector3.down);
                Physics.Raycast(ray, out hit, 75f, groundMask);

                float distanceToCenter = Mathf.Abs((new Vector2(rnd.x, rnd.y) - new Vector2(treeMap.GetLength(0) / 2f, treeMap.GetLength(0) / 2f)).magnitude);

                GameObject v = Instantiate((distanceToCenter < 75f ? ExteriorTrees[RandomNumber.Range(0, ExteriorTrees.Length)] : InteriorTrees[RandomNumber.Range(0, InteriorTrees.Length)]), new Vector3(location.x, hit.point.y, location.z), (distanceToCenter < 75f ? Quaternion.Euler(-90, 0, 0) : Quaternion.identity), transform);
                treeMap[rnd.x, rnd.y] = false;
                v.layer = LayerMask.NameToLayer("Terrain");


                //remove surrounding areas so trees are seperated
                if (rnd.x - 1 >= 0)
                    treeMap[rnd.x - 1, rnd.y] = false;
                if (rnd.x + 1 < treeMap.GetLength(0))
                    treeMap[rnd.x + 1, rnd.y] = false;

                if (rnd.y - 1 >= 0)
                    treeMap[rnd.x, rnd.y - 1] = false;
                if (rnd.y + 1 < treeMap.GetLength(1))
                    treeMap[rnd.x, rnd.y + 1] = false;

                if (rnd.x - 1 >= 0 && rnd.y - 1 >= 0)
                    treeMap[rnd.x - 1, rnd.y - 1] = false;
                if (rnd.x + 1 < treeMap.GetLength(0) && rnd.y - 1 >= 0)
                    treeMap[rnd.x + 1, rnd.y - 1] = false;
                if (rnd.x - 1 >= 0 && rnd.y + 1 < treeMap.GetLength(1))
                    treeMap[rnd.x - 1, rnd.y + 1] = false;
                if (rnd.x + 1 < treeMap.GetLength(0) && rnd.y + 1 < treeMap.GetLength(1))
                    treeMap[rnd.x + 1, rnd.y + 1] = false;
            }

            if( i % 200 == 0)
                yield return new WaitForEndOfFrame();
        }



    }

    public Coord GetRandomCoord()
    {
        Coord rnd = shuffledCoords.Dequeue();
        shuffledCoords.Enqueue(rnd);
        return rnd;
    }

    void AddWater()
    {

    }

    void SpawnPlants()
    {
        for (int x = 0; x < plantMap.GetLength(0); x++)
        {
            for (int y = 0; y < plantMap.GetLength(1); y++)
            {
                if (plantMap[x, y])
                {
                    //create a flower
                    Vector3 location = new Vector3(x * 2.5f + RandomNumber.Range(-1f, .1f), 15f, y * 2.5f + RandomNumber.Range(-1f, 1f));
                    RaycastHit hit;
                    Ray ray = new Ray(location, Vector3.down);
                    Physics.Raycast(ray, out hit, 30f, groundMask);

                    GameObject v = Instantiate(plant[RandomNumber.Range(0, plant.Length)], new Vector3(location.x, hit.point.y, location.z), Quaternion.Euler(0, 0, 0), transform);
                }

            }
        }
    }

    public void Visualize(GameObject cube)
    {
        //for (int x = 0; x < NatureMap.GetLength(0); x++)
        //{
        //    for (int y = 0; y < NatureMap.GetLength(1); y++)
        //    {
        //        if (NatureMap[x, y])
        //        {
        //            GameObject v = Instantiate(trees[RandomNumber.Range(0, trees.Length)], new Vector3(x * 2.5f * 4f, .3f, y * 2.5f * 4f) - new Vector3(197, 0, 197), Quaternion.Euler(-90, 0, 0), transform);
        //            //v.transform.localScale *= 4;
        //        }
        //    }
        //}

        //for (int x = 0; x < treeMap.GetLength(0); x++)
        //{
        //    for (int y = 0; y < treeMap.GetLength(1); y++)
        //    {
        //        if (treeMap[x, y])
        //        {
        //            GameObject v = Instantiate(c, new Vector3(x * 2.5f, 10f, y * 2.5f), Quaternion.Euler(-90, 0, 0), transform);
        //        }
        //    }
        //}

        //transform.position = new Vector3(-201.5f, 0, -201.5f);
    }
}
