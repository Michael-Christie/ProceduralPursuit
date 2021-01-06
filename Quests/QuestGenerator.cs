using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGenerator : MonoBehaviour
{
    public MotiveList listMotives;
    Motive start;
    public Quest g;

    public GameObject qGiver;
    public AbstractData data;

    public GameObject[] monsters;

    public QuestUIManager qM;

    public Material LocationMaterial;

    [Header("")]
    public GameObject SubquestComplete;

    public AudioClip questCompleteSound;

    public GameObject interactIcon;

    public void Generate()
    {
        int index = -1;
        if (LevelLoader.instance.doneQuest == -1)
            index = RandomNumber.Range(0, 2);
        else
        {
            if (LevelLoader.instance.canSwitch)
            {
                if (LevelLoader.instance.doneQuest == 0)
                    index = 1;
                else
                    index = 0;

                LevelLoader.instance.canSwitch = false;
            }
            else
                index = LevelLoader.instance.doneQuest;
        }

        LevelLoader.instance.doneQuest = index;

        start = listMotives.Motive[index].motives[0];

        CreateQuestTree();
    }

    void CreateQuestTree()
    {
        Quest quest = new Quest(data, monsters);
        quest.SubscribeToEvent(this);

        for(int i = 0; i < start.step.Length; i++)
        {
            if (start.step[i].options.attackOption != SubQuestType.Q_Attack.none)
                quest.AddElement(start.step[i].QuestList, start.step[i].options.attackOption);
            else if (start.step[i].options.itemOption != SubQuestType.Q_Items.none)
                quest.AddElement(start.step[i].QuestList, start.step[i].options.itemOption);
            else if (start.step[i].options.GotoOption != SubQuestType.Q_GoToLocations.none)
                quest.AddElement(start.step[i].QuestList, start.step[i].options.GotoOption);
            else
                quest.AddElement(start.step[i].QuestList);
        }

        g = quest;
    }

    public void ConfigureQuest()
    {
        g.ConfigureQuest();
    }

    public QuestData CreateData()
    {
        //creates the quest data that is used to generate the map spacing!!!
        QuestData qData = new QuestData();

        for (int i = 0; i < g.Task.Count; i++)
        {
            qData.requiresCastle = true; //this needs a value to be calculated;

            //if there is a monster fighting task
            if (g.Task[i].AttackType == SubQuestType.Q_Attack.Enemies)
                qData.requiresMonsters = true;

            if (g.Task[i].ItemType != SubQuestType.Q_Items.none)
            {
                // set up what items need to generate here
                QuestData.Items item = new QuestData.Items();
                item.itemType = g.Task[i].ItemType;
                //other item perameters?
            }

        }

        return qData;
    }

    public void AddObjectToPlayer(GameObject g)
    {
        Transform t = GameObject.Find("HandSocket").transform;
        Debug.Log(g);
        Debug.Log(t);
        GameObject s = Instantiate(g, t.position, t.rotation, t);
        DestroyImmediate(s.transform.GetChild(0).gameObject);
        DestroyImmediate(g);
    }

    public void DisplayQuest()
    {
        for (int i = 0; i < g.Task.Count; i++)
            qM.AddStep(g.Task[i]);
    }

    public void CompletedQuest(int i)
    {
        qM.MarkTaskAsComplete(i);
    }

    public void PlayPartical(Color partColor)
    {
        Transform p = GameObject.Find("Player").transform;

        for(int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                GameObject partical = Instantiate(SubquestComplete, p.position + (p.forward * 5 * x) + (p.right * 5 * y), Quaternion.identity, transform);
                ParticleSystem part = partical.GetComponent<ParticleSystem>();
                part.Stop();
                var m = part.main;
                m.startColor = partColor;
                part.Play();
            }
        }

    }

    public void PlayQuestCompletion()
    {
        AudioSource a = GetComponent<AudioSource>();
        a.clip = questCompleteSound;
        a.PlayOneShot(a.clip);
    }

    public void AddIcon(GameObject task)
    {
        Instantiate(interactIcon, task.transform.position + (Vector3.up * 2), Quaternion.identity, task.transform);
    }

}
