using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    private void Awake()
    {
        if (instance != this && instance != null)
            DestroyImmediate(this);
        else
        {
            instance = this;

            DontDestroyOnLoad(this);
        }
    }

    public int idNumber;
    public int roundNumber = 0;
    public int doneQuest = -1;
    public bool canSwitch = false;

    public int RequestOrder()
    {
        return ((idNumber + roundNumber) % 2);
    }

    public void SetPlayerId(int ID)
    {
        idNumber = ID;
    }
}
