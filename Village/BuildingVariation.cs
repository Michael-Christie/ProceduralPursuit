using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingVariation : MonoBehaviour
{
    [System.Serializable]
    public struct Object
    {
        public GameObject location;
        [Range(0,1f)]
        public float PickChance;
    }

    public Object[] Roofs;
    public Object[] Ladders;
    public Object[] Awnings;


    MeshRenderer MRender;
    [Space]
    public Color roofColor = new Color();
    public int roofIndex = 5;
    public bool HasRoof;
    public Color woodColor = new Color();
    public int woodIdex;
    public bool HasWood;

    private void Awake()
    {
        MRender = GetComponent<MeshRenderer>();
    }

    public void Customize()
    {
        if(HasRoof)
            MRender.materials[roofIndex].color = roofColor;
        if(HasWood)
            MRender.materials[woodIdex].color = woodColor;

        //Objects
        if (Roofs.Length > 0)
        {
            for (int i = 0; i < 1; i++)
            {
                int index = RandomNumber.Range(0, Roofs.Length);
                if (Roofs[index].PickChance > RandomNumber.Range(0, 1f))
                {
                    if (Roofs[index].location != null)
                        Roofs[index].location.SetActive(true);
                    i++;
                }
            }
        }

        if (Ladders.Length > 0)
        {
            for (int i = 0; i < 1; i++)
            {
                int index = RandomNumber.Range(0, Ladders.Length);
                if (Ladders[index].PickChance > RandomNumber.Range(0, 1f))
                {
                    if (Ladders[index].location != null)
                        Ladders[index].location.SetActive(true);
                    i++;
                }
            }
        }

        if (Awnings.Length > 0)
        {
            for (int i = 0; i < 1; i++)
            {
                int index = RandomNumber.Range(0, Awnings.Length);
                if (Awnings[index].PickChance > RandomNumber.Range(0, 1f))
                {
                    if (Awnings[index].location != null)
                        Awnings[index].location.SetActive(true);
                    i++;
                }
            }
        }
}

}
