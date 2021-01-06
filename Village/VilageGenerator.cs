using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class VilageGenerator : MonoBehaviour
{
    [Header("Village Area")]
    public Vector2 VillageSize = new Vector2(20, 20);
    public float radius = 15f;
    [Header("Village Settings")]
    [Range(0, 1f)]
    public float percentage;
    //public bool RandomSeed = false;
    //public int seed;
    public int numOfIterations = 10;
    [Space]
    //min and max building size
    public int MaxBuildingSize = 25;
    public int MinBuildingSize = 4;
    [Space]
    public VillageBuildingList BuildingList;
    public ColorScheme[] districtColors;

    [Space, Header("Path")]
    public Material pathMaterial;
    public GameObjectScriptArray NPCsModels;
    [Header("Animation")]
    public bool animate;
    //temp
    [Space]
    public GameObject cube;
    MeshFilter pMeshF;
    public Terrain t;
    public CreateNature NG;
    public QuestData data;
    public GameObject sword;



    List<Building> buildings = new List<Building>();
    List<GameObject> NPCs = new List<GameObject>();

    bool[,] buildingMap;

    GameObject village;

    List<Coord> Tiles = new List<Coord>();
    Queue<Coord> shuffledCoords;

    public Coord MapCenter;

    //private void Awake()
    //{
    //    //sets a random Seed ~ this should be done in the game manager later
    //    if (RandomSeed)
    //        seed = Random.Range(-10000, 10000);
    //    Generate();
    //}

    public void Generate()
    {
        //Creates the village gameobject
        village = new GameObject();
        village.name = "Village";
        village.transform.position = Vector3.zero;
        village.isStatic = true;

        //Sets up for the shuffle
        for (int i = 0; i < VillageSize.x; i++)
        {
            for (int j = 0; j < VillageSize.y; j++)
            {
                Tiles.Add(new Coord(i, j));
            }
        }

        //queues shuffle
        shuffledCoords = new Queue<Coord>(Shuffle.ShuffleArray(Tiles.ToArray()));
        //sets up the center of the map to be empty
        MapCenter = new Coord((int)(VillageSize.x * .5f), (int)(VillageSize.y * .5f));
        //sets up the village map
        buildingMap = new bool[(int)VillageSize.x, (int)VillageSize.y];

        //max number of houses allowed
        int houseCount = (int)(VillageSize.x * VillageSize.y * percentage);
        int currentHouseCount = 0;

        //while under the max number of houses
        for (int i = 0; i < houseCount; i++)
        {
            //get the next coord
            Coord rnd = GetRandomCoord();
            buildingMap[rnd.x, rnd.y] = true;
            currentHouseCount++;

            //if its the map center of the building isnt accessible anymore and checks its within the radius
            if (rnd == MapCenter || !IsAccessible(buildingMap, currentHouseCount) || (new Vector2(rnd.x, rnd.y) - new Vector2(MapCenter.x, MapCenter.y)).magnitude > radius)
            {
                buildingMap[rnd.x, rnd.y] = false;
                currentHouseCount--;
            }
        }


        //smooth out the houses into blocks
        for (int i = 0; i < numOfIterations; i++)
        {
            SmoothOutSides();
        }


        //build the connected cells together into blocks
        for (int x = 0; x < VillageSize.x; x++)
        {
            for (int y = 0; y < VillageSize.y; y++)
            {
                //local function ~ if its within the list then return true
                bool inList(Coord c)
                {
                    for (int i = 0; i < buildings.Count; i++)
                    {
                        if (buildings[i].tiles.Contains(c))
                            return true;
                    }

                    return false;
                }


                if (buildingMap[x, y] && !inList(new Coord(x, y)))
                {
                    //create a new object
                    GameObject buildingGroup = new GameObject();
                    buildingGroup.name = "Building Group";
                    buildingGroup.transform.parent = village.transform;
                    buildingGroup.isStatic = true;

                    //add the building component to it
                    Building b = buildingGroup.AddComponent<Building>();

                    //add in the current coord to that list
                    b.AddCoord(new Coord(x, y));
                    //create a temporary search list
                    List<Coord> toSearch = new List<Coord>();

                    //add in the close coords
                    if (x + 1 < VillageSize.x)
                        toSearch.Add(new Coord(x + 1, y));
                    if (x - 1 >= 0)
                        toSearch.Add(new Coord(x - 1, y));
                    if (y + 1 < VillageSize.y)
                        toSearch.Add(new Coord(x, y + 1));
                    if (y - 1 >= 0)
                        toSearch.Add(new Coord(x, y - 1));

                    //while there are still coords to search through
                    while (toSearch.Count > 0)
                    {
                        //take the first element in the array
                        Coord searchingCoord = toSearch[0];

                        //if this is a taken space on the map
                        if (buildingMap[searchingCoord.x, searchingCoord.y])
                        {
                            //add the coord to the buildings list
                            b.AddCoord(searchingCoord);

                            //searches through all the next coords
                            if (searchingCoord.x + 1 < VillageSize.x)
                                if (!b.tiles.Contains(new Coord(searchingCoord.x + 1, searchingCoord.y)) && !toSearch.Contains(new Coord(searchingCoord.x + 1, searchingCoord.y)))
                                    toSearch.Add(new Coord(searchingCoord.x + 1, searchingCoord.y));

                            if (searchingCoord.x - 1 >= 0)
                                if (!b.tiles.Contains(new Coord(searchingCoord.x - 1, searchingCoord.y)) && !toSearch.Contains(new Coord(searchingCoord.x - 1, searchingCoord.y)))
                                    toSearch.Add(new Coord(searchingCoord.x - 1, searchingCoord.y));

                            if (searchingCoord.y + 1 < VillageSize.y)
                                if (!b.tiles.Contains(new Coord(searchingCoord.x, searchingCoord.y + 1)) && !toSearch.Contains(new Coord(searchingCoord.x, searchingCoord.y + 1)))
                                    toSearch.Add(new Coord(searchingCoord.x, searchingCoord.y + 1));

                            if (searchingCoord.y - 1 >= 0)
                                if (!b.tiles.Contains(new Coord(searchingCoord.x, searchingCoord.y - 1)) && !toSearch.Contains(new Coord(searchingCoord.x, searchingCoord.y - 1)))
                                    toSearch.Add(new Coord(searchingCoord.x, searchingCoord.y - 1));
                        }

                        //remove it from the search list
                        toSearch.Remove(searchingCoord);
                    }

                    //building validating
                    if (b.Size <= MinBuildingSize || b.Size > MaxBuildingSize)
                    {
                        foreach (Coord c in b.tiles)
                        {
                            buildingMap[c.x, c.y] = false;
                        }

                        DestroyImmediate(buildingGroup);

                    }
                    else
                    {
                        //if its a valid building, make it more blocky
                        for (int j = 0; j < 20; j++)
                        {
                            List<Coord> changed = b.Smooth();

                            foreach (Coord c in changed)
                                buildingMap[c.x, c.y] = true;
                        }

                        //adds the building component to a list 
                        buildings.Add(b);

                    }

                }
            }
        }

        //create paths around buildings.
        CreatePath();

        //sort buildings
        buildings = BuildingSorter(buildings, true);
        //assign buildings
        AssingFunction();
        //create buildings
        CreateBuildings();

        SpawnNPC();

    }

    public void SpawnItems()
    {
        List<GameObject> posWeaponSpawn = new List<GameObject>();
        //find where all weapons can spawn
            //either on a gaurd
        foreach(NPCMove g in FindObjectOfType<NPCController>().Gaurds)
        {
            posWeaponSpawn.Add(g.gameObject); 
        }

        if (data?.requiredItems == null)
        {
            //in a barraks?
            GameObject barracks = GameObject.Find("Barracks");
            if (barracks)
                posWeaponSpawn.Add(barracks);
            //there should be a third option
        }
        

        //List<GameObject> MedicalShit = new List<GameObject>();
        //spawn all the weapons using these areas

        //find all the areas where medical stuff can spawn
            //this i have no fucking idea!!

        //any other items ?

        foreach(GameObject p in posWeaponSpawn)
        {
            if(RandomNumber.Range(0,1f) > .5f)
            {
                GiveSword(p);
            }
        }   
    }

    List<GameObject> Swords = new List<GameObject>();

    void GiveSword(GameObject obj)
    {
        Transform slot = obj.transform.Find("Slot");

        if (slot != null)
        {
            GameObject i = Instantiate(sword, slot.transform.position, slot.rotation * Quaternion.Euler(0, 0, 0), slot);
            i.name = "Sword";
            Swords.Add(i);
        }
    }

    List<Building> BuildingSorter(List<Building> buildingList, bool useAverage)
    {
        List<Building> orderedBuildings = buildingList;

        foreach (Building b in buildingList)
        {
            float distanceToCenter = Mathf.Infinity;
            float averageDistance = 0;
            if (!useAverage)
            {
                foreach (Coord c in b.tiles)
                {
                    float dist = Mathf.Abs((new Vector2(c.x, c.y) - new Vector2(MapCenter.x, MapCenter.y)).magnitude);
                    if (dist < distanceToCenter)
                        distanceToCenter = dist;
                }

            }
            else
            {
                foreach (Coord c in b.tiles)
                {
                    averageDistance += Mathf.Abs((new Vector2(c.x, c.y) - new Vector2(MapCenter.x, MapCenter.y)).magnitude);
                }

                averageDistance /= b.Size;
            }
            b.distanceToCenter = (useAverage ? averageDistance : distanceToCenter);
        }

        QuickSort(orderedBuildings, 0, buildingList.Count - 1);

        return orderedBuildings;
    }

    public void QuickSort(List<Building> buildingList, int low, int high)
    {
        if (low < high)
        {
            int par = Partition(buildingList, low, high);

            QuickSort(buildingList, low, par - 1);
            QuickSort(buildingList, par + 1, high);
        }
    }

    int Partition(List<Building> buildingList, int low, int high)
    {
        float piv = buildingList[high].distanceToCenter;

        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (buildingList[j].distanceToCenter < piv)
            {
                i++;

                Building temp = buildingList[i];
                buildingList[i] = buildingList[j];
                buildingList[j] = temp;
            }
        }

        Building temp1 = buildingList[i + 1];
        buildingList[i + 1] = buildingList[high];
        buildingList[high] = temp1;

        return i + 1;
    }

    public void AssingFunction()
    {
        //assign building functions ~ need to call upon the quest manager
        //put that here
        if(data != null)
            if (data.requiresMonsters)
            {
                //assign a district as being monsters that is between the starting point and any other High Value Place?
            }

        for (int i = 0; i < buildings.Count; i++)
        {
            //give a more detailed basis for deciding functions
            buildings[i].DecideFunction(MinBuildingSize, i, buildings.Count);
        }

    }

    public void CreateBuildings()
    {
        int i = 0;
        for (int j = 0; j < buildings.Count; j++)
        {
            buildings[j].identifier = districtColors[i % districtColors.Length];
            buildings[j].Build(BuildingList, animate);

            buildings[j].gameObject.name = (j.ToString());
            i++;
        }


        //create unique buildings first
        bool castle = false, barracks = false, square = false;
        int startingIndex = 0;
        //quest generation first
        if (data != null)
        {
            if (!data.requiresCastle)
            {
                startingIndex = 1;
                castle = true;
            }
        }

        for (int index = startingIndex; index < buildings.Count; index++)
        {
            if (buildings[index].containsUniqueBuilding)
                continue;


            int neededBuilding = (!castle ? 1 : (!barracks ? 2 : 3));

            int j = buildings[index].CreateUnique(neededBuilding);

            if (j == 1)
            {
                buildings[index].containsUniqueBuilding = true;
                castle = true;
                index = 0;
            }
            if (j == 2)
            {
                buildings[index].containsUniqueBuilding = true;
                barracks = true;
                index = 0;
            }
            if (j == 3)
            {
               buildings[index].containsUniqueBuilding = true;
                square = true;
                break;
            }

            if (index == buildings.Count - 1)
            {
                if (!castle)
                {
                    StartCoroutine(ReStartCreation());
                    return;
                    castle = true;
                    index = 0;
                    continue;
                }

                if (!barracks)
                {
                    barracks = true;
                    index = 0;
                    continue;
                }

                if (!square)
                {
                    square = true;
                    index = 0;
                    break;
                }
            }
        }

        foreach (Building b in buildings)
        {
            b.Create();

            //b.Visualize(cube);
        }


        //moves the village to have its ceneter at 0,0
        //village.transform.position = new Vector3(-VillageSize.x * 4f, 0, -VillageSize.y * 4f);

        //create the terrain
        StartCoroutine(TerrainGenerator.instance.Generate(buildingMap, lowresPathMap, radius));

        List<Coord> terrainMapReturn = TerrainGenerator.instance.GetMap();

        //create a new object
        GameObject buildingGroup = new GameObject();
        buildingGroup.name = "Envioment Group";
        buildingGroup.transform.parent = village.transform;
        buildingGroup.isStatic = true;

        //add the building component to it
        Building enviomentGroup = buildingGroup.AddComponent<Building>();

        foreach (Coord c in terrainMapReturn)
        {
            enviomentGroup.AddCoord(c);
            buildingMap[c.x, c.y] = true;
        }

        enviomentGroup.functionType = Building.Function.Envioment;
        buildings.Add(enviomentGroup);


        enviomentGroup.identifier = districtColors[0];
        enviomentGroup.Build(BuildingList, animate);
        enviomentGroup.Create();
        //enviomentGroup.Visualize(cube);

        CreateNature();
    }


    public IEnumerator ReStartCreation()
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(1);
    }

    public void RunAnimations()
    {
        foreach (Building b in buildings)
        {
            if(b != null)
                b.ShowAnimation();
        }
        pathAnimate = true;
    }

    public void CreateNature()
    {


        //generate nature
        NG.GenerateNature(buildingMap, lowresPathMap);
        //NG.Visualize(cube);
    }

    bool[,] pathMap;
    public bool[,] lowresPathMap;

    MeshCollider pMeshC;

    void CreatePath()
    {
        lowresPathMap = new bool[(int)VillageSize.x, (int)VillageSize.y];
        //creates a larger map of the village
        pathMap = new bool[(int)VillageSize.x * 3, (int)VillageSize.y * 3];

        for (int x = 0; x < VillageSize.x; x++)
        {
            for (int y = 0; y < VillageSize.y; y++)
            {
                //if the tile is clear
                if (!buildingMap[x, y])
                {
                    //check the 8 sides of the tile, to see if its next to a wall
                    if (x - 1 >= 0)
                        if (buildingMap[x - 1, y])
                        {
                            lowresPathMap[x, y] = true;

                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    pathMap[x * 3 + i, y * 3 + j] = true;
                                }
                            }
                            continue;
                        }
                    if (y - 1 >= 0)
                        if (buildingMap[x, y - 1])
                        {
                            lowresPathMap[x, y] = true;

                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    pathMap[x * 3 + i, y * 3 + j] = true;
                                }
                            }
                            continue;
                        }
                    if (x + 1 < VillageSize.x)
                        if (buildingMap[x + 1, y])
                        {
                            lowresPathMap[x, y] = true;

                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    pathMap[x * 3 + i, y * 3 + j] = true;
                                }
                            }
                            continue;
                        }
                    if (y + 1 < VillageSize.y)
                        if (buildingMap[x, y + 1])
                        {
                            lowresPathMap[x, y] = true;

                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    pathMap[x * 3 + i, y * 3 + j] = true;
                                }
                            }
                            continue;
                        }

                    //diagonals

                    if (x - 1 >= 0 && y - 1 >= 0)
                        if (buildingMap[x - 1, y - 1])
                        {
                            lowresPathMap[x, y] = true;

                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    pathMap[x * 3 + i, y * 3 + j] = true;
                                }
                            }
                            continue;
                        }

                    if (x + 1 < buildingMap.GetLength(0) && y - 1 >= 0)
                        if (buildingMap[x + 1, y - 1])
                        {
                            lowresPathMap[x, y] = true;

                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    pathMap[x * 3 + i, y * 3 + j] = true;
                                }
                            }
                            continue;
                        }

                    if (x - 1 >= 0 && y + 1 < buildingMap.GetLength(1))
                        if (buildingMap[x - 1, y + 1])
                        {
                            lowresPathMap[x, y] = true;

                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    pathMap[x * 3 + i, y * 3 + j] = true;
                                }
                            }
                            continue;
                        }

                    if (x + 1 < buildingMap.GetLength(0) && y + 1 < buildingMap.GetLength(1))
                        if (buildingMap[x + 1, y + 1])
                        {
                            lowresPathMap[x, y] = true;

                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    pathMap[x * 3 + i, y * 3 + j] = true;
                                }
                            }
                            continue;
                        }

                }
            }
        }

        //creates a new map to smooth over
        bool[,] newPathMap = new bool[pathMap.GetLength(0), pathMap.GetLength(1)];

        for (int x = 0; x < pathMap.GetLength(0); x++)
        {
            for (int y = 0; y < pathMap.GetLength(1); y++)
            {
                //if its a path, count all the neigbours
                if (pathMap[x, y])
                {
                    int count = 0;
                    //look at the 4 neigbours, if all are true, include it in the new map
                    if (x - 1 >= 0)
                        if (pathMap[x - 1, y])
                            count++;
                    if (y - 1 >= 0)
                        if (pathMap[x, y - 1])
                            count++;

                    if (x + 1 < pathMap.GetLength(0))
                        if (pathMap[x + 1, y])
                            count++;
                    if (y + 1 < pathMap.GetLength(1))
                        if (pathMap[x, y + 1])
                            count++;

                    if (count == 4)
                    {
                        newPathMap[x, y] = true;
                    }
                }
            }
        }

        pathMap = newPathMap;

        //creates the path gameobject
        GameObject Path = new GameObject();
        Path.name = "Path";
        Path.layer = LayerMask.NameToLayer("Path");
        Path.transform.parent = village.transform;

        MeshRenderer pMeshR = Path.AddComponent<MeshRenderer>();
        pMeshR.material = pathMaterial;
        pMeshF = Path.AddComponent<MeshFilter>();

        pMeshF.mesh = MarchingSquares.Generate(pathMap, MapCenter, 0);
        Path.transform.position -= new Vector3(-.5f, -.1f, -.5f);

        pMeshC = Path.AddComponent<MeshCollider>();
        pMeshC.sharedMesh = pMeshF.mesh;

        Path.AddComponent<NavMeshSourceTag>();

        pathRadius = 0;

      

    }

    void SpawnNPC()
    {
        AbstractData d = new AbstractData();
        Districts(d);

        GameObject NPCmanager = new GameObject();

        bool containsCastle = false;

        foreach (AbstractData.Area a in d.districts)
        {
            if (a.areaFunction == Building.Function.Castle)
            {
                containsCastle = true;
                break;
            }
        }

        Debug.Log("Castle is equal to" + containsCastle.ToString());

        //if (containsCastle)
        //{
        //    Debug.Log("Spawning King and Queen");

        //    if (RandomNumber.Range(0, 1f) > .5f)
        //    {
        //        //king
        //        GameObject g = Instantiate(NPCsModels.Characters[0]);
        //        g.name = "NPC King Noble";
        //        //g.transform.position = PathLocation();
        //        g.transform.parent = NPCmanager.transform;

        //        FindObjectOfType<NPCController>().Nobels.Add(g.AddComponent<NPCMove>());

        //        NPCs.Add(g);

        //        g.SetActive(false);
        //    }
        //    else if (RandomNumber.Range(0, 1f) > .5f)
        //    {
        //        //Queen
        //        GameObject g = Instantiate(NPCsModels.Characters[1]);
        //        g.name = "NPC Queen Noble";
        //        g.transform.position = PathLocation();
        //        g.transform.parent = NPCmanager.transform;

        //        FindObjectOfType<NPCController>().Nobels.Add(g.AddComponent<NPCMove>());

        //        NPCs.Add(g);

        //        g.SetActive(false);
        //    }
        //    else
        //    {
        //        //Queen
        //        GameObject g = Instantiate(NPCsModels.Characters[1]);
        //        g.name = "NPC Queen Noble";
        //        //g.transform.position = PathLocation();
        //        g.transform.parent = NPCmanager.transform;

        //        FindObjectOfType<NPCController>().Nobels.Add(g.AddComponent<NPCMove>());

        //        NPCs.Add(g);

        //        g.SetActive(false);

        //        //king
        //        g = Instantiate(NPCsModels.Characters[0]);
        //        g.name = "NPC King Noble";
        //        //g.transform.position = PathLocation();
        //        g.transform.parent = NPCmanager.transform;

        //        FindObjectOfType<NPCController>().Nobels.Add(g.AddComponent<NPCMove>());

        //        NPCs.Add(g);

        //        g.SetActive(false);
        //    }
        //}

        //spawn Gaurds
        for (int i = 0; i < 250; i++)
        {
            GameObject g = Instantiate(NPCsModels.Characters[RandomNumber.Range(2, 7)]);
            g.name = "NPC Gaurd " + i;
            //g.transform.position = PathLocation();
            g.transform.parent = NPCmanager.transform;

            FindObjectOfType<NPCController>().Gaurds.Add(g.AddComponent<NPCMove>());

            NPCs.Add(g);

            //g.SetActive(false);
        }

        //spawn civs
        for (int i = 0; i < 150; i++)
        {
            GameObject g = Instantiate(NPCsModels.Characters[RandomNumber.Range(7, 34)]);
            g.name = "NPC Civ " + i;
            //g.transform.position = PathLocation();
            g.transform.parent = NPCmanager.transform;

            FindObjectOfType<NPCController>().Pessents.Add(g.AddComponent<NPCMove>());

            NPCs.Add(g);


            //g.SetActive(false);
        }

        SpawnItems();

        //foreach (GameObject g in NPCs)
        //    g.SetActive(false);
    }

    float pathRadius = 1;
    bool pathAnimate = false;

    public void StartNPCs()
    {
        FindObjectOfType<NPCController>().SetUp(lowresPathMap);

    }

    void FixedUpdate()
    {
        if (pathMap != null && pathAnimate)
        {
            if (pathRadius < pathMap.GetLength(0) / 2 - 1)
            {
                pMeshF.mesh = MarchingSquares.Generate(pathMap, MapCenter, pathRadius);
                pMeshC.sharedMesh = pMeshF.mesh;
                pathRadius += 15 * Time.deltaTime;
            }

        }
    }

    //smooths out the side of the block
    void SmoothOutSides()
    {
        for (int i = 0; i < VillageSize.x; i++)
        {
            for (int j = 0; j < VillageSize.y; j++)
            {
                if (buildingMap[i, j])
                {
                    //left 2 right, up 2 down
                    int l2r = 0, u2d = 0;

                    if (i + 1 < VillageSize.x)
                    {
                        if (!buildingMap[i + 1, j])
                            l2r++;
                    }
                    else
                        l2r++;

                    if (i - 1 >= 0)
                    {
                        if (!buildingMap[i - 1, j])
                            l2r++;
                    }
                    else
                        l2r++;

                    if (l2r == 2)
                        l2r++;

                    if (j + 1 < VillageSize.y)
                    {
                        if (!buildingMap[i, j + 1])
                            u2d++;
                    }
                    else
                        u2d++;

                    if (j - 1 >= 0)
                    {
                        if (!buildingMap[i, j - 1])
                            u2d++;
                    }
                    else
                        u2d++;

                    if (u2d == 2)
                        u2d++;

                    if ((l2r + u2d >= 3))
                        buildingMap[i, j] = false;
                }
            }
        }
    }


    bool IsAccessible(bool[,] buildingMap, int curBuildCount)
    {
        bool[,] mapFlags = new bool[buildingMap.GetLength(0), buildingMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(MapCenter);
        mapFlags[MapCenter.x, MapCenter.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neigbX = tile.x + x;
                    int neigbY = tile.y + y;

                    if (x == 0 || y == 0)
                    {
                        if (neigbX >= 0 && neigbX < buildingMap.GetLength(0) && neigbY >= 0 && neigbY < buildingMap.GetLength(1))
                        {
                            if (!mapFlags[neigbX, neigbY] && !buildingMap[neigbX, neigbY])
                            {
                                mapFlags[neigbX, neigbY] = true;
                                queue.Enqueue(new Coord(neigbX, neigbY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccsess = (int)(VillageSize.x * VillageSize.y - curBuildCount);

        return targetAccsess == accessibleTileCount;

    }

    //returns the next Coord in the queue
    public Coord GetRandomCoord()
    {
        Coord rnd = shuffledCoords.Dequeue();
        shuffledCoords.Enqueue(rnd);
        return rnd;
    }

    void Districts(AbstractData data)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            AbstractData.Area area = new AbstractData.Area();

            area.areaFunction = buildings[i].functionType;
            area.position = buildings[i].Location;
            area.size = buildings[i].areaSize;
            area.area = buildings[i].gameObject;

            data.districts.Add(area);
        }
    }

    public AbstractData CreateData()
    {
        AbstractData data = new AbstractData();

        //adds all the areas to the data
        Districts(data);

        //
        for (int i = 0; i < NPCs.Count; i++)
        {
            AbstractData.Entity entity = new AbstractData.Entity();

            entity.NPC = NPCs[i];
            if (entity.NPC.name.Contains("Noble"))
            {
                Debug.Log(entity.NPC.name);
                entity.entityType = AbstractData.EntityType.NPCNoble;
            }
            else if (entity.NPC.name.Contains("Gaurd"))
            {
                Debug.Log(entity.NPC.name);
                entity.entityType = AbstractData.EntityType.NPCGuard;
            }
            else
                entity.entityType = AbstractData.EntityType.NPC;

            data.entitys.Add(entity);
        }

        foreach(GameObject g in Swords)
        {
            AbstractData.AllItems e = new AbstractData.AllItems();
            e.Item = g;
            e.ItemType = SubQuestType.Q_Items.Usable;
            e.location = g.transform.position;

            data.items.Add(e);
        }

        return data;
    }

}