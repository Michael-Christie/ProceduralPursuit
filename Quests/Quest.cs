using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    QuestGenerator generator;

    public AbstractData abstactedData;
    //this should probably be changed to an entity later
    public GameObject QuestGiver;

    public List<SubQuest> Task = new List<SubQuest>();
    public int currentTaskIndex = 0;

    public GameObject[] monsters;

    public bool finished = false;

    public Quest(AbstractData data, GameObject[] mons)
    {
        abstactedData = data;
        monsters = mons;
    }

    public void AddElement(SubQuestType.Q_type questType)
    {
        SubQuest Q = new SubQuest();
        Q.Subquest = questType;

        Task.Add(Q);
    }
    public void AddElement(SubQuestType.Q_type questType, SubQuestType.Q_Items item)
    {
        SubQuest Q = new SubQuest();
        Q.Subquest = questType;
        Q.ItemType = item;

        Task.Add(Q);
    }
    public void AddElement(SubQuestType.Q_type questType, SubQuestType.Q_GoToLocations location)
    {
        SubQuest Q = new SubQuest();
        Q.Subquest = questType;
        Q.GotoTypes = location;

        Task.Add(Q);
    }
    public void AddElement(SubQuestType.Q_type questType, SubQuestType.Q_Attack attack)
    {
        SubQuest Q = new SubQuest();
        Q.Subquest = questType;
        Q.AttackType = attack;

        Task.Add(Q);
    }

    public void ConfigureQuest()
    {
        //assings quest giver
        foreach (AbstractData.Entity e in abstactedData.entitys)
        {
            //find the kind basically?
            if (e.entityType == AbstractData.EntityType.NPCNoble)
            {
                QuestGiver = e.NPC;
                break;
            }
        }

        if (QuestGiver == null)
        {
            foreach (AbstractData.Entity e in abstactedData.entitys)
            {
                //find A Gaurd
                if (e.entityType == AbstractData.EntityType.NPCGuard)
                {
                    QuestGiver = e.NPC;
                    break;
                }
            }
        }

        for (int j = 0; j < 10; j++)
        {
            //runs through each quest, assigning it triggers / location
            for (int i = 0; i < Task.Count; i++)
            {
                //goes through in an order
                if ((int)Task[i].Subquest == j)
                {
                    switch (Task[i].Subquest)
                    {
                        //if Attack
                        case SubQuestType.Q_type.Attack:
                            //what needs to be attacked
                            if (Task[i].AttackType == SubQuestType.Q_Attack.Enemies)
                            {
                                Debug.Log("in Attack");
                                //find a ruined area,
                                for (int districIndex = 0; districIndex < abstactedData.districts.Count; districIndex++)
                                {
                                    Debug.Log("looking for district");
                                    if (abstactedData.districts[districIndex].areaFunction == Building.Function.Ruin)
                                    {
                                        Debug.Log("found District");
                                        GameObject newMonsterSpanwer = new GameObject();
                                        newMonsterSpanwer.name = "Monster Spawner";
                                        newMonsterSpanwer.transform.position = abstactedData.districts[districIndex].position;
                                        MonsterSpawner m = newMonsterSpanwer.AddComponent<MonsterSpawner>();

                                        bool[,] lowResMap = GameObject.FindObjectOfType<VilageGenerator>().lowresPathMap;
                                        Vector2 searchTile = new Vector2(newMonsterSpanwer.transform.position.x / 25, newMonsterSpanwer.transform.position.z / 25);

                                        m.AddSpawnLocation(FindNearestPath(lowResMap, searchTile));

                                        m.AddMonsters(monsters);

                                        // m.SpawnMonsters();
                                        //select this distric area
                                        Task[i].Trigger = newMonsterSpanwer;
                                    }
                                }

                                //spawn enemies there, 

                            }
                            else if (Task[i].AttackType == SubQuestType.Q_Attack.Friendlies)
                            {
                                while (true)
                                {
                                    int indexInt = RandomNumber.Range(0, abstactedData.entitys.Count);

                                    //find an NPC to attack
                                    if (abstactedData.entitys[indexInt].entityType != AbstractData.EntityType.NPCNoble && abstactedData.entitys[indexInt].entityType != AbstractData.EntityType.Enemy)
                                    {
                                        Task[i].Trigger = abstactedData.entitys[indexInt].NPC;
                                        break;
                                    }
                                }
                            }
                            break;

                        //if Talk
                        case SubQuestType.Q_type.Talk:
                            //which npc needs to be spoken to
                            if (Task[i - 1].Subquest == SubQuestType.Q_type.Goto)
                            {
                                if (Task[i - 1].GotoTypes == SubQuestType.Q_GoToLocations.QuestGiversLocation)
                                    Task[i].Trigger = QuestGiver;
                                else
                                {
                                    //pick an NPC?
                                    Task[i].Trigger = abstactedData.entitys[RandomNumber.Range(0, abstactedData.entitys.Count)].NPC;
                                }
                            }
                            //sub to event
                            Interactable TalkInter = Task[i].Trigger.AddComponent<Interactable>();
                            TalkInter.Initualize(this, false);

                            break;

                        //if Craft
                        case SubQuestType.Q_type.Craft:
                            //yeah what?
                            break;

                        //if Give
                        case SubQuestType.Q_type.Give:
                            //what item is to be "given" simulair to trade
                            if (Task[i - 1].Subquest == SubQuestType.Q_type.Goto)
                            {
                                if (Task[i - 1].GotoTypes == SubQuestType.Q_GoToLocations.QuestGiversLocation)
                                    Task[i].Trigger = QuestGiver;
                                else
                                {
                                    //pick an NPC?
                                    Task[i].Trigger = abstactedData.entitys[RandomNumber.Range(0, abstactedData.entitys.Count)].NPC;

                                }
                            }
                            //subscribes to event
                            Interactable GiveInter = Task[i].Trigger.AddComponent<Interactable>();
                            GiveInter.Initualize(this, false);

                            break;

                        //if Get
                        case SubQuestType.Q_type.Get:
                            //what item is to be collected ~ completely random atm
                            int rndIndex = 0;

                            do
                            {
                                if (abstactedData.items.Count == 0)
                                {
                                    GameObject newItem = new GameObject();
                                    newItem.layer = LayerMask.NameToLayer("InteractableLayer");
                                    newItem.name = "Temp item as list is empty";
                                    newItem.transform.position = new Vector3(250, 1, 250);

                                    newItem.AddComponent<BoxCollider>();
                                    Interactable inter = newItem.AddComponent<Interactable>();
                                    inter.Initualize(this, true);

                                    Task[i].Trigger = newItem;

                                    break;
                                }

                                rndIndex = RandomNumber.Range(0, abstactedData.items.Count);

                                if (abstactedData.items[rndIndex].ItemType == Task[i].ItemType)
                                {
                                    Task[i].Trigger = abstactedData.items[rndIndex].Item;

                                    Interactable inter = Task[i].Trigger.AddComponent<Interactable>();
                                    inter.Initualize(this, true);
                                }

                            } while (abstactedData.items[rndIndex].ItemType != Task[i].ItemType);

                            break;

                        //if Trade
                        case SubQuestType.Q_type.Trade:
                            //define who to trade with
                            if (i - 1 >= 0)
                            {
                                if (Task[i - 1].Subquest == SubQuestType.Q_type.Goto)
                                {
                                    if (Task[i - 1].GotoTypes == SubQuestType.Q_GoToLocations.QuestGiversLocation)
                                        Task[i].Trigger = QuestGiver;
                                }
                            }
                            else
                            {
                                //pick an npc to trade with?
                                Task[i].Trigger = abstactedData.entitys[RandomNumber.Range(0, abstactedData.entitys.Count)].NPC;
                            }

                            //subscribe the NPC to the Interaction Event;
                            Interactable TradeInter = Task[i].Trigger.AddComponent<Interactable>();
                            TradeInter.Initualize(this, false);

                            break;

                        //if Defend
                        case SubQuestType.Q_type.Defend:
                            //difine who to defend, and how many are left
                            Task[i].Trigger = abstactedData.entitys[RandomNumber.Range(0, abstactedData.entitys.Count)].NPC;
                            break;

                        //if Goto
                        case SubQuestType.Q_type.Goto:
                            //if the quest needs to go back to the quest giver , EZ
                            if (Task[i].GotoTypes == SubQuestType.Q_GoToLocations.QuestGiversLocation)
                            {
                                GameObject newLocation = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                newLocation.name = "TriggerLocation for Task " + i;
                                newLocation.transform.position = QuestGiver.transform.position;
                                MeshRenderer r = newLocation.GetComponent<MeshRenderer>();
                                r.material = generator.LocationMaterial;
                                Location l = newLocation.AddComponent<Location>();
                                l.Initialize(QuestGiver, new Vector3(5, 100, 5), this, generator.LocationMaterial);

                                Task[i].Trigger = newLocation;

                                newLocation.SetActive(false);
                            }
                            else
                            {
                                GameObject newLocation = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                newLocation.layer = LayerMask.NameToLayer("NPC");
                                newLocation.name = "TriggerLocation for Task " + i;
                                //look at the next quest element
                                //based upon that element, return a location to go towards
                                //e.g if a kill quest go to the monsters location
                                //if a trade go to an npcs location
                                MeshRenderer r = newLocation.GetComponent<MeshRenderer>();
                                r.material = generator.LocationMaterial;
                                newLocation.transform.position = Task[i + 1].Trigger.transform.position;

                                Vector3 size = new Vector3(10, 100, 10);

                                if(Task[i + 1].Trigger.name == "Monster Spawner")
                                {
                                    size = new Vector3(50, 100, 50);
                                }

                                Location l = newLocation.AddComponent<Location>();
                                l.Initialize(Task[i + 1].Trigger, size, this, generator.LocationMaterial);

                                Task[i].Trigger = newLocation;

                                newLocation.SetActive(false);

                            }
                            break;

                        //if Use
                        case SubQuestType.Q_type.Use:
                            break;
                    }
                }
            }
        }

        if (Task[currentTaskIndex].Subquest == SubQuestType.Q_type.Goto)
            Task[currentTaskIndex].Trigger.SetActive(true);


    }

    Vector3 FindNearestPath(bool[,] lowResMap, Vector2 searchPos)
    {
        int x = (int)searchPos.x;
        int y = (int)searchPos.y;

        if (lowResMap[x, y])
            return new Vector3(x * 25, 0, y * 25);

        Vector2 closest = new Vector2(1000,1000);
        float closestestDistance = Mathf.Infinity;

        for (int i = 0; i < lowResMap.GetLength(0); i++)
        {
            for (int j = 0; j < lowResMap.GetLength(1); j++)
            {
                if(Mathf.Abs((new Vector2(i,j) - searchPos).magnitude) < closestestDistance)
                {
                    closestestDistance = Mathf.Abs((new Vector2(i, j) - searchPos).magnitude);
                    closest = new Vector2(i, j);
                }
            }
        }

        return new Vector3(closest.x * 25f, 0, closest.y * 25f);
    }

    void UpdateQuests()
    {
        //if the quest actions on Task[currentTaskIndex] is completed,
        if (Task[currentTaskIndex].completed)
        {
            generator.CompletedQuest(currentTaskIndex);
            currentTaskIndex++;
            //move the hint on to the next task to be completed
            //some partical effect im sure.
            generator.PlayPartical(new Color(.19f, .78f, .9f, 1));
            //play sound
            generator.PlayQuestCompletion();    
        }

        if (currentTaskIndex == Task.Count)
        {
            //all quest are finished, yay, do something here. like end the game maybe.. or not, who cares.
            finished = true;
            generator.PlayPartical(new Color(.83f, .21f, .34f, 1));
            //play sound
            generator.PlayQuestCompletion();
        }

        if (Task[currentTaskIndex].Subquest == SubQuestType.Q_type.Goto)
            Task[currentTaskIndex].Trigger.SetActive(true);

        if (Task[currentTaskIndex].Subquest == SubQuestType.Q_type.Get || Task[currentTaskIndex].Subquest == SubQuestType.Q_type.Talk)
            generator.AddIcon(Task[currentTaskIndex].Trigger);
    }


    public void SubscribeToEvent(QuestGenerator g)
    {
        generator = g;
    }

    //Quest delegates
    public void ItemPickUp(GameObject collectedItem)
    {
        if (currentTaskIndex >= Task.Count)
            return;
        if (Task[currentTaskIndex].Subquest == SubQuestType.Q_type.Get)
        {

            if (Task[currentTaskIndex].Trigger == collectedItem)
            {
                Task[currentTaskIndex].completed = true;
                generator.AddObjectToPlayer(Task[currentTaskIndex].Trigger);
            }
            UpdateQuests();

        }
    }

    public void EntityKilled(GameObject entity)
    {
        if (currentTaskIndex >= Task.Count)
            return;
        if (Task[currentTaskIndex].Subquest == SubQuestType.Q_type.Attack)
        {
            if (Task[currentTaskIndex].Trigger == entity)
                Task[currentTaskIndex].completed = true;
            UpdateQuests();
        }
    }

    public void AtLocation(GameObject location)
    {
        if (currentTaskIndex >= Task.Count)
            return;
        if (Task[currentTaskIndex].Subquest == SubQuestType.Q_type.Goto)
        {
            if (Task[currentTaskIndex].Trigger == location)
            {
                Task[currentTaskIndex].completed = true;
                Task[currentTaskIndex].Trigger.SetActive(false);
            }
            UpdateQuests();
        }
    }

    public void InteractedWithNPC(GameObject NPC)
    {
        if (currentTaskIndex >= Task.Count)
            return;
        if (Task[currentTaskIndex].Subquest == SubQuestType.Q_type.Give)
        {
            if (Task[currentTaskIndex].Trigger == NPC)
                Task[currentTaskIndex].completed = true;
            UpdateQuests();
            //thank the player for giving an item to an NPC
        }
        else if (Task[currentTaskIndex].Subquest == SubQuestType.Q_type.Talk)
        {
            if (Task[currentTaskIndex].Trigger == NPC)
                Task[currentTaskIndex].completed = true;
            UpdateQuests();
            //need to do some text here
        }
        else if (Task[currentTaskIndex].Subquest == SubQuestType.Q_type.Trade)
        {
            if (Task[currentTaskIndex].Trigger == NPC)
                Task[currentTaskIndex].completed = true;
            UpdateQuests();
            //need to return a return item to the player
        }
    }


}
