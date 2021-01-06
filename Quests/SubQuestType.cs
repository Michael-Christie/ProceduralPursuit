using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubQuestType
{
    //numbers are priority order
    public enum Q_type
    {
        none = 0,
        Attack = 1,
        Talk = 6,
        Craft = 8,
        Give = 4,
        Get = 3,
        Trade = 5,
        Defend = 2,
        Goto = 9,
        Use = 7
    }

    public enum Q_Items
    {
        none,
        Study,
        Luxurie,
        Rare,
        Usable,
        Medical
    }

    public enum Q_Attack
    {
        none,
        Enemies,
        Friendlies
    }

    public enum Q_GoToLocations
    {
        none,
        QuestGiversLocation,
        AnyLocation,
        QuestGiverOrAnyWhere
    }

}
