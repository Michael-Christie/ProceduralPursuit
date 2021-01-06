using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class VillageBuildingList : ScriptableObject
{
    public groups[] buildingList;

    public GameObject wall;
    public GameObject cornerWall;

    [System.Serializable]
    public struct groups
    {
        public string ID;
        public Building[] buildings;
    }

    [System.Serializable]
    public class Building
    {
        public GameObject prefab;
        [Tooltip("The amount of space this tile holds")]
        public Vector2 objectSize = new Vector2(1, 1);
        public Vector3 rotationSide = new Vector3(1, 1, 1);

        [Space]
        public bool SpawnInCorner = false;
        public bool SpawnsOnEdge = true;
        [Header("Offset")]
        public Vector3 negX;
        public Vector3 posX;
        public Vector3 negY;
        public Vector3 posY;

    }
}
