using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VarianceObject : MonoBehaviour
{

    public static VarianceObject VO;

    [System.Serializable]
    public class Variance
    {
        public GameObject Object;
        [Range(0, 1f)]
        public float SpawnChance;

        [Header("RotationsAllowed")]
        public bool anyRotation;
        public bool front;
        public bool right;
        public bool back;
        public bool left;

    }

    public Variance[] Rooftops;

    void Awake()
    {
        Singleton();
    }

    void Singleton()
    {
        if (VO != null && VO != this)
            Destroy(this);
        else
            VO = this;
    }

    public Variance RequestRoof()
    {
        return Rooftops[RandomNumber.Range(0, Rooftops.Length)];
    }

}
