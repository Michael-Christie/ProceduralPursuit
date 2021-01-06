using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SubQuest
{
    public string ID;

    public SubQuestType.Q_type Subquest;
    public SubQuestType.Q_Items ItemType;
    public SubQuestType.Q_Attack AttackType;
    public SubQuestType.Q_GoToLocations GotoTypes;

    public GameObject Trigger;
    public bool completed;

}
