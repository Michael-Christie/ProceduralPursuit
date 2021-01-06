using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    public GameObject StepGameObject;
    public List<GameObject> UISteps = new List<GameObject>();

    public void AddStep(SubQuest s)
    {
        GameObject step = Instantiate(StepGameObject, transform.GetChild(1));

        Text task() => step.transform.Find("Task").GetComponent<Text>();
        //set up the content inside the step
        switch (s.Subquest)
        {
            case SubQuestType.Q_type.Attack:
                if(s.AttackType == SubQuestType.Q_Attack.Enemies)
                    task().text = "Defeat the Monsters and save the citivans!";
                else
                    task().text = s.Trigger.name + " must have done something wrong. I need to kill them!";
                step.name = "Attack";
                break;

            case SubQuestType.Q_type.Goto:
                task().text = "Head over to this area!";
                step.name = "Go To Location";
                break;

            case SubQuestType.Q_type.Talk:
                task().text = "I need to go talk to " + s.Trigger.name;
                step.name = "Talk to";
                break;

            case SubQuestType.Q_type.Get:
                task().text = "I need to steal a " + s.Trigger.name + " off of a gaurd";
                step.name = "Get Item";
                break;

            case SubQuestType.Q_type.Trade:
                task().text = "Talking to " + s.Trigger.name + " could provide me the item I need.";
                step.name = "Talk to";
                break;

            case SubQuestType.Q_type.Give:
                task().text = "I need to give {item} to " + s.Trigger.name;
                step.name = "Give Item";
                break;

            case SubQuestType.Q_type.Defend:
                task().text = s.Trigger.name + " is being attacked, they need to be defended!";
                step.name = "Defend";
                break;

            default:
                task().text = "Yet to fill out this subquest type";
                break;
        }

        UISteps.Add(step);
    }

    public void MarkTaskAsComplete(int i)
    {
        UISteps[i].transform.GetChild(0).GetComponent<Toggle>().isOn = true;
    }
}
