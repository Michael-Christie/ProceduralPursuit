using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractData
{
    //list for all buildings / types ~ this is basically the buildings list
    public List<Area> districts = new List<Area>();
    //List of NPC's / Entities? in the world ~ grouped by some function probably
    public List<Entity> entitys = new List<Entity>();
    //List of items ~ grouped by type
    public List<AllItems> items = new List<AllItems>();

    public struct Area
    {
        public Vector3 position;
        public Vector3 size;
        public Building.Function areaFunction;
        public GameObject area;
    }

    public struct Entity
    {
        //this is going to need details
        public GameObject NPC;
        public EntityType entityType;
    }

    public enum EntityType
    {
        NPC,
        NPCGuard,
        NPCNoble,
        Enemy
    }

    public struct AllItems
    {
        public SubQuestType.Q_Items ItemType;
        public GameObject Item;
        public Vector3 location;
    }
}
