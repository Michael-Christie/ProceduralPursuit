using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMiniMapGizmos : MonoBehaviour
{
    public GameObject playerIcon;
    public GameObject monsterIcon;
    public GameObject talkToIcon;
    public GameObject togoIcon;


    public static DrawMiniMapGizmos instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void Draw()
    {
        DrawPlayer();
        List<SubQuest> sub = FindObjectOfType<QuestGenerator>().g.Task;

        foreach (SubQuest s in sub)
        {
            if (!s.completed)
            {

                if (s.Subquest == SubQuestType.Q_type.Attack)
                {
                    DrawMonsters();
                }
                else if (s.Subquest == SubQuestType.Q_type.Talk)
                {
                    DrawTalkTo(s);
                }
                else if (s.Subquest == SubQuestType.Q_type.Goto)
                {
                    DrawGoTo(s);
                }

                break;
            }
        }

        //I need to give it a draw order depending on what quest it up

    }

    public void UnDraw()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    void DrawPlayer()
    {
        GameObject d = Instantiate(playerIcon, GameObject.Find("Player").transform.GetChild(0).transform.position + new Vector3(0, 100, 0), Quaternion.Euler(90, 0, 0), transform);
        d.GetComponent<SpriteRenderer>().color = Color.green;
    }

    void DrawMonsters()
    {
        Monster[] M = FindObjectsOfType<Monster>();

        if (M != null)
            foreach (Monster mon in M)
            {
                if (!mon.IsDead())
                {
                    GameObject d = Instantiate(monsterIcon, mon.transform.position + new Vector3(0, 100, 0), Quaternion.Euler(90, 0, 0), transform);
                    d.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
    }

    void DrawTalkTo(SubQuest sub)
    {

        if (sub.Subquest == SubQuestType.Q_type.Talk)
        {
            GameObject d = Instantiate(talkToIcon, sub.Trigger.transform.position + new Vector3(0, 100, 0), Quaternion.Euler(90, 0, 0), transform);
            d.GetComponent<SpriteRenderer>().color = Color.cyan;
        }

    }

    void DrawGoTo(SubQuest sub)
    {
        Location location = sub.Trigger.GetComponent<Location>();

        if (location != null)
        {

            GameObject d = Instantiate(togoIcon, location.transform.position + new Vector3(0, 100, 0), Quaternion.Euler(90, 0, 0), transform);
            d.GetComponent<SpriteRenderer>().color = Color.blue;
        }

    }
}
