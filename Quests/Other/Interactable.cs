using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    delegate void ItemIteneration(GameObject g);
    ItemIteneration coll;

    public void Initualize(Quest quest, bool isItem)
    {
        if (isItem)
            coll += quest.ItemPickUp;
        else
            coll += quest.InteractedWithNPC;
    }

    public void HitItem(GameObject whoHit)
    {
        if(whoHit.name == "Player")
        {
            coll(gameObject);
        }
    }
}
