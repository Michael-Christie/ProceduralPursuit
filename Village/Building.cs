using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public ColorScheme identifier;
    public float distanceToCenter;
    public bool containsUniqueBuilding = false;

    public enum Space
    {
        outside,
        free,
        taken,
        edge,
        corner,
        insideCorner
    }

    public VillageBuildingList VB;
    public bool animate;

    public List<Coord> tiles = new List<Coord>();

    public void AddCoord(Coord c) => tiles.Add(c);

    bool[,] buildingOutline;
    Space[,] interior;

    int MinX, MinY;

    public Vector3 areaSize;

    public int Size => tiles.Count;

    public delegate void RiseBuildings();
    public RiseBuildings RunAnimation;

    public Vector3 Location => new Vector3(MinX * 10f, 0, MinY * 10f);

    public enum Function
    {
        none,
        Living,
        Ruin,
        Castle,
        Envioment
    }

    public Function functionType;

    public void Visualize(GameObject c)
    {

        //builds the tile map, to indicate areas for building spawning.
        for (int x = 0; x < interior.GetLength(0); x++)
        {
            for (int y = 0; y < interior.GetLength(1); y++)
            {
                if (interior[x, y] == Space.outside)
                    continue;
                //creates the plane
                GameObject g = Instantiate(c, new Vector3(x * 2.5f + MinX * 10f, 0, y * 2.5f + MinY * 10f), Quaternion.Euler(90, 0, 0), transform);

                g.name = "tile plane";

                MeshRenderer m = g.GetComponent<MeshRenderer>();

                //colors the plane depending upon the space value
                switch (interior[x, y])
                {
                    //case Space.outside:
                    //    m.material.color = Color.blue;
                    //        break;
                    case Space.free:
                        m.material.color = Color.green;
                        break;
                    case Space.edge:
                        m.material.color = Color.red;
                        break;
                    case Space.taken:
                        m.material.color = Color.grey;
                        break;
                    case Space.corner:
                        m.material.color = Color.blue;
                        break;
                    default:
                        m.material.color = Color.white;
                        break;
                }

            }
        }
    }

    public void DecideFunction(int min, int currIndex, int MaxBuildings)
    {
        if (functionType == Function.none)
        {
            if (/*Size < min + (min * .3f) || */distanceToCenter > 18f || currIndex == MaxBuildings - 1)
                functionType = Function.Ruin;
            else
                functionType = Function.Living;
        }
    }

    //Builds the whole house
    public void Build(VillageBuildingList vb, bool _animate)
    {
        animate = _animate;
        //all the buildings
        VB = vb;
        //creates the 2d map
        SetUpMap();

    }

    public void ShowAnimation()
    {
        RunAnimation();
    }

    public int CreateUnique(int bNum)
    {
        VillageBuildingList.Building[] posibleBuildings = VB.buildingList[VB.buildingList.Length - 1].buildings;
        int num = 0;
        Vector2 size = new Vector2(1, 1);

        if (bNum == 1 && Size > 150f)
            return 0;

        switch (bNum)
        {
            case 1:
                size = posibleBuildings[1].objectSize;
                num = 1;
                break;
            case 2:
                size = posibleBuildings[2].objectSize;
                num = 2;
                break;
            case 3:
                size = posibleBuildings[0].objectSize;
                num = 0;
                break;
        }

        List<Coord> edges = new List<Coord>();

        for (int x = 0; x < interior.GetLength(0); x++)
        {
            for (int y = 0; y < interior.GetLength(1); y++)
            {
                if (interior[x, y] == Space.edge)
                    edges.Add(new Coord(x, y));
            }
        }

        int breakLoop = 0;

        shuffledCoords = new Queue<Coord>(Shuffle.ShuffleArray(edges.ToArray()));

        for (int k = 0; k < edges.Count; k++)
        {
            Coord rnd = GetRandomCoord();

            bool[] b = FreeSpace(rnd.x, rnd.y, size);

            //if the building fits in the space
            if (b[0])
            {
                GameObject h;
                if (b[3])
                    //spawn that building in the space
                    h = Instantiate(posibleBuildings[num].prefab, new Vector3(rnd.x * 2.5f + MinX * 10f + (b[1] ? -size.x - (size.x % 2 == 1 ? .5f : 0) : size.x + (size.x % 2 == 1 ? .5f : 0)), (animate? -45 : 0), rnd.y * 2.5f + MinY * 10f + (b[2] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0))) + (b[1] ? posibleBuildings[num].posX : posibleBuildings[num].negX), Quaternion.Euler(FindEdgeDirection(rnd.x, rnd.y)), transform);
                else
                    h = Instantiate(posibleBuildings[num].prefab, new Vector3(rnd.x * 2.5f + MinX * 10f + (b[1] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0)), (animate? -45 : 0), rnd.y * 2.5f + MinY * 10f + (b[2] ? -size.x + (size.x % 2 == 1 ? .5f : 0) : size.x - (size.x % 2 == 1 ? .5f : 0))) + (b[2] ? posibleBuildings[num].posY : posibleBuildings[num].negY), Quaternion.Euler(FindEdgeDirection(rnd.x, rnd.y)), transform);
                //if (!b[3])
                //{
                //    h.transform.Rotate(posibleBuildings[num].rotationSide, -90f);
                //}

                if (num == 2)
                    h.name = "Barracks";

                if (num == 1)
                    functionType = Function.Castle;

                if (animate)
                {
                    Raise rise = h.AddComponent<Raise>();

                    rise.timeToWait = RandomNumber.Range(0f, 2f);//((x + y) / 4f) / 10f;
                    RunAnimation += rise.StartRaise;
                }

                BuildingVariation v = h.transform.GetChild(0).GetComponent<BuildingVariation>();
                if (v)
                {
                    v.roofColor = identifier.roof;
                    v.woodColor = identifier.wood;
                    v.Customize();
                }


                //assign all the tiles as taken
                for (int i = 0; i < (b[3] ? size.x : size.y); i++)
                {
                    for (int j = 0; j < (b[3] ? size.y : size.x); j++)
                    {
                        interior[(b[1] ? rnd.x - i : rnd.x + i), (b[2] ? rnd.y - j : rnd.y + j)] = Space.taken;
                    }
                }

                return bNum;
            }

            if (breakLoop > 100)
                return -1;
        }

        //for (int x = 0; x < interior.GetLength(0); x++)
        //{
        //    for (int y = 0; y < interior.GetLength(1); y++)
        //    {
        //        if ((interior[x, y] == Space.edge && posibleBuildings[num].SpawnsOnEdge) || (interior[x, y] == Space.corner && posibleBuildings[num].SpawnInCorner))
        //        {

        //            bool[] b = FreeSpace(x, y, size);

        //            //if the building fits in the space
        //            if (b[0])
        //            {
        //                GameObject h;
        //                if (b[3])
        //                    //spawn that building in the space
        //                    h = Instantiate(posibleBuildings[num].prefab, new Vector3(x * 2.5f + MinX * 10f + (b[1] ? -size.x - (size.x % 2 == 1 ? .5f : 0) : size.x + (size.x % 2 == 1 ? .5f : 0)), -45, y * 2.5f + MinY * 10f + (b[2] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0))) + (b[1] ? posibleBuildings[num].posX : posibleBuildings[num].negX), Quaternion.Euler(FindEdgeDirection(x, y)), transform);
        //                else
        //                    h = Instantiate(posibleBuildings[num].prefab, new Vector3(x * 2.5f + MinX * 10f + (b[1] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0)), -45, y * 2.5f + MinY * 10f + (b[2] ? -size.x + (size.x % 2 == 1 ? .5f : 0) : size.x - (size.x % 2 == 1 ? .5f : 0))) + (b[2] ? posibleBuildings[num].posY : posibleBuildings[num].negY), Quaternion.Euler(FindEdgeDirection(x, y)), transform);
        //                //if (!b[3])
        //                //{
        //                //    h.transform.Rotate(posibleBuildings[num].rotationSide, -90f);
        //                //}

        //                Raise rise = h.AddComponent<Raise>();

        //                if (num == 1)
        //                    functionType = Function.Castle;

        //                rise.timeToWait = RandomNumber.Range(0f, 2f);//((x + y) / 4f) / 10f;
        //                rise.StartRaise();

        //                BuildingVariation v = h.transform.GetChild(0).GetComponent<BuildingVariation>();
        //                if (v)
        //                {
        //                    v.roofColor = identifier.roof;
        //                    v.woodColor = identifier.wood;
        //                    v.Customize();
        //                }


        //                //assign all the tiles as taken
        //                for (int i = 0; i < (b[3] ? size.x : size.y); i++)
        //                {
        //                    for (int j = 0; j < (b[3] ? size.y : size.x); j++)
        //                    {
        //                        interior[(b[1] ? x - i : x + i), (b[2] ? y - j : y + j)] = Space.taken;
        //                    }
        //                }

        //                return bNum;
        //            }
        //        }
        //    }
        //}

        return 0;
    }

    Queue<Coord> shuffledCoords;

    //Spawns all the buildings
    public void Create()
    {
        //pull the list based upon what the function of the tile is.
        VillageBuildingList.Building[] posibleBuildings = VB.buildingList[(int)functionType - 1].buildings;

        //int temp = 11;

        List<Coord> cornerTiles = new List<Coord>();
        List<Coord> edgeTiles = new List<Coord>();
        List<Coord> interiorEdge = new List<Coord>();


        for (int x = 0; x < interior.GetLength(0); x++)
        {
            for (int y = 0; y < interior.GetLength(1); y++)
            {
                if (interior[x, y] == Space.corner)
                    cornerTiles.Add(new Coord(x, y));
                if (interior[x, y] == Space.edge)
                    edgeTiles.Add(new Coord(x, y));
                if (interior[x, y] == Space.insideCorner)
                    interiorEdge.Add(new Coord(x, y));
            }
        }


        shuffledCoords = new Queue<Coord>(Shuffle.ShuffleArray(edgeTiles.ToArray()));


        //if its a castle, try the first two buildings first
        if (functionType == Function.Castle)
        {
            for (int k = 0; k < edgeTiles.Count; k++)
            {

                Coord rnd = GetRandomCoord();

                for (int r = 0; r < 2; r++)
                {
                    //take a random building object
                    int num = r;
                    Vector2 size = posibleBuildings[num].objectSize;

                    if (!posibleBuildings[num].SpawnsOnEdge)
                        continue;

                    bool[] b = FreeSpace(rnd.x, rnd.y, size);

                    //if the building fits in the space
                    if (b[0])
                    {
                        GameObject h;
                        if (b[3])
                            //spawn that building in the space 
                            h = Instantiate(posibleBuildings[num].prefab, new Vector3(rnd.x * 2.5f + MinX * 10f + (b[1] ? -size.x - (size.x % 2 == 1 ? .5f : 0) : size.x + (size.x % 2 == 1 ? .5f : 0)), (animate ? -45 : 0), rnd.y * 2.5f + MinY * 10f + (b[2] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0))) + (b[1] ? posibleBuildings[num].posX : posibleBuildings[num].negX), Quaternion.Euler(FindEdgeDirection(rnd.x, rnd.y)), transform);
                        else
                            h = Instantiate(posibleBuildings[num].prefab, new Vector3(rnd.x * 2.5f + MinX * 10f + (b[1] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0)), (animate ? -45 : 0), rnd.y * 2.5f + MinY * 10f + (b[2] ? -size.x + (size.x % 2 == 1 ? .5f : 0) : size.x - (size.x % 2 == 1 ? .5f : 0))) + (b[2] ? posibleBuildings[num].posY : posibleBuildings[num].negY), Quaternion.Euler(FindEdgeDirection(rnd.x, rnd.y)), transform);
                        //if (!b[3])
                        //{
                        //    h.transform.Rotate(posibleBuildings[num].rotationSide, 90f);
                        //}


                        if (animate)
                        {
                            Raise rise = h.AddComponent<Raise>();

                            rise.timeToWait = RandomNumber.Range(2f, 5f); //((x + y) / 4f) / 10f;
                            RunAnimation += rise.StartRaise;
                        }

                        BuildingVariation v = h.transform.GetChild(0).GetComponent<BuildingVariation>();

                        if (v)
                        {
                            v.roofColor = identifier.roof;
                            v.woodColor = identifier.wood;
                            v.Customize();
                        }


                        //assign all the tiles as taken
                        for (int i = 0; i < (b[3] ? size.x : size.y); i++)
                        {
                            for (int j = 0; j < (b[3] ? size.y : size.x); j++)
                            {
                                interior[(b[1] ? rnd.x - i : rnd.x + i), (b[2] ? rnd.y - j : rnd.y + j)] = Space.taken;
                            }
                        }

                        k = edgeTiles.Count;
                        break;

                    }

                }

                if (k == edgeTiles.Count)
                    break;
            }
        }

        //do the corners
        shuffledCoords = new Queue<Coord>(Shuffle.ShuffleArray(cornerTiles.ToArray()));
        for (int k = 0; k < cornerTiles.Count; k++)
        {
            Coord rnd = GetRandomCoord();

            if (interior[rnd.x, rnd.y] != Space.corner)
                continue;

            for (int r = 0; r < 100; r++)
            {
                //take a random building object
                int num = /*temp % posibleBuildings.Length;*/RandomNumber.Range(0, posibleBuildings.Length);
                Vector2 size = posibleBuildings[num].objectSize;

                if (!posibleBuildings[num].SpawnInCorner)
                    continue;

                bool[] b = FreeSpace(rnd.x, rnd.y, size);

                //if the building fits in the space
                if (b[0])
                {
                    GameObject h;
                    if (b[3])
                        //spawn that building in the space
                        h = Instantiate(posibleBuildings[num].prefab, new Vector3(rnd.x * 2.5f + MinX * 10f + (b[1] ? -size.x - (size.x % 2 == 1 ? .5f : 0) : size.x + (size.x % 2 == 1 ? .5f : 0)), (animate ? -45 : 0), rnd.y * 2.5f + MinY * 10f + (b[2] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0))) + (b[1] ? posibleBuildings[num].posX : posibleBuildings[num].negX), Quaternion.Euler(FindEdgeDirection(rnd.x, rnd.y)), transform);
                    else
                        h = Instantiate(posibleBuildings[num].prefab, new Vector3(rnd.x * 2.5f + MinX * 10f + (b[1] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0)), (animate ? -45 : 0), rnd.y * 2.5f + MinY * 10f + (b[2] ? -size.x + (size.x % 2 == 1 ? .5f : 0) : size.x - (size.x % 2 == 1 ? .5f : 0))) + (b[2] ? posibleBuildings[num].posY : posibleBuildings[num].negY), Quaternion.Euler(FindEdgeDirection(rnd.x, rnd.y)), transform);
                    //if (!b[3])
                    //{
                    //    h.transform.Rotate(posibleBuildings[num].rotationSide, -90f);
                    //}

                    if (animate)
                    {
                        Raise rise = h.AddComponent<Raise>();

                        rise.timeToWait = RandomNumber.Range(0f, 2f);//((x + y) / 4f) / 10f;
                        RunAnimation += rise.StartRaise;
                    }

                    if (h.transform.childCount > 0)
                    {
                        BuildingVariation v = h.transform.GetChild(0).GetComponent<BuildingVariation>();
                        if (v)
                        {
                            v.roofColor = identifier.roof;
                            v.woodColor = identifier.wood;
                            v.Customize();
                        }
                    }


                    //assign all the tiles as taken
                    for (int i = 0; i < (b[3] ? size.x : size.y); i++)
                    {
                        for (int j = 0; j < (b[3] ? size.y : size.x); j++)
                        {
                            interior[(b[1] ? rnd.x - i : rnd.x + i), (b[2] ? rnd.y - j : rnd.y + j)] = Space.taken;
                        }
                    }



                }
            }
        }

        //spawn corrners
        //for (int x = 0; x < interior.GetLength(0); x++)
        //{
        //    for (int y = 0; y < interior.GetLength(1); y++)
        //    {
        //        if (interior[x, y] == Space.corner)
        //        {
        //            //try 100 times to get a house that fits in the space
        //            for (int r = 0; r < 100; r++)
        //            {
        //                //take a random building object
        //                int num = /*temp % posibleBuildings.Length;*/RandomNumber.Range(0, posibleBuildings.Length);
        //                Vector2 size = posibleBuildings[num].objectSize;

        //                if (!posibleBuildings[num].SpawnInCorner)
        //                    continue;

        //                bool[] b = FreeSpace(x, y, size);

        //                //if the building fits in the space
        //                if (b[0])
        //                {
        //                    GameObject h;
        //                    if (b[3])
        //                        //spawn that building in the space
        //                        h = Instantiate(posibleBuildings[num].prefab, new Vector3(x * 2.5f + MinX * 10f + (b[1] ? -size.x - (size.x % 2 == 1 ? .5f : 0) : size.x + (size.x % 2 == 1 ? .5f : 0)), -45, y * 2.5f + MinY * 10f + (b[2] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0))) + (b[1] ? posibleBuildings[num].posX : posibleBuildings[num].negX), Quaternion.Euler(FindEdgeDirection(x, y)), transform);
        //                    else
        //                        h = Instantiate(posibleBuildings[num].prefab, new Vector3(x * 2.5f + MinX * 10f + (b[1] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0)), -45, y * 2.5f + MinY * 10f + (b[2] ? -size.x + (size.x % 2 == 1 ? .5f : 0) : size.x - (size.x % 2 == 1 ? .5f : 0))) + (b[2] ? posibleBuildings[num].posY : posibleBuildings[num].negY), Quaternion.Euler(FindEdgeDirection(x, y)), transform);
        //                    //if (!b[3])
        //                    //{
        //                    //    h.transform.Rotate(posibleBuildings[num].rotationSide, -90f);
        //                    //}

        //                    Raise rise = h.AddComponent<Raise>();

        //                    rise.timeToWait = RandomNumber.Range(0f, 2f);//((x + y) / 4f) / 10f;
        //                    rise.StartRaise();

        //                    BuildingVariation v = h.transform.GetChild(0).GetComponent<BuildingVariation>();
        //                    if (v)
        //                    {
        //                        v.roofColor = identifier.roof;
        //                        v.woodColor = identifier.wood;
        //                        v.Customize();
        //                    }


        //                    //assign all the tiles as taken
        //                    for (int i = 0; i < (b[3] ? size.x : size.y); i++)
        //                    {
        //                        for (int j = 0; j < (b[3] ? size.y : size.x); j++)
        //                        {
        //                            interior[(b[1] ? x - i : x + i), (b[2] ? y - j : y + j)] = Space.taken;
        //                        }
        //                    }


        //                }
        //            }
        //        }
        //    }
        //}

        shuffledCoords = new Queue<Coord>(Shuffle.ShuffleArray(edgeTiles.ToArray()));
        //do the edges
        for (int k = 0; k < edgeTiles.Count; k++)
        {
            Coord rnd = GetRandomCoord();

            for (int r = 0; r < 100; r++)
            {
                //take a random building object
                int num = /*temp % posibleBuildings.Length;*/RandomNumber.Range(0, posibleBuildings.Length);
                Vector2 size = posibleBuildings[num].objectSize;

                if (!posibleBuildings[num].SpawnsOnEdge)
                    continue;

                bool[] b = FreeSpace(rnd.x, rnd.y, size);

                //if the building fits in the space
                if (b[0])
                {
                    GameObject h;
                    if (b[3])
                        //spawn that building in the space 
                        h = Instantiate(posibleBuildings[num].prefab, new Vector3(rnd.x * 2.5f + MinX * 10f + (b[1] ? -size.x - (size.x % 2 == 1 ? .5f : 0) : size.x + (size.x % 2 == 1 ? .5f : 0)), (animate ? -45 : 0), rnd.y * 2.5f + MinY * 10f + (b[2] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0))) + (b[1] ? posibleBuildings[num].posX : posibleBuildings[num].negX), Quaternion.Euler(FindEdgeDirection(rnd.x, rnd.y)), transform);
                    else
                        h = Instantiate(posibleBuildings[num].prefab, new Vector3(rnd.x * 2.5f + MinX * 10f + (b[1] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0)), (animate ? -45 : 0), rnd.y * 2.5f + MinY * 10f + (b[2] ? -size.x + (size.x % 2 == 1 ? .5f : 0) : size.x - (size.x % 2 == 1 ? .5f : 0))) + (b[2] ? posibleBuildings[num].posY : posibleBuildings[num].negY), Quaternion.Euler(FindEdgeDirection(rnd.x, rnd.y)), transform);
                    //if (!b[3])
                    //{
                    //    h.transform.Rotate(posibleBuildings[num].rotationSide, 90f);
                    //}

                    if (animate)
                    {
                        Raise rise = h.AddComponent<Raise>();

                        rise.timeToWait = RandomNumber.Range(2f, 5f); //((x + y) / 4f) / 10f;
                        RunAnimation += rise.StartRaise;
                    }

                    if (h.transform.childCount > 0)
                    {
                        BuildingVariation v = h.transform.GetChild(0).GetComponent<BuildingVariation>();

                        if (v)
                        {
                            v.roofColor = identifier.roof;
                            v.woodColor = identifier.wood;
                            v.Customize();
                        }
                    }


                    //assign all the tiles as taken
                    for (int i = 0; i < (b[3] ? size.x : size.y); i++)
                    {
                        for (int j = 0; j < (b[3] ? size.y : size.x); j++)
                        {
                            interior[(b[1] ? rnd.x - i : rnd.x + i), (b[2] ? rnd.y - j : rnd.y + j)] = Space.taken;
                        }
                    }


                }
            }
        }


        ////spawns edges
        //for (int x = 0; x < interior.GetLength(0); x++)
        //{
        //    for (int y = 0; y < interior.GetLength(1); y++)
        //    {

        //        if (interior[x, y] == Space.edge)
        //        {
        //            //try 100 times to get a house that fits in the space
        //            for (int r = 0; r < 100; r++)
        //            {
        //                //take a random building object
        //                int num = /*temp % posibleBuildings.Length;*/RandomNumber.Range(0, posibleBuildings.Length);
        //                Vector2 size = posibleBuildings[num].objectSize;

        //                if (!posibleBuildings[num].SpawnsOnEdge)
        //                    continue;

        //                bool[] b = FreeSpace(x, y, size);

        //                //if the building fits in the space
        //                if (b[0])
        //                {
        //                    GameObject h;
        //                    if (b[3])
        //                        //spawn that building in the space 
        //                        h = Instantiate(posibleBuildings[num].prefab, new Vector3(x * 2.5f + MinX * 10f + (b[1] ? -size.x - (size.x % 2 == 1 ? .5f : 0) : size.x + (size.x % 2 == 1 ? .5f : 0)), -45, y * 2.5f + MinY * 10f + (b[2] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0))) + (b[1] ? posibleBuildings[num].posX : posibleBuildings[num].negX), Quaternion.Euler(FindEdgeDirection(x, y)), transform);
        //                    else
        //                        h = Instantiate(posibleBuildings[num].prefab, new Vector3(x * 2.5f + MinX * 10f + (b[1] ? -size.y + (size.y % 2 == 1 ? .5f : 0) : size.y - (size.y % 2 == 1 ? .5f : 0)), -45, y * 2.5f + MinY * 10f + (b[2] ? -size.x + (size.x % 2 == 1 ? .5f : 0) : size.x - (size.x % 2 == 1 ? .5f : 0))) + (b[2] ? posibleBuildings[num].posY : posibleBuildings[num].negY), Quaternion.Euler(FindEdgeDirection(x, y)), transform);
        //                    //if (!b[3])
        //                    //{
        //                    //    h.transform.Rotate(posibleBuildings[num].rotationSide, 90f);
        //                    //}

        //                    Raise rise = h.AddComponent<Raise>();

        //                    rise.timeToWait = RandomNumber.Range(2f, 5f); //((x + y) / 4f) / 10f;
        //                    rise.StartRaise();

        //                    BuildingVariation v = h.transform.GetChild(0).GetComponent<BuildingVariation>();

        //                    if (v)
        //                    {
        //                        v.roofColor = identifier.roof;
        //                        v.woodColor = identifier.wood;
        //                        v.Customize();
        //                    }


        //                    //assign all the tiles as taken
        //                    for (int i = 0; i < (b[3] ? size.x : size.y); i++)
        //                    {
        //                        for (int j = 0; j < (b[3] ? size.y : size.x); j++)
        //                        {
        //                            interior[(b[1] ? x - i : x + i), (b[2] ? y - j : y + j)] = Space.taken;
        //                        }
        //                    }


        //                }
        //            }
        //        }
        //    }
        //}

        //if (globalBreeak > 20)
        //{
        //    Debug.Log("Broke", gameObject);
        //    yield break;
        //}

        //if (transform.childCount < 2 && globalBreeak < 20)
        //{
        //    globalBreeak++;
        //    Rebuild();
        //}

        //for (int k = 0; k < edgeTiles.Count; k++)
        //{
        //    Coord rnd = GetRandomCoord();

        //    if(interior[rnd.x, rnd.y] == Space.edge)
        //    {
        //        bool XRotate = false;
        //        if (rnd.x - 1 < 0 || rnd.x + 1 >= interior.GetLength(0))
        //            XRotate = true;
        //        if (!XRotate)
        //        {
        //            if (interior[rnd.x - 1, rnd.y] == Space.free || interior[rnd.x - 1, rnd.y] == Space.outside)
        //                XRotate = true;
        //            if(interior[rnd.x + 1, rnd.y] == Space.free || interior[rnd.x + 1, rnd.y] == Space.outside)
        //                XRotate = true;
        //        }
        //        Instantiate(VB.wall, new Vector3(rnd.x * 2.5f + MinX * 10f, 0, rnd.y * 2.5f + MinY * 10f), (XRotate ? Quaternion.Euler(new Vector3(0,-90,0)) : Quaternion.identity), transform);
        //    }
        //}

        if (functionType != Function.Ruin)
        {

            foreach (Coord c in interiorEdge)
            {
                if (interior[c.x, c.y] == Space.insideCorner)
                {
                    int sidesTaken = 0;
                    if (interior[c.x + 1, c.y] == Space.taken)
                        sidesTaken++;
                    if (interior[c.x - 1, c.y] == Space.taken)
                        sidesTaken++;
                    if (interior[c.x, c.y + 1] == Space.taken)
                        sidesTaken++;
                    if (interior[c.x, c.y - 1] == Space.taken)
                        sidesTaken++;

                    if (sidesTaken == 2)
                    {
                        GameObject h = Instantiate(VB.cornerWall, new Vector3(c.x * 2.5f + MinX * 10f, (animate? -25f:0), c.y * 2.5f + MinY * 10f), Quaternion.identity, transform);

                        if (animate)
                        {
                            Raise rise = h.AddComponent<Raise>();

                            rise.timeToWait = RandomNumber.Range(0f, 2f);//((x + y) / 4f) / 10f;
                            RunAnimation += rise.StartRaise;
                        }
                    }
                }
            }
        }


    }

    int globalBreeak = 0;

    public Coord GetRandomCoord()
    {
        Coord rnd = shuffledCoords.Dequeue();
        shuffledCoords.Enqueue(rnd);
        return rnd;
    }

    void Rebuild()
    {
        int num = -1;
        if (containsUniqueBuilding)
        {
            //castle
            if (transform.Find("Unique Building 5(Clone)"))
                num = 1;
            //barracks
            else if (transform.Find("Unique Building 4(Clone)"))
                num = 3;
            //square
            else
                num = 2;
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        SetUpMap();
        if (containsUniqueBuilding)
        {
            int j = CreateUnique(num);
            if (j == -1)
                return;
        }
        Create();
    }

    bool PositiveDirection(int x, int y)
    {
        return false;
    }

    Vector3 FindEdgeDirection(int x, int y)
    {
        int ix = 0, iy = 0;

        //0 is right
        //90 is down
        //-90 is up
        //180 is left
        if (x + 1 < interior.GetLength(0))
        {
            if (interior[x + 1, y] == Space.outside)
                ix += 1;
        }
        else
            ix += 1;

        if (y + 1 < interior.GetLength(1))
        {
            if (interior[x, y + 1] == Space.outside)
                iy += 1;
        }
        else
            iy += 1;

        if (y - 1 >= 0)
        {
            if (interior[x, y - 1] == Space.outside)
                iy -= 1;
        }
        else
            iy -= 1;

        if (x - 1 >= 0)
        {
            if (interior[x - 1, y] == Space.outside)
                ix -= 1;
        }
        else
            ix -= 1;


        if (ix == 1 && iy == 0)
            return new Vector3(0, 0, 0); //works
        if (ix == 0 && iy == 1)
            return new Vector3(0, -90, 0); //works
        if (ix == 0 && iy == -1)
            return new Vector3(0, 90, 0); //works
        if (ix == -1 && iy == 0)
            return new Vector3(0, 180, 0); //works

        if (ix == -1 && iy == -1)
            return new Vector3(0, 90, 0);
        if (ix == 1 && iy == 1)
            return new Vector3(0, -90, 0); //-90
        if (ix == -1 && iy == 1)
            return new Vector3(0, 180, 0); //180 was
        if (ix == 1 && iy == -1)
            return new Vector3(0, 0, 0);

        return new Vector3(180, 0, 0);
    }

    //checks for if there is space to spawn the building.
    bool[] FreeSpace(int x, int y, Vector2 size)
    {
        //initiates the direction of which the loops iterate.
        bool negativeX = false;
        bool negativeY = false;

        bool flip = false;

        //assigns the direction for loops
        if (x + 1 >= interior.GetLength(0) || interior[x + 1, y] == Space.outside)
            negativeX = true;
        if (y + 1 >= interior.GetLength(1) || interior[x, y + 1] == Space.outside)
            negativeY = true;

        if (y + 1 >= interior.GetLength(1))
            flip = true;
        if (y - 1 < 0)
            flip = true;

        if (!flip)
        {
            if (interior[x, y + 1] == Space.outside || interior[x, y - 1] == Space.outside)
            {
                flip = true;
            }
        }

        flip = !flip;

        //over the size of the building
        for (int i = 0; i < (flip ? size.x : size.y); i++)
        {
            for (int j = 0; j < (flip ? size.y : size.x); j++)
            {
                //checks the edges of the array first
                //right edge to left
                if (negativeX)
                {
                    if (x - i >= interior.GetLength(0) || x - i < 0)
                        return new bool[] { false, negativeX, negativeY, flip };
                }
                //left edge to right
                else
                {
                    if (x + i >= interior.GetLength(0) || x + i < 0)
                        return new bool[] { false, negativeX, negativeY, flip };
                }

                //top to bottom
                if (negativeY)
                {
                    if (y - j >= interior.GetLength(1) || y - j < 0)
                        return new bool[] { false, negativeX, negativeY, flip };
                }
                //bottom to top
                else
                {
                    if (y + j >= interior.GetLength(1) || y + j < 0)
                        return new bool[] { false, negativeX, negativeY, flip };
                }

                //checks the current cell in the loop
                if (interior[(negativeX ? x - i : x + i), (negativeY ? y - j : y + j)] == Space.taken || interior[(negativeX ? x - i : x + i), (negativeY ? y - j : y + j)] == Space.outside || interior[(negativeX ? x - i : x + i), (negativeY ? y - j : y + j)] == Space.insideCorner)
                    return new bool[] { false, negativeX, negativeY, flip };

            }
        }

        return new bool[] { true, negativeX, negativeY, flip };
    }

    void SetUpMap()
    {
        if (buildingOutline == null)
            buildingOutline = CreateMap();

        //interior map is 4 times the size of the building outline
        interior = new Space[buildingOutline.GetLength(0) * 4, buildingOutline.GetLength(1) * 4];

        //sets up the interior map
        for (int i = 0; i < buildingOutline.GetLength(0); i++)
        {
            for (int j = 0; j < buildingOutline.GetLength(1); j++)
            {
                //for every coord, 

                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        //if the buildings true or not based upon the cell, as the array has empty space.
                        if (buildingOutline[i, j])
                            interior[i * 4 + x, j * 4 + y] = Space.free;
                        else
                            interior[i * 4 + x, j * 4 + y] = Space.outside;
                    }
                }

            }
        }

        //finds the edge ring of the building
        for (int x = 0; x < interior.GetLength(0); x++)
        {
            for (int y = 0; y < interior.GetLength(1); y++)
            {

                if (interior[x, y] == Space.free)
                {
                    //if the interior is along the edge of the aray, then its a wall
                    if (x - 1 < 0 || x + 1 >= interior.GetLength(0) || y - 1 < 0 || y + 1 >= interior.GetLength(1))
                    {
                        interior[x, y] = Space.edge;
                        continue;
                    }


                    if (interior[x + 1, y] == Space.outside)
                    {
                        interior[x, y] = Space.edge;
                        continue;
                    }
                    if (interior[x - 1, y] == Space.outside)
                    {
                        interior[x, y] = Space.edge;
                        continue;
                    }
                    if (interior[x, y + 1] == Space.outside)
                    {
                        interior[x, y] = Space.edge;
                        continue;
                    }
                    if (interior[x, y - 1] == Space.outside)
                    {
                        interior[x, y] = Space.edge;
                        continue;
                    }

                }
            }
        }

        //finds corners
        for (int x = 0; x < interior.GetLength(0); x++)
        {
            for (int y = 0; y < interior.GetLength(1); y++)
            {

                if (interior[x, y] == Space.edge)
                {
                    int ix = 0, iy = 0;

                    if (x + 1 < interior.GetLength(0) && x + 2 < interior.GetLength(0))
                    {
                        if (interior[x + 1, y] == Space.outside && interior[x + 2, y] == Space.outside)
                            ix++;

                    }
                    else if (x + 2 != interior.GetLength(0))
                        ix++;

                    if (x - 1 >= 0 && x - 2 >= 0)
                    {
                        if (interior[x - 1, y] == Space.outside && interior[x - 2, y] == Space.outside)
                            ix++;
                    }
                    else if (x - 2 != -1)
                        ix++;


                    if (y + 1 < interior.GetLength(1) && y + 2 < interior.GetLength(1))
                    {
                        if (interior[x, y + 1] == Space.outside && interior[x, y + 2] == Space.outside)
                            iy++;

                    }
                    else if (y + 2 != interior.GetLength(1))
                        iy++;

                    if (y - 1 >= 0 && y - 2 >= 0)
                    {
                        if (interior[x, y - 1] == Space.outside && interior[x, y - 2] == Space.outside)
                            iy++;
                    }
                    else if (y - 2 != -1)
                        iy++;

                    if (iy == 1 && ix == 1)
                        interior[x, y] = Space.corner;

                }
            }
        }

        //find internal corners
        for (int x = 0; x < interior.GetLength(0); x++)
        {
            for (int y = 0; y < interior.GetLength(1); y++)
            {


                if (interior[x, y] == Space.free)
                {
                    int ix = 0, iy = 0;

                    if (x + 1 < interior.GetLength(0) && x + 2 < interior.GetLength(0))
                    {
                        if (interior[x + 1, y] == Space.edge && interior[x + 2, y] == Space.edge)
                            ix++;

                    }
                    else if (x + 2 != interior.GetLength(0))
                        ix++;

                    if (x - 1 >= 0 && x - 2 >= 0)
                    {
                        if (interior[x - 1, y] == Space.edge && interior[x - 2, y] == Space.edge)
                            ix++;
                    }
                    else if (x - 2 != -1)
                        ix++;


                    if (y + 1 < interior.GetLength(1) && y + 2 < interior.GetLength(1))
                    {
                        if (interior[x, y + 1] == Space.edge && interior[x, y + 2] == Space.edge)
                            iy++;

                    }
                    else if (y + 2 != interior.GetLength(1))
                        iy++;

                    if (y - 1 >= 0 && y - 2 >= 0)
                    {
                        if (interior[x, y - 1] == Space.edge && interior[x, y - 2] == Space.edge)
                            iy++;
                    }
                    else if (y - 2 != -1)
                        iy++;

                    if (iy == 1 && ix == 1)
                        interior[x, y] = Space.insideCorner;

                }
            }
        }
    }

    //smooths out the the building.
    public List<Coord> Smooth()
    {
        //creates a tempoary coord list
        List<Coord> changedCoords = new List<Coord>();

        bool[,] m = CreateMap();

        for (int i = 0; i < m.GetLength(0); i++)
        {
            for (int j = 0; j < m.GetLength(1); j++)
            {
                //on a true tile, check the amount of true neighbours
                if (!m[i, j])
                {
                    int amount = 0;

                    if (i + 1 < m.GetLength(0))
                        if (m[i + 1, j])
                            amount++;
                    if (j + 1 < m.GetLength(1))
                        if (m[i, j + 1])
                            amount++;

                    if (i - 1 >= 0)
                        if (m[i - 1, j])
                            amount++;
                    if (j - 1 >= 0)
                        if (m[i, j - 1])
                            amount++;

                    //based upon this value, adds a coord to the building list to make it more cubic
                    if (amount >= 3)
                    {
                        m[i, j] = true;
                        tiles.Add(new Coord(i + MinX, j + MinY));
                        changedCoords.Add(new Coord(i + MinX, j + MinY));
                    }
                }
            }
        }

        buildingOutline = m;
        return changedCoords;
    }


    bool[,] CreateMap()
    {
        //find size of map;
        int minX = tiles[0].x, minY = tiles[0].y;
        int MaxX = tiles[0].x, maxY = tiles[0].y;

        //find the max coord value
        foreach (Coord c in tiles)
        {
            if (c.x > MaxX)
                MaxX = c.x;
            if (c.x < minX)
                minX = c.x;
            if (c.y > maxY)
                maxY = c.y;
            if (c.y < minY)
                minY = c.y;
        }

        //sets up the map array
        int sizeX = MaxX - minX;
        int sizeY = maxY - minY;

        bool[,] Map = new bool[sizeX + 1, sizeY + 1];

        //asigns values to the new map
        foreach (Coord c in tiles)
        {
            int x = c.x - minX;
            int y = c.y - minY;

            Map[x, y] = true;
        }

        MinX = minX;
        MinY = minY;

        areaSize = new Vector3(sizeX, 1, sizeY);

        return Map;
    }


}
