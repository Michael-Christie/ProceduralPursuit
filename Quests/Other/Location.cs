using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Location : MonoBehaviour
{
    BoxCollider box;
    delegate void collisionHappened(GameObject g);
    collisionHappened coll;

    

    public void Initialize(GameObject target, Vector3 size, Quest quest, Material m)
    {
        transform.localScale = size;
        box = GetComponent<BoxCollider>();
        //box.size = size;

        box.isTrigger = true;
        transform.parent = target.transform;
        //transform.position += (size);
        coll += quest.AtLocation;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Player")
        {
            coll(gameObject);
        }
    }

}
