using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnColor : MonoBehaviour
{
    public BuildingVariation BV;

    public bool hasWood = false;
    public int woodIdex = 0;
    public bool hadRoof = false;
    public int RoofIndex = 0; 

    private void OnEnable()
    {

        if (hasWood)
            transform.GetComponent<MeshRenderer>().materials[woodIdex].color = BV.woodColor;    

        if (hadRoof)
            transform.GetComponent<MeshRenderer>().materials[RoofIndex].color = BV.roofColor;
    }
}
