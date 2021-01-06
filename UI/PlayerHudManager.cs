using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudManager : MonoBehaviour
{
    public GameObject Interact;
    public GameObject Attack;
    public GameObject holder;

    public GameObject CineCam;

    QuestGenerator g;

    private void Awake()
    {
        g = FindObjectOfType<QuestGenerator>();
    }

    private void Update()
    {
        if (g.g.currentTaskIndex == g.g.Task.Count)
            return;

        if (CineCam.activeInHierarchy)
            holder.SetActive(false);
        else
            holder.SetActive(true);

        if (g.g.Task[g.g.currentTaskIndex].Subquest == SubQuestType.Q_type.Attack)
            Attack.SetActive(true);
        else
            Attack.SetActive(false);

        if (g.g.Task[g.g.currentTaskIndex].Subquest == SubQuestType.Q_type.Get || g.g.Task[g.g.currentTaskIndex].Subquest == SubQuestType.Q_type.Talk || g.g.Task[g.g.currentTaskIndex].Subquest == SubQuestType.Q_type.Give)
            Interact.SetActive(true);
        else
            Interact.SetActive(false);


    }
}
