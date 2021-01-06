using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomNumber 
{
    public static void Initialize(int seed)
    {
        Random.InitState(seed);
    }

    public static float Range(float min, float max)
    {
        return Random.Range(min, max);
    }

    public static int Range(int min, int max)
    {
        return Random.Range(min, max);
    }
}
