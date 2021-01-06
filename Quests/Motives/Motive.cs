using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Motive : ScriptableObject
{
    public Step[] step;

    [System.Serializable]
    public class Step
    {
        public SubQuestType.Q_type QuestList;
        public Options options;
        public bool alwaysIncluded;
        [Range(0, 1f)]
        public float includePercentage = 1f;
    }

    [System.Serializable]
    public class Options
    {
        public SubQuestType.Q_Attack attackOption;
        public SubQuestType.Q_Items itemOption;
        public SubQuestType.Q_GoToLocations GotoOption;
    }
}
