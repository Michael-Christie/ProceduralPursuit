using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    bool canRun = false;
    bool[,] pathmap;

    [Header("Nobels")]
    public List<NPCMove> Nobels = new List<NPCMove>();

    [Header("Gaurds")]
    public List<NPCMove> Gaurds = new List<NPCMove>();
    public int minGaurdSize = 2;
    public int maxGaurdSize = 8;
    public List<GaurdGroup> Groups = new List<GaurdGroup>();

    [Header("Pessents")]
    public List<NPCMove> Pessents = new List<NPCMove>();
    public int minPessentSize = 2;
    public int maxPessentSize = 4;
    public List<PessentRelation> publicRelations = new List<PessentRelation>();

    [Header("DataMap")]
    public int width = 5;
    public int height = 5;
    NPCPopulation[,] NPCSpread;


    //local classes
    [System.Serializable]
    public class GaurdGroup
    {
        public GaurdState GS;

        public List<NPCMove> Gaurds = new List<NPCMove>();
        //public bool IsCompleted => AllAtDestination();

        public bool AllAtDestination()
        {
            bool completed = true;

            foreach (NPCMove g in Gaurds)
                if (!g.RequiresNewAction)
                    completed = false;

            return completed;
        }
    }

    public class PessentRelation
    {
        public List<NPCMove> Relationship = new List<NPCMove>();

        public bool AllAtDestination()
        {
            int i = 0;

            foreach (NPCMove g in Relationship)
                if (!g.RequiresNewAction)
                    i++;

            return (i < 2); 
        }
    }

    [System.Serializable]
    public struct NPCPopulation
    {
        public int Nobles;
        public int Gaurds;
        public int Pessents;
    }

    public enum GaurdState
    {
        idel = 0,
        walking = 1,
        talking = 2,
        wondering = 3
    }


    //NPC
    void SetUpGaurds()
    {
        //sets up the gaurd groups
        int currentIndex = 0;
        //while theres still unassigned gaurds
        while (currentIndex < Gaurds.Count)
        {

            //get the next size
            int nextGaurdSize = RandomNumber.Range(minGaurdSize, maxGaurdSize);

            //set up the group
            GaurdGroup newGroup = new GaurdGroup();

            Vector3 startingPos = GetPathPos();
            Vector3 destination = GetPathPos();

            for (int i = 0; i < nextGaurdSize; i++)
            {

                //if the index exists
                if (currentIndex + i < Gaurds.Count)
                {
                    //set their location
                    Gaurds[currentIndex + i].transform.position = startingPos + Random.insideUnitSphere * 2f;
                    Gaurds[currentIndex + i].gameObject.SetActive(true);

                    Gaurds[currentIndex + i].SetUpAgent(currentIndex + i);
                    //and first destination
                    Gaurds[currentIndex + i].SetDestination(destination);

                    newGroup.Gaurds.Add(Gaurds[currentIndex + i]);
                }
            }

            //add the size to the indexCount
            currentIndex += nextGaurdSize;
            newGroup.GS = GaurdState.walking;
            Groups.Add(newGroup);

        }
    }

    void SetUpPessents()
    {
        //sets up the pedestrian relation
        int currentIndex = 0;
        //while theres still unassigned pessents
        while (currentIndex < Pessents.Count)
        {

            //get the next size
            int nextPessentSize = RandomNumber.Range(minPessentSize, maxPessentSize);

            //set up the group
            PessentRelation PRelationship = new PessentRelation();

            Vector3 startingPos = GetPathPos();
            Vector3 destination = GetPathPos();

            for (int i = 0; i < nextPessentSize; i++)
            {

                //if the index exists
                if (currentIndex + i < Pessents.Count)
                {
                    //set their location
                    Pessents[currentIndex + i].transform.position = startingPos + Random.insideUnitSphere * 2f;
                    Pessents[currentIndex + i].gameObject.SetActive(true);

                    Pessents[currentIndex + i].SetUpAgent(currentIndex + i);
                    //and first destination
                    Pessents[currentIndex + i].SetDestination(destination);

                    PRelationship.Relationship.Add(Pessents[currentIndex + i]);
                }
            }

            //add the size to the indexCount
            currentIndex += nextPessentSize;
            publicRelations.Add(PRelationship);
        }
    }

    public void SetUp(bool[,] pm)
    {
        pathmap = pm;
        AllValidLocation();

        SetUpGaurds();
        SetUpPessents();
  
        canRun = true;
    }

    List<Vector3> togoLocations = new List<Vector3>();

    void AllValidLocation()
    {
        for (int i = 0; i < pathmap.GetLength(0); i++)
            for (int j = 0; j < pathmap.GetLength(1); j++)
                if (pathmap[i, j])
                    togoLocations.Add(new Vector3(i * 10f + 4, 0, j * 10f + 4));
    }

    Vector3 GetPathPos()
    {
        Vector3 location = Vector3.zero;

        while (location == Vector3.zero)
        {
            int index = RandomNumber.Range(0, togoLocations.Count);

            location = togoLocations[index];
        }

        return location;
    }

    private void Update()
    {
        if (canRun)
        {
            //check for crowd density?
            //NPCPopulation[,] peopleData = CalculateData();
            ////check through all current controllers
            //for (int i = 0; i < publicRelations.Count; i++)
            //{
            //    if (publicRelations[i].AllAtDestination())
            //    {
            //        //temp
            //        Vector3 destination = GetPathPos();

            //        for (int j = 0; j < publicRelations[i].Relationship.Count; j++)
            //        {
            //            publicRelations[i].Relationship[j].SetDestination(destination);
            //        }
            //    }
            //}

            //set paths for npcs that need new paths and with functions
            for (int i = 0; i < Groups.Count; i++)
            {

                if (Groups[i].AllAtDestination())
                {
                    Groups[i].GS = GaurdState.idel;

                    //tempoary...
                    Vector3 destination = GetPathPos();

                    foreach (NPCMove n in Groups[i].Gaurds)
                        n.SetDestination(destination);
                    Groups[i].GS = GaurdState.walking;
                }


                //decide next action

                //either idel for a bit then walk

                //or talk then walk?


            }
        }

        //set paths for npcs that need new paths and with functions
        for (int i = 0; i < publicRelations.Count; i++)
        {

            if (publicRelations[i].AllAtDestination())
            {

                //tempoary...
                Vector3 destination = GetPathPos();

                foreach (NPCMove n in publicRelations[i].Relationship)
                    n.SetDestination(destination);
            }


            //decide next action

            //either idel for a bit then walk

            //or talk then walk?

        }


    }

    NPCPopulation[,] CalculateData()
    {
        if (!Application.isPlaying)
            return null;

        NPCPopulation[,] newData = new NPCPopulation[width, height];

        float cellWidth = 500f / width;
        float cellHeight = 500f / height;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                newData[i, j].Nobles = 0;
                newData[i, j].Gaurds = 0;
                newData[i, j].Pessents = 0;
            }
        }

        if (Nobels.Count > 0)
            foreach (NPCMove m in Nobels)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (m.transform.localPosition.x < i * cellWidth && m.transform.localPosition.z < j * cellHeight)
                            newData[i, j].Nobles++;
                    }
                }
            }

        if (Gaurds.Count > 0)
            foreach (NPCMove m in Gaurds)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (m.transform.position.x <= i * cellWidth && m.transform.position.z <= j * cellHeight)
                            newData[i, j].Gaurds++;
                    }
                }
            }

        if (Pessents.Count > 0)
            foreach (NPCMove m in Pessents)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (m.transform.position.x <= i * cellWidth && m.transform.position.z <= j * cellHeight)
                            newData[i, j].Pessents += 1;
                    }
                }
            }


        return newData;
    }

    private void OnDrawGizmos()
    {
        NPCPopulation[,] data = CalculateData();

        float cellWidth = 500f / width;
        float cellHeight = 500f / height;

        if (data == null)
            return;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(data[i,j].Gaurds > 10)
                {
                    Gizmos.DrawCube(new Vector3(i * cellWidth, 25f, j * cellHeight), new Vector3(50,50,50));
                }
            }
        }
    }
}
