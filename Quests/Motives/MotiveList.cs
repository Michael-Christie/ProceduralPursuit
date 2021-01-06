using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MotiveList : ScriptableObject
{
    public MotiveL[] Motive;

    [System.Serializable]
    public class MotiveL
    {
        public string ID;
        public List<Motive> motives = new List<Motive>();
    }
}
