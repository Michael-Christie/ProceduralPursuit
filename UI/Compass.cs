using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform player;
    GameObject go;
    // Update is called once per frame
    void Update()
    {
        if (go)
        {
            player = go.transform;
            Vector3 rot = transform.rotation.eulerAngles;
            rot.y = player.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(rot);
        }
        else
            go = GameObject.Find("Player");
    }
}
