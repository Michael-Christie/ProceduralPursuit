using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData
{

    public bool requiresCastle = false;
    public bool requiresMonsters = false;

    public class Items
    {
        //item type
        public SubQuestType.Q_Items itemType;
        //item location?
        //
    }

    public List<Items> requiredItems = new List<Items>();

    
}
