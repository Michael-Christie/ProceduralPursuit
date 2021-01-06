using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboard : MonoBehaviour
{
    Transform player;
    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            player = GameObject.Find("Player").transform;
            return;
        }

        transform.LookAt(player);
    }
}
