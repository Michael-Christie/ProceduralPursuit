using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float turnspeed;
    public Transform VMcam;

    public void FixedUpdate()
    {
        VMcam.Rotate(new Vector3(0, turnspeed * Time.deltaTime, 0), Space.World);
    }
}
