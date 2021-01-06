using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour
{
    public NavMeshAgent agent;
    Animator anim;
    public bool RequiresNewAction = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetUpAgent(int i)
    {
        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.autoRepath = false;
        agent.speed = 1.5f;
        agent.avoidancePriority = i;
    }

    private void FixedUpdate()
    {
        if (agent == null)
            return;

        if (agent.remainingDistance < 3f && !RequiresNewAction)
        {
            RequiresNewAction = true;
            agent.isStopped = true;
        }
        else if(agent.remainingDistance > 3f && !agent.pathPending)
        {
            agent.isStopped = false;
        }

        if (agent.isStopped)
            anim.SetBool("Walking", false);
        else
            anim.SetBool("Walking", true);
    }

    public void SetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        RequiresNewAction = false;
        agent.isStopped = false;
        //anim.SetBool("Walking", true);
    }


}
